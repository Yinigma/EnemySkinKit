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

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        protected BrackenSkin SkinData { get; }

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
            if(VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(bracken.creatureVoice, "modVoice");
                bracken.creatureVoice.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(bracken.creatureSFX, "modEffects");
                bracken.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                }
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() );
            vanillaLeafMaterial = SkinData.LeafMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftEyeMaterial = SkinData.LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = SkinData.RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaLeftEyeMesh = SkinData.LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = SkinData.RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngerSound = SkinData.AngerAudioAction.ApplyToSource(bracken.creatureAngerVoice);
            vanillaKillSound = SkinData.NeckSnapAudioAction.Apply(ref bracken.crackNeckSFX);

            vanillaPoofParticleMaterial = SkinData.DeathSporeMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaPoofParticle = SkinData.DeathSporeParticleAction.ApplyRef(ref audioAnimEvents.particle);
            //vanillaPoofParticle = DeathSporeAction.Apply(ref );
            if (audioAnimEvents != null)
            {
                vanillaFoundSound = SkinData.FoundAudioAction.Apply(ref audioAnimEvents.audioClip);
                vanillaLeafRustleSounds = SkinData.LeafRustleAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(bracken, this);
        }

        public override void Remove(GameObject enemy)
        {
            FlowermanAI bracken = enemy.GetComponent<FlowermanAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bracken, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                bracken.creatureVoice.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                bracken.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = bracken.creatureSFX;
                }
            }
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

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
            }
            if (VoiceSilenced)
            {
                modCreatureVoice.Stop();
            }
        }
    }
}