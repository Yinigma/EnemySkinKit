using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ForestKeeperSkinner : BaseSkinner
    {
        protected const string LOD0_PATH = "FGiantModelContainer/BodyLOD0";
        protected const string LOD1_PATH = "FGiantModelContainer/BodyLOD1";
        protected const string LOD2_PATH = "FGiantModelContainer/BodyLOD2";
        protected const string ANCHOR_PATH = "FGiantModelContainer/AnimContainer/metarig";

        protected Material vanillaBodyMaterial;
        protected AudioClip vanillaFarWideAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioAction FarWideAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        
        public ForestKeeperSkinner
        (
            bool muteSoundEffects,
            bool muteVoice, 
            ArmatureAttachment[] attachments, 
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction, 
            AudioAction farWideAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            FarWideAudioAction = farWideAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaFarWideAudio = FarWideAudioAction.ApplyToSource(enemy.GetComponent<ForestGiantAI>().farWideSFX);
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
            FarWideAudioAction.RemoveFromSource(enemy.GetComponent<ForestGiantAI>().farWideSFX, vanillaFarWideAudio);
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
