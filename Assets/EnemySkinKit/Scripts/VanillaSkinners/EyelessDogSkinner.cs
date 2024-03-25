using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class EyelessDogSkinner : BaseSkinner
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

        public EyelessDogSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction topTeethMeshAction, 
            StaticMeshAction bottomTeethMeshAction, 
            MaterialAction bodyMaterialAction, 
            MaterialAction topTeethMaterialAction, 
            MaterialAction bottomTeethMaterialAction,
            AudioAction screamAudioAction,
            AudioAction killPlayerAudioAction,
            AudioAction breathingAudioAction
        ) : base(muteSoundEffects, muteVoice)
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
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaTopTeethMaterial = TopTeethMaterialAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBottomTeethMaterial = BottomTeethMaterialAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTeethTopMesh = TopTeethMeshAction.Apply(enemy.transform.Find(TEETH_TOP_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaTeethBottomMesh = BottomTeethMeshAction.Apply(enemy.transform.Find(TEETH_BOTTOM_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaScreamAudio = ScreamAudioAction.Apply(ref enemy.GetComponent<MouthDogAI>().screamSFX);
            vanillaKillPlayerAudio = KillPlayerAudioAction.Apply(ref enemy.GetComponent<MouthDogAI>().killPlayerSFX);
            vanillaBreathingAudio = BreathingAudioAction.Apply(ref enemy.GetComponent<MouthDogAI>().breathingSFX);
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
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
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
    }
}