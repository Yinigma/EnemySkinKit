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

        protected AudioClip vanillaAgitatedAudio;
        protected AudioClip vanillaJiggleAudio;
        protected AudioClip vanillaHitSlimeAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaIdleAudio;
        protected Color vanillaGradientColor;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected HygrodereSkin SkinData { get; }

        public HygrodereSkinner(HygrodereSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaGradientColor = SkinData.SlimeGradiantColorAction.Apply(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>().material, COLOR_PROPERTY);
            vanillaSlimeMaterial = SkinData.SlimeMaterialAction.Apply(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaAgitatedAudio = SkinData.AgitatedAudioAction.Apply(ref enemy.GetComponent<BlobAI>().agitatedSFX);
            vanillaJiggleAudio = SkinData.JiggleAudioAction.Apply(ref enemy.GetComponent<BlobAI>().jiggleSFX);
            vanillaHitSlimeAudio = SkinData.HitAudioAction.Apply(ref enemy.GetComponent<BlobAI>().hitSlimeSFX);
            vanillaKillPlayerAudio = SkinData.KillPlayerAudioAction.Apply(ref enemy.GetComponent<BlobAI>().killPlayerSFX);
            vanillaIdleAudio = SkinData.IdleAudioAction.Apply(ref enemy.GetComponent<BlobAI>().idleSFX);

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
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.SlimeMaterialAction.Remove(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSlimeMaterial);
            SkinData.SlimeGradiantColorAction.Remove(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>().material, COLOR_PROPERTY, vanillaGradientColor);
            SkinData.AgitatedAudioAction.Remove(ref enemy.GetComponent<BlobAI>().agitatedSFX, vanillaAgitatedAudio);
            SkinData.JiggleAudioAction.Remove(ref enemy.GetComponent<BlobAI>().jiggleSFX, vanillaJiggleAudio);
            SkinData.HitAudioAction.Remove(ref enemy.GetComponent<BlobAI>().hitSlimeSFX, vanillaHitSlimeAudio);
            SkinData.KillPlayerAudioAction.Remove(ref enemy.GetComponent<BlobAI>().killPlayerSFX, vanillaKillPlayerAudio);
            SkinData.IdleAudioAction.Remove(ref enemy.GetComponent<BlobAI>().idleSFX, vanillaIdleAudio);
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
