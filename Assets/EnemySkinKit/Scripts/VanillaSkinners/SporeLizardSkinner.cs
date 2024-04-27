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
        protected Material vanillaBodyMaterial;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioClip[] vanillaFrightenedAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaStompAudio;
        protected AudioClip vanillaAngryAudio;
        protected AudioClip vanillaPuffAudio;
        protected AudioClip vanillaNervousAudio;
        protected AudioClip vanillaRattleAudio;
        protected AudioClip vanillaBiteAudio;
        protected List<GameObject> activeAttachments;

        protected AudioListAction FrightenedAudioAction { get; }
        protected AudioAction StompAudioAction { get; }
        protected AudioAction AngryAudioAction { get; }
        protected AudioAction PuffAudioAction { get; }
        protected AudioAction NervousAudioAction { get; }
        protected AudioAction RattleAudioAction { get; }
        protected AudioAction BiteAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioListAction FootstepsAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureEffects;
        public SporeLizardSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioListAction frightenedAudioAction,
            AudioAction stompAudioAction,
            AudioAction angryAudioAction,
            AudioAction puffAudioAction,
            AudioAction nervousAudioAction,
            AudioAction rattleAudioAction,
            AudioAction biteAudioAction,
            AudioAction hitBodyAudioAction,
            AudioListAction footstepsAudioAction
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            FrightenedAudioAction = frightenedAudioAction;
            StompAudioAction = stompAudioAction;
            AngryAudioAction = angryAudioAction;
            PuffAudioAction = puffAudioAction;
            NervousAudioAction = nervousAudioAction;
            RattleAudioAction = rattleAudioAction;
            BiteAudioAction = biteAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            FootstepsAudioAction = footstepsAudioAction;
            Attachments = attachments;
        }

        
        public override void Apply(GameObject enemy)
        {
            PufferAI lizard = enemy.GetComponent<PufferAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaFrightenedAudio = FrightenedAudioAction.Apply(ref lizard.frightenSFX);
            vanillaStompAudio = StompAudioAction.Apply(ref lizard.stomp);
            vanillaAngryAudio = AngryAudioAction.Apply(ref lizard.angry);
            vanillaPuffAudio = PuffAudioAction.Apply(ref lizard.puff);
            vanillaNervousAudio = NervousAudioAction.Apply(ref lizard.nervousMumbling);
            vanillaRattleAudio = RattleAudioAction.Apply(ref lizard.rattleTail);
            vanillaBiteAudio = BiteAudioAction.Apply(ref lizard.bitePlayerSFX);
            if(audioAnimEvents!=null)
            {
                vanillaFootstepsAudio = FootstepsAudioAction.Apply(ref audioAnimEvents.randomClips);
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
            BodyMeshAction.Apply
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
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            FrightenedAudioAction.Remove(ref lizard.frightenSFX, vanillaFrightenedAudio);
            StompAudioAction.Remove(ref lizard.stomp, vanillaStompAudio);
            AngryAudioAction.Remove(ref lizard.angry, vanillaAngryAudio);
            PuffAudioAction.Remove(ref lizard.puff, vanillaPuffAudio);
            NervousAudioAction.Remove(ref lizard.nervousMumbling, vanillaNervousAudio);
            RattleAudioAction.Remove(ref lizard.rattleTail, vanillaRattleAudio);
            BiteAudioAction.Remove(ref lizard.bitePlayerSFX, vanillaBiteAudio);
            if (audioAnimEvents != null)
            {
                FootstepsAudioAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnShakeTail(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, AngryAudioAction.WorkingClip(vanillaAngryAudio));
            }
        }

        public void OnStomp(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(StompAudioAction.WorkingClip(vanillaStompAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, StompAudioAction.WorkingClip(vanillaStompAudio));
            }
        }

        public void OnPuff(PufferAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(PuffAudioAction.WorkingClip(vanillaPuffAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, PuffAudioAction.WorkingClip(vanillaPuffAudio));
            }
        }

        public void OnAlarmed(PufferAI pufferAI)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(RattleAudioAction.WorkingClip(vanillaRattleAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, RattleAudioAction.WorkingClip(vanillaRattleAudio));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if(EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }

}