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

        protected AudioClip vanillaScreamAudio;
        protected AudioClip vanillaBreathingAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaGrowlAudio;
        protected AudioClip vanillaChaseAudio;
        protected AudioClip vanillaLungeAudio;
        protected AudioClip[] vanillaFootstepsAudio;

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected ParticleSystem vanillaSpawnParticle;
        protected VanillaMaterial vanillaRunDustMaterial;
        protected VanillaMaterial vanillaSpawnMaterial;
        protected ParticleSystem vanillaRunDustParticle;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureVoice;
        protected ParticleSystem replacementRunDustParticle;
        protected ParticleSystem replacementSpawnParticle;

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
            vanillaScreamAudio = SkinData.ScreamAudioAction.Apply(ref dog.screamSFX);
            vanillaKillPlayerAudio = SkinData.KillPlayerAudioAction.Apply(ref dog.killPlayerSFX);
            vanillaBreathingAudio = SkinData.BreathingAudioAction.Apply(ref dog.breathingSFX);
            vanillaGrowlAudio = SkinData.GrowlAudioAction.Apply(ref dog.enemyBehaviourStates[1].VoiceClip);
            vanillaChaseAudio = SkinData.ChasingAudioAction.Apply(ref dog.enemyBehaviourStates[2].VoiceClip);
            vanillaLungeAudio = SkinData.LungeAudioAction.Apply(ref dog.enemyBehaviourStates[3].SFXClip);
            if (audioAnimEvents != null)
            {
                vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(dog.creatureVoice, "modVoice");
                dog.creatureVoice.mute = true;
            }

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
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                dog.creatureVoice.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.TopTeethMaterialAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaTopTeethMaterial);
            SkinData.BottomTeethMaterialAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBottomTeethMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.TopTeethMeshAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethTopMesh);
            SkinData.BottomTeethMeshAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethBottomMesh);
            SkinData.ScreamAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().screamSFX, vanillaScreamAudio);
            SkinData.KillPlayerAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().killPlayerSFX, vanillaKillPlayerAudio);
            SkinData.BreathingAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().breathingSFX, vanillaBreathingAudio);
            SkinData.GrowlAudioAction.Remove(ref dog.enemyBehaviourStates[1].VoiceClip, vanillaGrowlAudio);
            SkinData.ChasingAudioAction.Remove(ref dog.enemyBehaviourStates[2].VoiceClip, vanillaChaseAudio);
            SkinData.LungeAudioAction.Remove(ref dog.enemyBehaviourStates[3].VoiceClip, vanillaLungeAudio);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }

            if(vanillaRunDustParticle != null)
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

        public void OnEnterChasingState(MouthDogAI dog)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.BreathingAudioAction.WorkingClip(vanillaBreathingAudio));
                //WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, ChasingAudioAction.WorkingClip(dog.enemyBehaviourStates[2].VoiceClip), modCreatureVoice.volume);
            }
        }

        public void OnEnterSuspiciousState(MouthDogAI dog)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.GrowlAudioAction.WorkingClip(vanillaGrowlAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.GrowlAudioAction.WorkingClip(vanillaGrowlAudio), modCreatureVoice.volume);
            }
        }

        public void OnChaseHowl(MouthDogAI dog)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.ScreamAudioAction.WorkingClip(vanillaScreamAudio));
            }
        }
        
        public void OnKillPlayer(MouthDogAI instance, GameNetcodeStuff.PlayerControllerB killedPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.pitch = Random.Range(0.96f, 1.04f);
                
                modCreatureVoice.PlayOneShot(SkinData.KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio), 1f);
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