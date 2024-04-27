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
        protected Material vanillaBodyMaterial;
        protected Material vanillaTopTeethMaterial;
        protected Material vanillaBottomTeethMaterial;

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

        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction TopTeethMeshAction { get; }
        protected StaticMeshAction BottomTeethMeshAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction TopTeethMaterialAction { get; }
        protected MaterialAction BottomTeethMaterialAction { get; }
        protected AudioAction ScreamAudioAction { get; }
        protected AudioAction KillPlayerAudioAction { get; }
        protected AudioAction BreathingAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        protected AudioAction StunAudioAction { get; }
        protected AudioAction GrowlAudioAction { get; }
        protected AudioAction ChasingAudioAction { get; }
        protected AudioAction LungeAudioAction { get; }
        protected AudioListAction FootstepsAudioListAction { get; }

        protected bool VoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureVoice;

        public EyelessDogSkinner
        (
            ArmatureAttachment[] attachments,
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction topTeethMeshAction, 
            StaticMeshAction bottomTeethMeshAction, 
            MaterialAction bodyMaterialAction, 
            MaterialAction topTeethMaterialAction, 
            MaterialAction bottomTeethMaterialAction,
            AudioAction screamAudioAction,
            AudioAction killPlayerAudioAction,
            AudioAction breathingAudioAction,
            AudioAction stunAudioAction,
            AudioAction growlAudioAction,
            AudioAction chasingAudioAction,
            AudioAction lungeAudioAction,
            AudioListAction footstepsAudioListAction
        )
        {
            BodyMeshAction = bodyMeshAction;
            TopTeethMeshAction = topTeethMeshAction;
            BottomTeethMeshAction = bottomTeethMeshAction;
            BodyMaterialAction = bodyMaterialAction;
            TopTeethMaterialAction = topTeethMaterialAction;
            BottomTeethMaterialAction = bottomTeethMaterialAction;
            ScreamAudioAction = screamAudioAction;
            KillPlayerAudioAction = killPlayerAudioAction;
            BreathingAudioAction = breathingAudioAction;
            Attachments = attachments;
            StunAudioAction = stunAudioAction;
            GrowlAudioAction = growlAudioAction;
            ChasingAudioAction = chasingAudioAction;
            LungeAudioAction = lungeAudioAction;
            FootstepsAudioListAction = footstepsAudioListAction;
        }

        public override void Apply(GameObject enemy)
        {
            MouthDogAI dog = enemy.GetComponent<MouthDogAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaTopTeethMaterial = TopTeethMaterialAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBottomTeethMaterial = BottomTeethMaterialAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTeethTopMesh = TopTeethMeshAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaTeethBottomMesh = BottomTeethMeshAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaScreamAudio = ScreamAudioAction.Apply(ref dog.screamSFX);
            vanillaKillPlayerAudio = KillPlayerAudioAction.Apply(ref dog.killPlayerSFX);
            vanillaBreathingAudio = BreathingAudioAction.Apply(ref dog.breathingSFX);
            vanillaGrowlAudio = GrowlAudioAction.Apply(ref dog.enemyBehaviourStates[1].VoiceClip);
            vanillaChaseAudio = ChasingAudioAction.Apply(ref dog.enemyBehaviourStates[2].VoiceClip);
            vanillaLungeAudio = LungeAudioAction.Apply(ref dog.enemyBehaviourStates[3].SFXClip);
            if (audioAnimEvents != null)
            {
                vanillaFootstepsAudio = FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(dog.creatureVoice, "modVoice");
                dog.creatureVoice.mute = true;
            }
            BodyMeshAction.Apply
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
            TopTeethMaterialAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaTopTeethMaterial);
            BottomTeethMaterialAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBottomTeethMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            TopTeethMeshAction.Remove(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethTopMesh);
            BottomTeethMeshAction.Remove(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaTeethBottomMesh);
            ScreamAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().screamSFX, vanillaScreamAudio);
            KillPlayerAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().killPlayerSFX, vanillaKillPlayerAudio);
            BreathingAudioAction.Remove(ref enemy.GetComponent<MouthDogAI>().breathingSFX, vanillaBreathingAudio);
            GrowlAudioAction.Remove(ref dog.enemyBehaviourStates[1].VoiceClip, vanillaGrowlAudio);
            ChasingAudioAction.Remove(ref dog.enemyBehaviourStates[2].VoiceClip, vanillaChaseAudio);
            LungeAudioAction.Remove(ref dog.enemyBehaviourStates[3].VoiceClip, vanillaLungeAudio);
            if (audioAnimEvents != null)
            {
                FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnEnterChasingState(MouthDogAI dog)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(BreathingAudioAction.WorkingClip(vanillaBreathingAudio));
                //WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, ChasingAudioAction.WorkingClip(dog.enemyBehaviourStates[2].VoiceClip), modCreatureVoice.volume);
            }
        }

        public void OnEnterSuspiciousState(MouthDogAI dog)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(GrowlAudioAction.WorkingClip(vanillaGrowlAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, GrowlAudioAction.WorkingClip(vanillaGrowlAudio), modCreatureVoice.volume);
            }
        }

        public void OnChaseHowl(MouthDogAI dog)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(ScreamAudioAction.WorkingClip(vanillaScreamAudio));
            }
        }
        
        public void OnKillPlayer(MouthDogAI instance, GameNetcodeStuff.PlayerControllerB killedPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.pitch = Random.Range(0.96f, 1.04f);
                
                modCreatureVoice.PlayOneShot(KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio), 1f);
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