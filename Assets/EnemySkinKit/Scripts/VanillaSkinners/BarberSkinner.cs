using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BarberSkinner : BaseSkinner, BarberEventHandler
    {
        protected const string MESH_PATH = "MeshContainer/Mesh";
        protected const string LOWER_BLADE_PATH = "MeshContainer/AnimContainer/Scissors/LowerBlade";
        protected const string UPPER_BLADE_PATH = "MeshContainer/AnimContainer/Scissors/UpperBlade";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer/metarig";

        protected Mesh vanillaUpperBladeMesh;
        protected Mesh vanillaLowerBladeMesh;
        protected VanillaMaterial vanillaBarberMaterial;
        protected AudioClip vanillaSnipClip;
        protected AudioClip vanillaDrumrollClip;
        protected AudioClip[] vanillaParadeClips;
        protected Material[] replacementMaterials;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected BarberSkin SkinData { get; }

        public BarberSkinner(BarberSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            ClaySurgeonAI klayman = enemy.GetComponent<ClaySurgeonAI>();
            vanillaBarberMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(), 0);
            SkinData.UpperScissorsMaterialAction.Apply(enemy.transform.Find(UPPER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            SkinData.LowerScissorsMaterialAction.Apply(enemy.transform.Find(LOWER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaUpperBladeMesh = SkinData.UpperScissorsMeshAction.Apply(enemy.transform.Find(UPPER_BLADE_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaLowerBladeMesh = SkinData.LowerScissorsMeshAction.Apply(enemy.transform.Find(LOWER_BLADE_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaDrumrollClip = SkinData.DrumRoll.Apply(ref klayman.snareDrum);
            vanillaParadeClips = SkinData.MoveAudioListAction.Apply(ref klayman.paradeClips);
            vanillaSnipClip = SkinData.SnipAudioAction.Apply(ref klayman.snipScissors);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[] 
                { 
                    enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() 
                }, 
                enemy.transform.Find(ANCHOR_PATH)
            );
            List<Material> repMats = new List<Material>();
            if(skinnedMeshReplacement != null)
            {
                repMats.AddRange(skinnedMeshReplacement.GetComponentsInChildren<Renderer>().SelectMany((Renderer r) => r.materials));
            }
            if(SkinData.BodyMaterialAction.actionType == MaterialActionType.REPLACE)
            {
                repMats.Add(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>().material);
            }
            if (SkinData.UpperScissorsMaterialAction.actionType == MaterialActionType.REPLACE)
            {
                repMats.Add(enemy.transform.Find(UPPER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>().material);
            }
            if (SkinData.LowerScissorsMaterialAction.actionType == MaterialActionType.REPLACE)
            {
                repMats.Add(enemy.transform.Find(LOWER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>().material);
            }

            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            //LINQ got me doing that evil shit
            repMats.AddRange
            (
                activeAttachments.SelectMany
                (
                    (GameObject attachment) => attachment.GetComponentsInChildren<Renderer>().SelectMany
                    (
                        (Renderer rend) => rend.materials
                    )
                )
            );
            //God I hope you didn't put too many things in here. You didn't do that, right modder?
            replacementMaterials = repMats.ToArray();
            EnemySkinRegistry.RegisterEnemyEventHandler(klayman, this);
        }

        public override void Remove(GameObject enemy)
        {
            ClaySurgeonAI klayman = enemy.GetComponent<ClaySurgeonAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(klayman, this);
            //It's all the same ref for the barber so that the alpha fade only has to be set once
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(), 0, vanillaBarberMaterial);
            SkinData.UpperScissorsMaterialAction.Remove(enemy.transform.Find(UPPER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBarberMaterial);
            SkinData.LowerScissorsMaterialAction.Remove(enemy.transform.Find(LOWER_BLADE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBarberMaterial);

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.UpperScissorsMeshAction.Remove(enemy.transform.Find(UPPER_BLADE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaUpperBladeMesh);
            SkinData.LowerScissorsMeshAction.Remove(enemy.transform.Find(LOWER_BLADE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLowerBladeMesh);
            SkinData.DrumRoll.Remove(ref klayman.snareDrum, vanillaDrumrollClip);
            SkinData.MoveAudioListAction.Remove(ref klayman.paradeClips, vanillaParadeClips);
            SkinData.SnipAudioAction.Remove(ref klayman.snipScissors, vanillaSnipClip);
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }

        public void OnEnemyUpdate(EnemyAI enemy)
        {
            if(SkinData.DoFade)
            {
                float num = Vector3.Distance(StartOfRound.Instance.audioListener.transform.position, enemy.transform.position + Vector3.up * 0.7f);
                ClaySurgeonAI klayman = enemy as ClaySurgeonAI;
                foreach (Material mat in replacementMaterials)
                {
                    if (mat.HasFloat("_AlphaCutoff"))
                    {
                        mat.SetFloat("_AlphaCutoff", (num - klayman.minDistance) / (klayman.maxDistance - klayman.minDistance));
                    }
                }
            }
        }
    }
}