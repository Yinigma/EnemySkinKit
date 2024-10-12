using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class HygrodereSkinner : BaseSkinner
    {
        protected const string SLIME_PATH = "Icosphere";
        protected const string ANCHOR_PATH = "Armature";
        protected const string COLOR_PROPERTY = "_Gradient_Color";

        protected VanillaMaterial vanillaSlimeMaterial; //mmm... vanilla slime...

        protected Color vanillaGradientColor;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        protected AudioReflector modMovableAudioSource;
        protected AudioReflector modCreatureEffects;

        protected HygrodereSkin SkinData { get; }

        public HygrodereSkinner(HygrodereSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            BlobAI slime = enemy.GetComponent<BlobAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaGradientColor = SkinData.SlimeGradiantColorAction.Apply(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>().material, COLOR_PROPERTY);
            vanillaSlimeMaterial = SkinData.SlimeMaterialAction.Apply(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.AgitatedAudioAction.ApplyToMap(enemy.GetComponent<BlobAI>().agitatedSFX, clipMap);
            SkinData.JiggleAudioAction.ApplyToMap(enemy.GetComponent<BlobAI>().jiggleSFX, clipMap);
            SkinData.HitAudioAction.ApplyToMap(enemy.GetComponent<BlobAI>().hitSlimeSFX, clipMap);
            SkinData.KillPlayerAudioAction.ApplyToMap(enemy.GetComponent<BlobAI>().killPlayerSFX, clipMap);
            SkinData.IdleAudioAction.ApplyToMap(enemy.GetComponent<BlobAI>().idleSFX, clipMap);

            modMovableAudioSource = CreateAudioReflector(slime.movableAudioSource, clipMap, slime.NetworkObjectId); 
            slime.movableAudioSource.mute = true;
            modCreatureEffects = CreateAudioReflector(slime.creatureSFX, clipMap, slime.NetworkObjectId); 
            slime.creatureSFX.mute = true;

            skinnedMeshReplacement = SkinData.SlimeMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public override void Remove(GameObject enemy)
        {
            BlobAI slime = enemy.GetComponent<BlobAI>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.SlimeMaterialAction.Remove(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSlimeMaterial);
            SkinData.SlimeGradiantColorAction.Remove(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>().material, COLOR_PROPERTY, vanillaGradientColor);

            DestroyAudioReflector(modMovableAudioSource);
            slime.movableAudioSource.mute = true;
            DestroyAudioReflector(modCreatureEffects);
            slime.creatureSFX.mute = true;

            SkinData.SlimeMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }
    }

}
