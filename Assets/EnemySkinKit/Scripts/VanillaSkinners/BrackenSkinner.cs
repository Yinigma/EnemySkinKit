using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BrackenSkinner : BaseSkinner, BrackenEventHandler
    {
        protected const string MESH_PATH = "FlowermanModel/LOD1";
        protected const string LEFT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/LeftEye";
        protected const string RIGHT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/RightEye";
        protected const string POOF_PARTICLE_PATH = "FlowermanModel/AnimContainer/PoofParticle";
        protected const string ANCHOR_PATH = "FlowermanModel/AnimContainer";
        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaLeafMaterial;
        protected VanillaMaterial vanillaRightEyeMaterial;
        protected VanillaMaterial vanillaLeftEyeMaterial;
        protected ParticleSystem vanillaPoofParticle;

        protected Mesh vanillaRightEyeMesh;
        protected Mesh vanillaLeftEyeMesh;

        protected AudioClip vanillaAngerSound;
        protected AudioClip vanillaKillSound;
        protected VanillaMaterial vanillaPoofParticleMaterial;
        protected AudioClip vanillaFoundSound;
        protected AudioClip[] vanillaLeafRustleSounds;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modAngerVoice;
        protected AudioReflector modNeckSnap;

        protected BrackenSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        public BrackenSkinner
        (
            BrackenSkin skinData
        )
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            FlowermanAI bracken = enemy.GetComponent<FlowermanAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();

            //Audio - populate map
            SkinData.AngerAudioAction.ApplyToMap(bracken.creatureAngerVoice.clip, clipMap);
            SkinData.NeckSnapAudioAction.ApplyToMap(bracken.crackNeckSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(bracken.enemyType.hitBodySFX, clipMap);
            SkinData.StunAudioAction.ApplyToMap(bracken.enemyType.stunSFX, clipMap);
            if (audioAnimEvents != null)
            {
                SkinData.FoundAudioAction.ApplyToMap(audioAnimEvents.audioClip, clipMap);
                SkinData.LeafRustleAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }

            //THEN spin up the audio sources
            modCreatureVoice = CreateAudioReflector(bracken.creatureVoice, clipMap, bracken.NetworkObjectId);
            bracken.creatureVoice.mute = true;
            modAngerVoice = CreateAudioReflector(bracken.creatureAngerVoice, clipMap, bracken.NetworkObjectId);
            bracken.creatureAngerVoice.mute = true;
            modCreatureEffects = CreateAudioReflector(bracken.creatureSFX, clipMap, bracken.NetworkObjectId);
            bracken.creatureSFX.mute = true;
            modNeckSnap = CreateAudioReflector(bracken.crackNeckAudio, clipMap, bracken.NetworkObjectId);
            bracken.crackNeckAudio.mute = true;

            //Attachments
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() );

            //Materials
            vanillaLeafMaterial = SkinData.LeafMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftEyeMaterial = SkinData.LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = SkinData.RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));

            //Meshes
            vanillaLeftEyeMesh = SkinData.LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = SkinData.RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());

            //Particles
            vanillaPoofParticleMaterial = SkinData.DeathSporeMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaPoofParticle = SkinData.DeathSporeParticleAction.ApplyRef(ref audioAnimEvents.particle);
            //vanillaPoofParticle = DeathSporeAction.Apply(ref );
            
            EnemySkinRegistry.RegisterEnemyEventHandler(bracken, this);
        }

        public override void Remove(GameObject enemy)
        {
            FlowermanAI bracken = enemy.GetComponent<FlowermanAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bracken, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();

            DestroyAudioReflector(modCreatureEffects);
            DestroyAudioReflector(modCreatureVoice);
            DestroyAudioReflector(modNeckSnap);
            DestroyAudioReflector(modAngerVoice);
            bracken.creatureVoice.mute = false;
            bracken.creatureSFX.mute = false;
            bracken.creatureAngerVoice.mute = false;
            bracken.crackNeckAudio.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.LeafMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeafMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 1, vanillaBodyMaterial);
            SkinData.LeftEyeMaterialAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftEyeMaterial);
            SkinData.RightEyeMaterialAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightEyeMaterial);
            SkinData.BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, skinnedMeshReplacement);
            SkinData.LeftEyeMeshAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftEyeMesh);
            SkinData.RightEyeMeshAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightEyeMesh);
            SkinData.AngerAudioAction.RemoveFromSource(bracken.creatureAngerVoice, vanillaAngerSound);
            SkinData.NeckSnapAudioAction.Remove(ref bracken.crackNeckSFX, vanillaKillSound);

            SkinData.DeathSporeParticleAction.RemoveRef(ref audioAnimEvents.particle, vanillaPoofParticle);
            SkinData.DeathSporeMaterialAction.Remove(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaPoofParticleMaterial);

            if (audioAnimEvents != null)
            {
                SkinData.FoundAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaFoundSound);
                SkinData.LeafRustleAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaLeafRustleSounds);
            }
        }
    }
}