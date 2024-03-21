using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class CoilHeadSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "SpringManModel/Body";
        protected const string HEAD_PATH = "SpringManModel/Head";
        protected const string ANCHOR_PATH = "SpringManModel/AnimContainer/metarig";

        protected Material vanillaBodyMaterial;
        protected Material vanillaRustMaterial;
        protected Material vanillaHeadMaterial;
        protected Mesh vanillaHeadMesh;
        protected AudioClip[] vanillaSpringAudio;
        protected List<GameObject> activeAttachments;

        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction HeadMeshAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction RustMaterialAction { get; }
        protected MaterialAction HeadMaterialAction { get; }

        protected AudioListAction SpringAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public CoilHeadSkinner(bool muteSoundEffects, bool muteVoice, ArmatureAttachment[] attachments, SkinnedMeshAction bodyMeshAction, StaticMeshAction headMeshAction, MaterialAction bodyMaterialAction, MaterialAction rustMaterialAction, MaterialAction headMaterialAction, AudioListAction springAudioAction) : base(muteSoundEffects, muteVoice)
        {
            
            BodyMeshAction = bodyMeshAction;
            HeadMeshAction = headMeshAction;
            BodyMaterialAction = bodyMaterialAction;
            RustMaterialAction = rustMaterialAction;
            HeadMaterialAction = headMaterialAction;
            SpringAudioAction = springAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRustMaterial = RustMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaHeadMaterial = HeadMaterialAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaSpringAudio = SpringAudioAction.Apply(ref enemy.GetComponent<SpringManAI>().springNoises);
            vanillaHeadMesh = HeadMeshAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>());
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            RustMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaRustMaterial);
            HeadMaterialAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaHeadMaterial);
            SpringAudioAction.Remove(ref enemy.GetComponent<SpringManAI>().springNoises, vanillaSpringAudio);
            BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            HeadMeshAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaHeadMesh);
        }
    }
}