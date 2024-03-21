using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class SnareFleaSkinner : BaseSkinner
    {
        protected const string LOD0_PATH = "CentipedeModel/LOD1";
        protected const string LOD1_PATH = "CentipedeModel/LOD2";
        protected const string ANCHOR_PATH = "CentipedeModel/AnimContainer/Armature";

        protected Material vanillaBodyMaterial;
        protected AudioClip vanillaFallShriekAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaHitAudio;
        protected AudioClip[] vanillaShrieksAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioAction FallShriekAudioAction { get; }
        protected AudioAction HitGroundAudioAction { get; }
        protected AudioAction HitAudioAction { get; }
        protected AudioListAction ShrieksAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        

        public SnareFleaSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction fallShriekAudioAction,
            AudioAction hitGroundAudioAction,
            AudioAction hitAudioAction,
            AudioListAction shrieksAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            FallShriekAudioAction = fallShriekAudioAction;
            HitGroundAudioAction = hitGroundAudioAction;
            HitAudioAction = hitAudioAction;
            ShrieksAudioAction = shrieksAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaFallShriekAudio = FallShriekAudioAction.Apply(ref enemy.GetComponent<CentipedeAI>().fallShriek);
            vanillaHitGroundAudio = HitGroundAudioAction.Apply(ref enemy.GetComponent<CentipedeAI>().hitGroundSFX);
            vanillaHitAudio = HitAudioAction.Apply(ref enemy.GetComponent<CentipedeAI>().hitCentipede);
            vanillaShrieksAudio = ShrieksAudioAction.Apply(ref enemy.GetComponent<CentipedeAI>().shriekClips);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
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
            FallShriekAudioAction.Remove(ref enemy.GetComponent<CentipedeAI>().fallShriek, vanillaFallShriekAudio);
            HitGroundAudioAction.Remove(ref enemy.GetComponent<CentipedeAI>().hitGroundSFX, vanillaHitGroundAudio);
            HitAudioAction.Remove(ref enemy.GetComponent<CentipedeAI>().hitCentipede, vanillaHitAudio);
            ShrieksAudioAction.Remove(ref enemy.GetComponent<CentipedeAI>().shriekClips, vanillaShrieksAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }
}