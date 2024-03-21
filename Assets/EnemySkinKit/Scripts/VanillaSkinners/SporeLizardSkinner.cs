using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class SporeLizardSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "PufferModel/BezierCurve";
        protected const string ANCHOR_PATH = "PufferModel/AnimContainer/Armature";
        protected Material vanillaBodyMaterial;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioClip[] vanillaFrightenedAudio;
        protected AudioClip vanillaStompAudio;
        protected AudioClip vanillaAngryAudio;
        protected AudioClip vanillaPuffAudio;
        protected AudioClip vanillaNervousAudio;
        protected AudioClip vanillaRattleAudio;
        protected AudioClip vanillaBiteAudio;
        protected List<GameObject> activeAttachments;

        protected AudioListAction FrightenedAudioAction { get; }
        protected AudioAction StompAudioAction { get; }
        protected AudioAction AngryAudioAction { get; }
        protected AudioAction PuffAudioAction { get; }
        protected AudioAction NervousAudioAction { get; }
        protected AudioAction RattleAudioAction { get; }
        protected AudioAction BiteAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }


        public SporeLizardSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioListAction frightenedAudioAction,
            AudioAction stompAudioAction,
            AudioAction angryAudioAction,
            AudioAction puffAudioAction,
            AudioAction nervousAudioAction,
            AudioAction rattleAudioAction,
            AudioAction biteAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            FrightenedAudioAction = frightenedAudioAction;
            StompAudioAction = stompAudioAction;
            AngryAudioAction = angryAudioAction;
            PuffAudioAction = puffAudioAction;
            NervousAudioAction = nervousAudioAction;
            RattleAudioAction = rattleAudioAction;
            BiteAudioAction = biteAudioAction;
            Attachments = attachments;
        }

        
        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaFrightenedAudio = FrightenedAudioAction.Apply(ref enemy.GetComponent<PufferAI>().frightenSFX);
            vanillaStompAudio = StompAudioAction.Apply(ref enemy.GetComponent<PufferAI>().stomp);
            vanillaAngryAudio = AngryAudioAction.Apply(ref enemy.GetComponent<PufferAI>().angry);
            vanillaPuffAudio = PuffAudioAction.Apply(ref enemy.GetComponent<PufferAI>().puff);
            vanillaNervousAudio = NervousAudioAction.Apply(ref enemy.GetComponent<PufferAI>().nervousMumbling);
            vanillaRattleAudio = RattleAudioAction.Apply(ref enemy.GetComponent<PufferAI>().rattleTail);
            vanillaBiteAudio = BiteAudioAction.Apply(ref enemy.GetComponent<PufferAI>().bitePlayerSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            FrightenedAudioAction.Remove(ref enemy.GetComponent<PufferAI>().frightenSFX, vanillaFrightenedAudio);
            StompAudioAction.Remove(ref enemy.GetComponent<PufferAI>().stomp, vanillaStompAudio);
            AngryAudioAction.Remove(ref enemy.GetComponent<PufferAI>().angry, vanillaAngryAudio);
            PuffAudioAction.Remove(ref enemy.GetComponent<PufferAI>().puff, vanillaPuffAudio);
            NervousAudioAction.Remove(ref enemy.GetComponent<PufferAI>().nervousMumbling, vanillaNervousAudio);
            RattleAudioAction.Remove(ref enemy.GetComponent<PufferAI>().rattleTail, vanillaRattleAudio);
            BiteAudioAction.Remove(ref enemy.GetComponent<PufferAI>().bitePlayerSFX, vanillaBiteAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }

}