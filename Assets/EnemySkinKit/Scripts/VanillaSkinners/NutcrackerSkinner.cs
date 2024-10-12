using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class NutcrackerSkinner : BaseSkinner, NutcrackerEventHandler
    {
        protected const string LOD0_PATH = "MeshContainer/LOD0";
        protected const string LOD1_PATH = "MeshContainer/LOD1";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer/metarig";
        protected const string ANIM_EVENTS_PATH = "MeshContainer/AnimContainer";
        protected const string BLOOD_FOUNTAIN_PARTICLE = "MeshContainer/AnimContainer/metarig/spinecontainer/spine/spine.001/BloodSpurtParticle (1)/BloodSpurtParticle";
        protected VanillaMaterial vanillaBodyMaterial;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modLongRange;
        protected AudioReflector modTorsoTurn;
        protected VanillaMaterial vanillaSpurtMat;
        protected VanillaMaterial vanillaFountainMat;
        protected ParticleSystem vanillaSpurtParticle;
        protected ParticleSystem replacementFountainParticle;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected NutcrackerSkin SkinData { get; }

        public NutcrackerSkinner( NutcrackerSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            NutcrackerEnemyAI nutcracker = enemy.GetComponent<NutcrackerEnemyAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.TorsoTurnAudioAction.ApplyToMap(nutcracker.torsoTurnAudio.clip, clipMap);
            SkinData.TorsoFinishTurningAudioListAction.ApplyToMap(nutcracker.torsoFinishTurningClips, clipMap);
            SkinData.AimAudioAction.ApplyToMap(nutcracker.aimSFX, clipMap);
            SkinData.KickAudioAction.ApplyToMap(nutcracker.kickSFX, clipMap);
            SkinData.AngryDrumsAudioAction.ApplyToMap(nutcracker.creatureVoice.clip, clipMap);
            SkinData.HitEyeAudioAction.ApplyToMap(nutcracker.enemyType.audioClips[0], clipMap);
            SkinData.HitEyeAudioAction.ApplyToMap(nutcracker.enemyType.audioClips[1], clipMap);
            SkinData.HitEyeAudioAction.ApplyToMap(nutcracker.enemyType.audioClips[2], clipMap);
            SkinData.HitEyeAudioAction.ApplyToMap(nutcracker.enemyType.audioClips[3], clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(nutcracker.enemyType.hitBodySFX, clipMap);
            if (audioAnimEvents!=null)
            {
                SkinData.JointSqueaksAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
                SkinData.FootstepsAudioListAction.ApplyToMap(audioAnimEvents.randomClips2, clipMap);
            }

            modCreatureEffects = CreateAudioReflector(nutcracker.creatureSFX, clipMap, nutcracker.NetworkObjectId); 
            nutcracker.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(nutcracker.creatureVoice, clipMap, nutcracker.NetworkObjectId); 
            nutcracker.creatureVoice.mute = true;
            modLongRange = CreateAudioReflector(nutcracker.longRangeAudio, clipMap, nutcracker.NetworkObjectId); 
            nutcracker.longRangeAudio.mute = true;
            modTorsoTurn = CreateAudioReflector(nutcracker.torsoTurnAudio, clipMap, nutcracker.NetworkObjectId); 
            nutcracker.torsoTurnAudio.mute = true;

            vanillaSpurtMat = SkinData.BloodSpurtMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaFountainMat = SkinData.BloodFountainMaterialAction.Apply(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystemRenderer>(), 0);

            vanillaSpurtParticle = SkinData.BloodSpurtParticleAction.ApplyRef(ref audioAnimEvents.particle);
            replacementFountainParticle = SkinData.BloodFountainParticleAction.Apply(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystem>());

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "spinecontainer", enemy.transform.Find($"{ ANCHOR_PATH }/spinecontainer") } }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(nutcracker, this);
        }

        public override void Remove(GameObject enemy)
        {
            NutcrackerEnemyAI nutcracker = enemy.GetComponent<NutcrackerEnemyAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(nutcracker, this);

            DestroyAudioReflector(modCreatureEffects);
            nutcracker.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            nutcracker.creatureVoice.mute = false;
            DestroyAudioReflector(modLongRange);
            nutcracker.longRangeAudio.mute = false;
            DestroyAudioReflector(modTorsoTurn);
            nutcracker.torsoTurnAudio.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            SkinData.BloodSpurtParticleAction.RemoveRef(ref audioAnimEvents.particle, vanillaSpurtParticle);
            SkinData.BloodFountainParticleAction.Remove(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystem>(), replacementFountainParticle);

            SkinData.BloodSpurtMaterialAction.Remove(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSpurtMat);
            SkinData.BloodFountainMaterialAction.Remove(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystemRenderer>(), 0, vanillaFountainMat);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }
    }

}