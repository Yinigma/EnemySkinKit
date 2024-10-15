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
        protected const string SMOKE_PATH = "FireParticlesContainer/Smoke (1)";
        protected const string FIRE_PATH = "FireParticlesContainer/LingeringFire";
        protected const string FLASH_PATH = "FireParticlesContainer/BrightFlash";
        protected const string ANCHOR_PATH = "FGiantModelContainer/AnimContainer";

        protected VanillaMaterial vanillaBodyMaterial;
        protected AudioClip vanillaRoarAudio;
        protected AudioClip vanillaEatPlayerAudio;
        protected AudioClip vanillaFallAudio;
        protected AudioClip vanillaDeathCryAudio;
        protected AudioClip vanillaBurnAudio;
        protected AudioClip[] vanillaStompAudio;
        protected AudioClip[] vanillaRumbleAudio;
        protected List<GameObject> activeAttachments;
        protected VanillaMaterial vanillaBloodMaterial;
        protected VanillaMaterial vanillaSmokeMaterial;
        protected VanillaMaterial vanillaFireMaterial;
        protected VanillaMaterial vanillaFlashMaterial;
        protected ParticleSystem vanillaBloodParticle;
        protected ParticleSystem replacementSmokeParticle;
        protected ParticleSystem replacementFireParticle;
        protected ParticleSystem replacementFlashParticle;
        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureVoice;

        protected ForestKeeperSkin SkinData { get; }

        public ForestKeeperSkinner(ForestKeeperSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            ForestGiantAI giant = enemy.GetComponent<ForestGiantAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaDeathCryAudio = SkinData.DeathCryAudioAction.Apply(ref giant.dieSFX);
            vanillaFallAudio = SkinData.FallAudioAction.Apply(ref giant.giantFall);
            vanillaBurnAudio = SkinData.BurnAudioAction.ApplyToSource(giant.giantBurningAudio);
            if(audioAnimEvents!=null)
            {
                vanillaStompAudio = SkinData.StompAudioListAction.Apply(ref audioAnimEvents.randomClips);
                vanillaRumbleAudio = SkinData.RumbleAudioListAction.Apply(ref audioAnimEvents.randomClips2);
                vanillaRoarAudio = SkinData.RoarAudioAction.Apply(ref audioAnimEvents.audioClip);
                vanillaEatPlayerAudio = SkinData.EatPlayerAudioAction.Apply(ref audioAnimEvents.audioClip2);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(giant.creatureVoice, "modVoice");
                giant.creatureVoice.mute = true;
            }

            ParticleSystem vanillaSmoke = giant.transform.Find(SMOKE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaFire = giant.transform.Find(FIRE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaFlash = giant.transform.Find(FLASH_PATH)?.GetComponent<ParticleSystem>();

            vanillaBloodMaterial = SkinData.BloodMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSmokeMaterial = SkinData.SmokeMaterialAction.Apply(vanillaSmoke?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaFireMaterial = SkinData.FireMaterialAction.Apply(vanillaFire?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaFlashMaterial = SkinData.FlashMaterialAction.Apply(vanillaFlash?.GetComponent<ParticleSystemRenderer>(), 0);

            vanillaBloodParticle = SkinData.BloodParticleAction.ApplyRef(ref audioAnimEvents.particle);
            replacementSmokeParticle = SkinData.SmokeParticleAction.Apply(vanillaSmoke);
            replacementFireParticle = SkinData.FireParticleAction.Apply(vanillaFire);
            replacementFlashParticle = SkinData.FlashParticleAction.Apply(vanillaFlash);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
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
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.DeathCryAudioAction.Remove(ref giant.giantCry, vanillaDeathCryAudio);
            SkinData.FallAudioAction.Remove(ref giant.giantFall, vanillaFallAudio);
            SkinData.BurnAudioAction.RemoveFromSource(giant.giantBurningAudio, vanillaBurnAudio);
            if (audioAnimEvents != null)
            {
                SkinData.StompAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaStompAudio);
                SkinData.RumbleAudioListAction.Remove(ref audioAnimEvents.randomClips2, vanillaRumbleAudio);
                SkinData.RoarAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaRoarAudio);
                SkinData.EatPlayerAudioAction.Remove(ref audioAnimEvents.audioClip2, vanillaEatPlayerAudio);
            }

            ParticleSystem vanillaSmoke = giant.transform.Find(SMOKE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaFire = giant.transform.Find(FIRE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaFlash = giant.transform.Find(FLASH_PATH)?.GetComponent<ParticleSystem>();

            SkinData.BloodParticleAction.RemoveRef(ref audioAnimEvents.particle, vanillaBloodParticle);
            SkinData.SmokeParticleAction.Remove(vanillaSmoke, replacementSmokeParticle);
            SkinData.FireParticleAction.Remove(vanillaFire, replacementFireParticle);
            SkinData.FlashParticleAction.Remove(vanillaFlash, replacementFlashParticle);

            SkinData.BloodMaterialAction.Remove(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodMaterial);
            SkinData.SmokeMaterialAction.Remove(vanillaSmoke?.GetComponent<ParticleSystemRenderer>(), 0, vanillaSmokeMaterial);
            SkinData.FireMaterialAction.Remove(vanillaFire?.GetComponent<ParticleSystemRenderer>(), 0, vanillaFireMaterial);
            SkinData.FlashMaterialAction.Remove(vanillaFlash?.GetComponent<ParticleSystemRenderer>(), 0, vanillaFlashMaterial);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnKilled(EnemyAI enemy)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.DeathCryAudioAction.WorkingClip(vanillaDeathCryAudio));
            }
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }
    }

}
