using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class OldBirdNestSkinner : Skinner
    {
        protected const string MECH_PATH = "MeshContainer/Body";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer";

        protected VanillaMaterial vanillaDefaultMaterial;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;


        protected OldBirdSkin SkinData { get; }

        public OldBirdNestSkinner(OldBirdSkin skinData)
        {
            SkinData = skinData;
        }

        public void Apply(GameObject enemy)
        {
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.NestAttachments, enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaDefaultMaterial = SkinData.NestBodyMaterialAction.Apply(enemy?.transform?.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(), 0);
            SkinData.NestBodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void Remove(GameObject enemy)
        {
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.NestBodyMaterialAction.Remove(enemy?.transform.Find(MECH_PATH)?.GetComponent<SkinnedMeshRenderer>(), 0, vanillaDefaultMaterial);
            SkinData.NestBodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }
    }
}