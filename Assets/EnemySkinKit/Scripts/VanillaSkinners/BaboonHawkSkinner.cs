using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BaboonHawkSkinner : BaseSkinner
    {
        protected const string LOD0_PATH = "BaboonBirdModel/BodyLOD0";
        protected const string LOD1_PATH = "BaboonBirdModel/BodyLOD1";
        protected const string LOD2_PATH = "BaboonBirdModel/BodyLOD2";
        protected const string ANCHOR_PATH = "BaboonBirdModel/AnimContainer/metarig";
        
        protected Material vanillaBodyMaterial;
        protected AudioClip[] vanillaScreamAudio;
        protected AudioClip[] vanillaLaughAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioListAction ScreamAudioAction { get; }
        protected AudioListAction LaughAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }



        public BaboonHawkSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioListAction screamAction,
            AudioListAction laughAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            ScreamAudioAction = screamAction;
            LaughAudioAction = laughAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaScreamAudio = ScreamAudioAction.Apply(ref enemy.GetComponent<BaboonBirdAI>().cawScreamSFX);
            vanillaLaughAudio = LaughAudioAction.Apply(ref enemy.GetComponent<BaboonBirdAI>().cawLaughSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            ScreamAudioAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawScreamSFX, vanillaScreamAudio);
            LaughAudioAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawLaughSFX, vanillaLaughAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }
}