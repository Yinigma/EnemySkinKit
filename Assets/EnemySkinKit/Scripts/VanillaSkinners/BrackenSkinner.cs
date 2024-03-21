using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BrackenSkinner : BaseSkinner
    {
        protected const string MESH_PATH = "FlowermanModel/LOD1";
        protected const string LEFT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/LeftEye";
        protected const string RIGHT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/RightEye";
        protected const string ANCHOR_PATH = "FlowermanModel/AnimContainer";
        protected Material vanillaBodyMaterial;
        protected Material vanillaLeafMaterial;
        protected Material vanillaRightEyeMaterial;
        protected Material vanillaLeftEyeMaterial;

        protected Mesh vanillaRightEyeMesh;
        protected Mesh vanillaLeftEyeMesh;

        protected AudioClip vanillaAngerSound;
        protected AudioClip vanillaKillSound;
        protected List<GameObject> activeAttachments;

        protected MaterialAction LeafMaterialAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftEyeMaterialAction { get; }
        protected MaterialAction RightEyeMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftEyeMeshAction { get; }
        protected StaticMeshAction RightEyeMeshAction { get; }
        protected AudioAction AngerAudioAction { get; }
        protected AudioAction SnapNeckAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public BrackenSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction leafMaterialAction,
            MaterialAction bodyMaterialAction,
            MaterialAction leftEyeMaterialAction,
            MaterialAction rightEyeMaterialAction,
            SkinnedMeshAction bodyMeshAction,
            StaticMeshAction leftEyeMeshAction,
            StaticMeshAction rightEyeMeshAction,
            AudioAction angerAudioAction,
            AudioAction snapNeckAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            LeafMaterialAction = leafMaterialAction;
            BodyMaterialAction = bodyMaterialAction;
            LeftEyeMaterialAction = leftEyeMaterialAction;
            RightEyeMaterialAction = rightEyeMaterialAction;
            BodyMeshAction = bodyMeshAction;
            LeftEyeMeshAction = leftEyeMeshAction;
            RightEyeMeshAction = rightEyeMeshAction;
            AngerAudioAction = angerAudioAction;
            SnapNeckAudioAction = snapNeckAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() );
            vanillaLeafMaterial = LeafMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaLeftEyeMaterial = LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaLeftEyeMesh = LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngerSound = AngerAudioAction.ApplyToSource(enemy.GetComponent<FlowermanAI>().creatureAngerVoice);
            vanillaKillSound = SnapNeckAudioAction.Apply(ref enemy.GetComponent<FlowermanAI>().crackNeckSFX);
            //entity.transform.Find("FlowermanModel/AnimContainer/PoofParticle").gameObject.SetActive(false);
            //entity.transform.Find("FlowermanModel/AnimContainer/ParticleCollision").gameObject.SetActive(false);
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            LeafMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeafMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 1, vanillaBodyMaterial);
            LeftEyeMaterialAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftEyeMaterial);
            RightEyeMaterialAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightEyeMaterial);
            BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            LeftEyeMeshAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftEyeMesh);
            RightEyeMeshAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightEyeMesh);
            AngerAudioAction.RemoveFromSource(enemy.GetComponent<FlowermanAI>().creatureAngerVoice, vanillaAngerSound);
            SnapNeckAudioAction.Remove(ref enemy.GetComponent<FlowermanAI>().crackNeckSFX, vanillaKillSound);
        }
    }
}