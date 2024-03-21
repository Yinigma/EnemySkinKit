using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ThumperSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "CrawlerModel/Cube.002";
        protected const string ANCHOR_PATH = "CrawlerModel/AnimContainer/metarig";
        protected Material vanillaBodyMaterial;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioClip vanillaShortRoarAudio;
        protected AudioClip[] vanillaHitWallAudio;
        protected AudioClip vanillaBitePlayerAudio;
        protected AudioClip vanillaEatPlayerAudio;
        protected AudioClip[] vanillaHitAudio;
        protected AudioClip[] vanillaLongRoarAudio;
        protected List<GameObject> activeAttachments;


        protected AudioAction ShortRoarAudioAction { get; }
        protected AudioListAction HitWallAudioAction { get; }
        protected AudioAction BiteAudioAction { get; }
        protected AudioAction EatPlayerAudioAction { get; }
        protected AudioListAction HitAudioAction { get; }
        protected AudioListAction LongRoarAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public ThumperSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction shortRoarAudioAction,
            AudioListAction hitWallAudioAction,
            AudioAction bitePlayerAudioAction,
            AudioAction eatPlayerAudioAction,
            AudioListAction hitAudioAction,
            AudioListAction longRoarAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            ShortRoarAudioAction = shortRoarAudioAction;
            HitWallAudioAction = hitWallAudioAction;
            BiteAudioAction = bitePlayerAudioAction;
            EatPlayerAudioAction = eatPlayerAudioAction;
            HitAudioAction = hitAudioAction;
            LongRoarAudioAction = longRoarAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaShortRoarAudio = ShortRoarAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().shortRoar);
            vanillaHitWallAudio = HitWallAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().hitWallSFX);
            vanillaBitePlayerAudio = BiteAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().bitePlayerSFX);
            vanillaEatPlayerAudio = EatPlayerAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().eatPlayerSFX);
            vanillaHitAudio = HitAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().hitCrawlerSFX);
            vanillaLongRoarAudio = LongRoarAudioAction.Apply(ref enemy.GetComponent<CrawlerAI>().longRoarSFX);
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
            ShortRoarAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().shortRoar, vanillaShortRoarAudio);
            HitWallAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().hitWallSFX, vanillaHitWallAudio);
            BiteAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().bitePlayerSFX, vanillaBitePlayerAudio);
            EatPlayerAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().eatPlayerSFX, vanillaEatPlayerAudio);
            HitAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().hitCrawlerSFX, vanillaHitAudio);
            LongRoarAudioAction.Remove(ref enemy.GetComponent<CrawlerAI>().longRoarSFX, vanillaLongRoarAudio);
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