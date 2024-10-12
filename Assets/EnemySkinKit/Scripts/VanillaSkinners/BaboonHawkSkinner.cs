using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BaboonHawkSkinner : BaseSkinner, BaboonHawkEventHandler
    {
        protected const string LOD0_PATH = "BaboonBirdModel/BodyLOD0";
        protected const string LOD1_PATH = "BaboonBirdModel/BodyLOD1";
        protected const string LOD2_PATH = "BaboonBirdModel/BodyLOD2";
        protected const string ANCHOR_PATH = "BaboonBirdModel/AnimContainer/metarig";
        protected const string ANIM_EFFECTS_PATH = "BaboonBirdModel/AnimContainer";
        protected const string SECONDARY_BLOOD_PARTICLE_PATH = "BaboonBirdModel/AnimContainer/metarig/spine/spine.001/spine.003/spine.004/HeadBoneContainer/spine.005/BloodSpurtParticle (1)/BloodParticle";

        protected VanillaMaterial vanillaBodyMaterial;
        protected AudioClip[] vanillaScreamAudio;
        protected AudioClip[] vanillaLaughAudio;
        protected AudioClip vanillaIntimidateAudio;
        protected AudioClip vanillaIntimidateVoice;
        protected AudioClip vanillaEnterFightAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaStabAudio;
        protected AudioClip vanillaDeathAudio;
        protected VanillaMaterial vanillaBloodMat;
        private VanillaMaterial vanillaSecondaryBloodMat;
        private VanillaMaterial vanillaBloodStabMat;
        protected AudioClip vanillaHitBodyAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected List<GameObject> activeAttachments;
        protected ParticleSystem vanillaBloodParticle;
        protected GameObject skinnedMeshReplacement;

        protected BaboonHawkSkin SkinData { get; }

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modAggressionAudio;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        public BaboonHawkSkinner
        (
            BaboonHawkSkin skinData
        )
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            BaboonBirdAI bbhawk = enemy.GetComponent<BaboonBirdAI>();
            BaboonHawkAudioEvents animAudioEvents = enemy.transform.Find(ANIM_EFFECTS_PATH)?.gameObject?.GetComponent<BaboonHawkAudioEvents>();

            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            SkinData.ScreamAudioListAction.ApplyToMap(bbhawk.cawScreamSFX, clipMap);
            SkinData.LaughAudioListAction.ApplyToMap(bbhawk.cawLaughSFX, clipMap);
            SkinData.IntimidateVoiceAction.ApplyToMap(bbhawk.enemyType.audioClips[1], clipMap);
            SkinData.IntimidateAudioAction.ApplyToMap(bbhawk.enemyType.audioClips[2], clipMap);
            SkinData.EnterFightAction.ApplyToMap(bbhawk.enemyType.audioClips[3], clipMap);
            SkinData.KillPlayerAudioAction.ApplyToMap(bbhawk.enemyType.audioClips[4], clipMap);
            SkinData.DeathAudioAction.ApplyToMap(bbhawk.enemyType.audioClips[5], clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(bbhawk.enemyType.hitBodySFX, clipMap);

            if (animAudioEvents != null)
            {
                SkinData.FootstepsAudioAction.ApplyToMap(animAudioEvents.randomClips, clipMap);
            }

            modCreatureVoice = CreateAudioReflector(bbhawk.creatureVoice, clipMap, bbhawk.NetworkObjectId);
            bbhawk.creatureVoice.mute = true;
            modAggressionAudio = CreateAudioReflector(bbhawk.aggressionAudio, clipMap, bbhawk.NetworkObjectId);
            bbhawk.aggressionAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(bbhawk.creatureSFX, clipMap, bbhawk.NetworkObjectId);
            bbhawk.creatureSFX.mute = true;

            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            ParticleSystem secBloodParticle = bbhawk.transform.Find(SECONDARY_BLOOD_PARTICLE_PATH).GetComponent<ParticleSystem>();

            vanillaBloodMat = SkinData.BloodMaterialAction.Apply(animAudioEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSecondaryBloodMat = SkinData.BloodMaterialAction.Apply(secBloodParticle?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBloodStabMat = SkinData.BloodMaterialAction.Apply(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0);

            vanillaBloodParticle = SkinData.BloodParticleAction.ApplyRef(ref animAudioEvents.particle);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(bbhawk, this);
        }

        public override void Remove(GameObject enemy)
        {
            BaboonBirdAI bbhawk = enemy.GetComponent<BaboonBirdAI>();
            BaboonHawkAudioEvents animAudioEvents = enemy.transform.Find(ANIM_EFFECTS_PATH)?.gameObject?.GetComponent<BaboonHawkAudioEvents>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bbhawk, this);
            DestroyAudioReflector(modAggressionAudio);
            DestroyAudioReflector(modCreatureEffects);
            DestroyAudioReflector(modCreatureVoice);
            bbhawk.creatureVoice.mute = false;
            bbhawk.aggressionAudio.mute = false;
            bbhawk.creatureSFX.mute = false;
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.ScreamAudioListAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawScreamSFX, vanillaScreamAudio);
            SkinData.LaughAudioListAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawLaughSFX, vanillaLaughAudio);

            SkinData.BloodParticleAction.RemoveRef(ref animAudioEvents.particle, vanillaBloodParticle);

            ParticleSystem secBloodParticle = bbhawk.transform.Find(SECONDARY_BLOOD_PARTICLE_PATH).GetComponent<ParticleSystem>();

            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodMat);
            SkinData.BloodMaterialAction.Remove(secBloodParticle?.GetComponent<ParticleSystemRenderer>(), 0, vanillaSecondaryBloodMat);
            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0, vanillaBloodStabMat);

            vanillaBloodParticle = SkinData.BloodParticleAction.ApplyRef(ref animAudioEvents.particle);

            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0, vanillaBloodStabMat);

            if (animAudioEvents != null)
            {
                SkinData.FootstepsAudioAction.Remove(ref animAudioEvents.randomClips, vanillaFootstepsAudio);
            }
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