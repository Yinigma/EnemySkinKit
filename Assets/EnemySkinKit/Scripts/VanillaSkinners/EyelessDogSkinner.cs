using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class EyelessDogSkinner : BaseSkinner, EyelessDogEventHandler
    {
        protected const string TEETH_TOP_PATH = "MouthDogModel/AnimContainer/Armature/Neck1Container/Neck1/Neck2/JawUpper/TeethTop";
        protected const string TEETH_BOTTOM_PATH = "MouthDogModel/AnimContainer/Armature/Neck1Container/Neck1/Neck2/JawLower/TeethBottom";
        protected const string LOD0_PATH = "MouthDogModel/ToothDogBody";
        protected const string LOD1_PATH = "MouthDogModel/ToothDogBodyLOD1";
        protected const string ANCHOR_PATH = "MouthDogModel/AnimContainer";
        protected const string SPAWN_PARTICLE_PATH = "MouthDogModel/AnimContainer/ParticleEffects/AppearFromGround";
        protected const string RUN_PARTICLE_PATH = "MouthDogModel/AnimContainer/ParticleEffects/RunDust";
        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaTopTeethMaterial;
        protected VanillaMaterial vanillaBottomTeethMaterial;

        protected Mesh vanillaTeethTopMesh;
        protected Mesh vanillaTeethBottomMesh;

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected ParticleSystem vanillaSpawnParticle;
        protected VanillaMaterial vanillaRunDustMaterial;
        protected VanillaMaterial vanillaSpawnMaterial;
        protected ParticleSystem vanillaRunDustParticle;

        
        protected ParticleSystem replacementRunDustParticle;
        protected ParticleSystem replacementSpawnParticle;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modCreatureEffects;

        protected EyelessDogSkin SkinData { get; }

        public EyelessDogSkinner( EyelessDogSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            MouthDogAI dog = enemy.GetComponent<MouthDogAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaTopTeethMaterial = SkinData.TopTeethMaterialAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBottomTeethMaterial = SkinData.BottomTeethMaterialAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTeethTopMesh = SkinData.TopTeethMeshAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaTeethBottomMesh = SkinData.BottomTeethMeshAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>());
            SkinData.ScreamAudioAction.ApplyToMap(dog.screamSFX, clipMap);
            SkinData.KillPlayerAudioAction.ApplyToMap(dog.killPlayerSFX, clipMap);
            SkinData.BreathingAudioAction.ApplyToMap(dog.breathingSFX, clipMap);
            SkinData.GrowlAudioAction.ApplyToMap(dog.enemyBehaviourStates[1].VoiceClip, clipMap);
            SkinData.ChasingAudioAction.ApplyToMap(dog.enemyBehaviourStates[2].VoiceClip, clipMap);
            SkinData.LungeAudioAction.ApplyToMap(dog.enemyBehaviourStates[3].SFXClip, clipMap);
            SkinData.StunAudioAction.ApplyToMap(dog.enemyType.stunSFX, clipMap);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }
            modCreatureVoice = CreateAudioReflector(dog.creatureVoice, clipMap, dog.NetworkObjectId);
            dog.creatureVoice.mute = true;
            modCreatureEffects = CreateAudioReflector(dog.creatureSFX, clipMap, dog.NetworkObjectId);
            dog.creatureSFX.mute = true;

            vanillaRunDustParticle = dog.transform.Find(RUN_PARTICLE_PATH)?.GetComponent<ParticleSystem>();
            vanillaSpawnParticle = dog.transform.Find(SPAWN_PARTICLE_PATH)?.GetComponent<ParticleSystem>();

            vanillaRunDustMaterial = SkinData.RunDustMaterialAction.Apply(vanillaRunDustParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSpawnMaterial = SkinData.SpawnDustMaterialAction.Apply(vanillaSpawnParticle.GetComponent<ParticleSystemRenderer>(), 0);

            replacementRunDustParticle = SkinData.RunDustParticleAction.Apply(vanillaRunDustParticle);
            replacementSpawnParticle = SkinData.SpawnDustParticleAction.Apply(vanillaSpawnParticle);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() 
                {
                    //what is this rig?
                    { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") },
                    { "Neck1Container", enemy.transform.Find($"{ANCHOR_PATH}/Armature/Neck1Container") }
                }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(dog, this);
        }

        public override void Remove(GameObject enemy)
        {
            MouthDogAI dog = enemy.GetComponent<MouthDogAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(dog, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.TopTeethMaterialAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaTopTeethMaterial);
            SkinData.BottomTeethMaterialAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBottomTeethMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.TopTeethMeshAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethTopMesh);
            SkinData.BottomTeethMeshAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethBottomMesh);

            DestroyAudioReflector(modCreatureVoice);
            dog.creatureVoice.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            dog.creatureSFX.mute = false;

            if (vanillaRunDustParticle != null)
            {
                SkinData.RunDustParticleAction.Remove(vanillaRunDustParticle, replacementRunDustParticle);
                SkinData.RunDustMaterialAction.Remove(vanillaRunDustParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaRunDustMaterial);
            }
            
            if(vanillaSpawnParticle != null)
            {
                SkinData.SpawnDustParticleAction.Remove(vanillaSpawnParticle, replacementSpawnParticle);
                SkinData.SpawnDustMaterialAction.Remove(vanillaSpawnParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSpawnMaterial);
            }

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }
    }
}