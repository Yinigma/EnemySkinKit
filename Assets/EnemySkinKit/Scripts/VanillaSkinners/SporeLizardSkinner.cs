using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class SporeLizardSkinner : BaseSkinner, SporeLizardEventHandler
    {
        protected const string BODY_PATH = "PufferModel/BezierCurve";
        protected const string ANCHOR_PATH = "PufferModel/AnimContainer/Armature";
        protected const string ANIM_EVENT_PATH = "PufferModel/AnimContainer";
        protected VanillaMaterial vanillaBodyMaterial;

        protected AudioClip[] vanillaFrightenedAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaStompAudio;
        protected AudioClip vanillaAngryAudio;
        protected AudioClip vanillaPuffAudio;
        protected AudioClip vanillaNervousAudio;
        protected AudioClip vanillaRattleAudio;
        protected AudioClip vanillaBiteAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureEffects;
        protected SporeLizardSkin SkinData { get; }

        public SporeLizardSkinner(SporeLizardSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            PufferAI lizard = enemy.GetComponent<PufferAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaFrightenedAudio = SkinData.FrightenedAudioListAction.Apply(ref lizard.frightenSFX);
            vanillaStompAudio = SkinData.StompAudioAction.Apply(ref lizard.stomp);
            vanillaAngryAudio = SkinData.AngryAudioAction.Apply(ref lizard.angry);
            vanillaPuffAudio = SkinData.PuffAudioAction.Apply(ref lizard.puff);
            vanillaNervousAudio = SkinData.NervousMumbleAudioAction.Apply(ref lizard.nervousMumbling);
            vanillaRattleAudio = SkinData.RattleTailAudioAction.Apply(ref lizard.rattleTail);
            vanillaBiteAudio = SkinData.BiteAudioAction.Apply(ref lizard.bitePlayerSFX);
            if(audioAnimEvents!=null)
            {
                vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(lizard.creatureSFX, "modEffects");
                lizard.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                }
            }
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(lizard, this);
        }

        public override void Remove(GameObject enemy)
        {
            PufferAI lizard = enemy.GetComponent<PufferAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(lizard, this);
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                lizard.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = lizard.creatureSFX;
                }
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.FrightenedAudioListAction.Remove(ref lizard.frightenSFX, vanillaFrightenedAudio);
            SkinData.StompAudioAction.Remove(ref lizard.stomp, vanillaStompAudio);
            SkinData.AngryAudioAction.Remove(ref lizard.angry, vanillaAngryAudio);
            SkinData.PuffAudioAction.Remove(ref lizard.puff, vanillaPuffAudio);
            SkinData.NervousMumbleAudioAction.Remove(ref lizard.nervousMumbling, vanillaNervousAudio);
            SkinData.RattleTailAudioAction.Remove(ref lizard.rattleTail, vanillaRattleAudio);
            SkinData.BiteAudioAction.Remove(ref lizard.bitePlayerSFX, vanillaBiteAudio);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnShakeTail(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.AngryAudioAction.WorkingClip(vanillaAngryAudio));
            }
        }

        public void OnStomp(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.StompAudioAction.WorkingClip(vanillaStompAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.StompAudioAction.WorkingClip(vanillaStompAudio));
            }
        }

        public void OnPuff(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.PuffAudioAction.WorkingClip(vanillaPuffAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.PuffAudioAction.WorkingClip(vanillaPuffAudio));
            }
        }

        public void OnAlarmed(PufferAI pufferAI)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.RattleTailAudioAction.WorkingClip(vanillaRattleAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.RattleTailAudioAction.WorkingClip(vanillaRattleAudio));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if(EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }

}