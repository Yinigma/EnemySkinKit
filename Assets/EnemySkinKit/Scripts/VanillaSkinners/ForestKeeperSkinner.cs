using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ForestKeeperSkinner : BaseSkinner, ForestKeeperEventHandler
    {
        protected const string LOD0_PATH = "FGiantModelContainer/BodyLOD0";
        protected const string LOD1_PATH = "FGiantModelContainer/BodyLOD1";
        protected const string LOD2_PATH = "FGiantModelContainer/BodyLOD2";
        protected const string ANCHOR_PATH = "FGiantModelContainer/AnimContainer";

        protected Material vanillaBodyMaterial;
        protected AudioClip vanillaRoarAudio;
        protected AudioClip vanillaEatPlayerAudio;
        protected AudioClip vanillaFallAudio;
        protected AudioClip vanillaDeathCryAudio;
        protected AudioClip vanillaBurnAudio;
        protected AudioClip[] vanillaStompAudio;
        protected AudioClip[] vanillaRumbleAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioAction FarWideAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        protected AudioAction StunAudioAction { get; }
        protected AudioAction RoarAudioAction { get; }
        protected AudioAction EatPlayerAudioAction { get; }
        protected AudioAction FallAudioAction { get; }
        protected AudioAction DeathCryAudioAction { get; }
        protected AudioAction BurnAudioAction { get; }
        protected AudioListAction StompAudioListAction { get; }
        protected AudioListAction RumbleAudioListAction { get; }

        protected bool VoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureVoice;

        public ForestKeeperSkinner
        (
            ArmatureAttachment[] attachments, 
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction stunAudioAction,
            AudioAction roarAudioAction,
            AudioAction eatPlayerAudioAction,
            AudioAction fallAudioAction,
            AudioAction deathCryAudioAction,
            AudioListAction stompAudioListAction,
            AudioListAction rumbleAudioListAction,
            AudioAction burnAudioAction
        )
        {
            
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            StunAudioAction = stunAudioAction;
            RoarAudioAction = roarAudioAction;
            EatPlayerAudioAction = eatPlayerAudioAction;
            FallAudioAction = fallAudioAction;
            DeathCryAudioAction = deathCryAudioAction;
            StompAudioListAction = stompAudioListAction;
            RumbleAudioListAction = rumbleAudioListAction;
            BurnAudioAction = burnAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            ForestGiantAI giant = enemy.GetComponent<ForestGiantAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaDeathCryAudio = DeathCryAudioAction.Apply(ref giant.dieSFX);
            vanillaFallAudio = FallAudioAction.Apply(ref giant.giantFall);
            vanillaBurnAudio = BurnAudioAction.ApplyToSource(giant.giantBurningAudio);
            if(audioAnimEvents!=null)
            {
                vanillaStompAudio = StompAudioListAction.Apply(ref audioAnimEvents.randomClips);
                vanillaRumbleAudio = RumbleAudioListAction.Apply(ref audioAnimEvents.randomClips2);
                vanillaRoarAudio = RoarAudioAction.Apply(ref audioAnimEvents.audioClip);
                vanillaEatPlayerAudio = EatPlayerAudioAction.Apply(ref audioAnimEvents.audioClip2);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(giant.creatureVoice, "modVoice");
                giant.creatureVoice.mute = true;
            }
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig") } }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(giant, this);
        }

        public override void Remove(GameObject enemy)
        {
            ForestGiantAI giant = enemy.GetComponent<ForestGiantAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(giant, this);
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                giant.creatureVoice.mute = false;
            }
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            DeathCryAudioAction.Remove(ref giant.giantCry, vanillaDeathCryAudio);
            FallAudioAction.Remove(ref giant.giantFall, vanillaFallAudio);
            BurnAudioAction.RemoveFromSource(giant.giantBurningAudio, vanillaBurnAudio);
            if (audioAnimEvents != null)
            {
                StompAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaStompAudio);
                RumbleAudioListAction.Remove(ref audioAnimEvents.randomClips2, vanillaRumbleAudio);
                RoarAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaRoarAudio);
                EatPlayerAudioAction.Remove(ref audioAnimEvents.audioClip2, vanillaEatPlayerAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnKilled(EnemyAI enemy)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(DeathCryAudioAction.WorkingClip(vanillaDeathCryAudio));
            }
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }
    }

}
