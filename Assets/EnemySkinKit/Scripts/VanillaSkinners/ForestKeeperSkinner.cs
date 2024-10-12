using AntlerShed.EnemySkinKit.AudioReflection;
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
        protected const string CLOSE_WIDE_PATH = "FGiantModelContainer/CloseWideSFX";
        protected const string SMOKE_PATH = "FireParticlesContainer/Smoke (1)";
        protected const string FIRE_PATH = "FireParticlesContainer/LingeringFire";
        protected const string FLASH_PATH = "FireParticlesContainer/BrightFlash";
        protected const string ANCHOR_PATH = "FGiantModelContainer/AnimContainer";

        protected VanillaMaterial vanillaBodyMaterial;
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

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        protected AudioReflector modCloseWide;
        protected AudioReflector modFarWide;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modBurning;
        protected AudioReflector modCreatureEffects;

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
            SkinData.DeathCryAudioAction.ApplyToMap(giant.dieSFX, clipMap);
            SkinData.FallAudioAction.ApplyToMap(giant.giantFall, clipMap);
            SkinData.BurnAudioAction.ApplyToMap(giant.giantBurningAudio.clip, clipMap);
            SkinData.StunAudioAction.ApplyToMap(giant.enemyType.stunSFX, clipMap);
            if (audioAnimEvents!=null)
            {
                SkinData.StompAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
                SkinData.RumbleAudioListAction.ApplyToMap(audioAnimEvents.randomClips2, clipMap);
                SkinData.RoarAudioAction.ApplyToMap(audioAnimEvents.audioClip, clipMap);
                SkinData.EatPlayerAudioAction.ApplyToMap(audioAnimEvents.audioClip2, clipMap);
            }

            AudioSource closeWideAudio = giant.transform.Find(CLOSE_WIDE_PATH)?.GetComponent<AudioSource>();
            if(closeWideAudio != null)
            {
                modCloseWide = CreateAudioReflector(closeWideAudio, clipMap, giant.NetworkObjectId);
                closeWideAudio.mute = true;
            }
            modFarWide = CreateAudioReflector(giant.farWideSFX, clipMap, giant.NetworkObjectId);
            giant.farWideSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(giant.creatureVoice, clipMap, giant.NetworkObjectId);
            giant.creatureVoice.mute = true;
            modBurning = CreateAudioReflector(giant.giantBurningAudio, clipMap, giant.NetworkObjectId);
            giant.giantBurningAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(giant.creatureSFX, clipMap, giant.NetworkObjectId);
            giant.creatureSFX.mute = true;

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

            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            AudioSource closeWideAudio = giant.transform.Find(CLOSE_WIDE_PATH)?.GetComponent<AudioSource>();
            if (closeWideAudio != null)
            {
                DestroyAudioReflector(modCloseWide);
                closeWideAudio.mute = false;
            }
            DestroyAudioReflector(modFarWide);
            giant.farWideSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            giant.creatureVoice.mute = false;
            DestroyAudioReflector(modBurning);
            giant.giantBurningAudio.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            giant.creatureSFX.mute = false;

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
    }

}
