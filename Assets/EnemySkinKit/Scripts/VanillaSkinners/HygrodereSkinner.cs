using AntlerShed.EnemySkinKit.SkinAction;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class HygrodereSkinner : BaseSkinner
    {
        protected const string SLIME_PATH = "Icosphere";
        protected const string ANCHOR_PATH = "Armature";

        protected Material vanillaSlimeMaterial; //mmm... vanilla slime...

        protected AudioClip vanillaAgitatedAudio;
        protected AudioClip vanillaJiggleAudio;
        protected AudioClip vanillaHitSlimeAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaIdleAudio;
        protected List<GameObject> activeAttachments;

        protected AudioAction AgitatedAudioAction { get; }
        protected AudioAction JiggleAudioAction { get; }
        protected AudioAction HitSlimeAudioAction { get; }
        protected AudioAction KillPlayerAudioAction { get; }
        protected AudioAction IdleAudioAction { get; }
        protected MaterialAction SlimeMaterialAction { get; }
        protected SkinnedMeshAction SlimeMeshAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public HygrodereSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction slimeMaterialAction, 
            SkinnedMeshAction slimeMeshAction,
            AudioAction agitatedAudioAction,
            AudioAction jiggleAudioAction,
            AudioAction hitSlimeAudioAction,
            AudioAction killPlayerAudioAction,
            AudioAction idleAudioAction
        )
        {
            SlimeMaterialAction = slimeMaterialAction;
            SlimeMeshAction = slimeMeshAction;
            AgitatedAudioAction = agitatedAudioAction;
            JiggleAudioAction = jiggleAudioAction;
            HitSlimeAudioAction = hitSlimeAudioAction;
            KillPlayerAudioAction = killPlayerAudioAction;
            IdleAudioAction = idleAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaSlimeMaterial = SlimeMaterialAction.Apply(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaAgitatedAudio = AgitatedAudioAction.Apply(ref enemy.GetComponent<BlobAI>().agitatedSFX);
            vanillaJiggleAudio = JiggleAudioAction.Apply(ref enemy.GetComponent<BlobAI>().jiggleSFX);
            vanillaHitSlimeAudio = HitSlimeAudioAction.Apply(ref enemy.GetComponent<BlobAI>().hitSlimeSFX);
            vanillaKillPlayerAudio = KillPlayerAudioAction.Apply(ref enemy.GetComponent<BlobAI>().killPlayerSFX);
            vanillaIdleAudio = IdleAudioAction.Apply(ref enemy.GetComponent<BlobAI>().idleSFX);

            SlimeMeshAction.Apply
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
            SlimeMaterialAction.Remove(enemy.transform.Find(SLIME_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSlimeMaterial);
            AgitatedAudioAction.Remove(ref enemy.GetComponent<BlobAI>().agitatedSFX, vanillaAgitatedAudio);
            JiggleAudioAction.Remove(ref enemy.GetComponent<BlobAI>().jiggleSFX, vanillaJiggleAudio);
            HitSlimeAudioAction.Remove(ref enemy.GetComponent<BlobAI>().hitSlimeSFX, vanillaHitSlimeAudio);
            KillPlayerAudioAction.Remove(ref enemy.GetComponent<BlobAI>().killPlayerSFX, vanillaKillPlayerAudio);
            IdleAudioAction.Remove(ref enemy.GetComponent<BlobAI>().idleSFX, vanillaIdleAudio);
            SlimeMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(SLIME_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }

}
