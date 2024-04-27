using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ThumperSkinner : BaseSkinner, ThumperEventHandler
    {
        protected const string BODY_PATH = "CrawlerModel/Cube.002";
        protected const string ANCHOR_PATH = "CrawlerModel/AnimContainer/metarig";
        protected const string ANIM_EVENT_PATH = "CrawlerModel/AnimContainer";
        protected Material vanillaBodyMaterial;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioClip vanillaShortRoarAudio;
        protected AudioClip[] vanillaHitWallAudio;
        protected AudioClip vanillaBitePlayerAudio;
        protected AudioClip vanillaEatPlayerAudio;
        protected AudioClip[] vanillaHitAudio;
        protected AudioClip[] vanillaLongRoarAudio;
        protected AudioClip[] vanillaStompAudio;
        protected List<GameObject> activeAttachments;

        protected bool VoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureVoice;

        protected AudioAction ShortRoarAudioAction { get; }
        protected AudioListAction HitWallAudioAction { get; }
        protected AudioListAction StompAudioAction { get; }
        protected AudioAction BiteAudioAction { get; }
        protected AudioAction EatPlayerAudioAction { get; }
        protected AudioListAction HitAudioAction { get; }
        protected AudioAction StunAudioAction { get; }
        protected AudioListAction LongRoarAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public ThumperSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction shortRoarAudioAction,
            AudioListAction hitWallAudioAction,
            AudioAction bitePlayerAudioAction,
            AudioAction eatPlayerAudioAction,
            AudioListAction hitAudioAction,
            AudioListAction longRoarAudioAction,
            AudioAction stunAudioAction,
            AudioListAction stompAudioListAction
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            ShortRoarAudioAction = shortRoarAudioAction;
            HitWallAudioAction = hitWallAudioAction;
            BiteAudioAction = bitePlayerAudioAction;
            EatPlayerAudioAction = eatPlayerAudioAction;
            HitAudioAction = hitAudioAction;
            LongRoarAudioAction = longRoarAudioAction;
            StompAudioAction = stompAudioListAction;
            StunAudioAction = stunAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            CrawlerAI thumper = enemy.GetComponent<CrawlerAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaShortRoarAudio = ShortRoarAudioAction.Apply(ref thumper.shortRoar);
            vanillaHitWallAudio = HitWallAudioAction.Apply(ref thumper.hitWallSFX);
            vanillaBitePlayerAudio = BiteAudioAction.Apply(ref thumper.bitePlayerSFX);
            vanillaEatPlayerAudio = EatPlayerAudioAction.Apply(ref thumper.eatPlayerSFX);
            vanillaHitAudio = HitAudioAction.Apply(ref thumper.hitCrawlerSFX);
            vanillaLongRoarAudio = LongRoarAudioAction.Apply(ref thumper.longRoarSFX);
            if(audioAnimEvents!=null)
            {
                vanillaStompAudio = StompAudioAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(thumper.creatureVoice, "modVoice");
                thumper.creatureVoice.mute = true;
            }
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(thumper, this);
        }

        public override void Remove(GameObject enemy)
        {
            CrawlerAI thumper = enemy.GetComponent<CrawlerAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(thumper, this);
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                thumper.creatureVoice.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            ShortRoarAudioAction.Remove(ref thumper.shortRoar, vanillaShortRoarAudio);
            HitWallAudioAction.Remove(ref thumper.hitWallSFX, vanillaHitWallAudio);
            BiteAudioAction.Remove(ref thumper.bitePlayerSFX, vanillaBitePlayerAudio);
            EatPlayerAudioAction.Remove(ref thumper.eatPlayerSFX, vanillaEatPlayerAudio);
            HitAudioAction.Remove(ref thumper.hitCrawlerSFX, vanillaHitAudio);
            LongRoarAudioAction.Remove(ref thumper.longRoarSFX, vanillaLongRoarAudio);
            if (audioAnimEvents != null)
            {
                StompAudioAction.Remove(ref audioAnimEvents.randomClips, vanillaStompAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnScreech(CrawlerAI instance)
        {
            if (VoiceSilenced)
            {
                AudioClip[] longRoarClips = LongRoarAudioAction.WorkingClips(vanillaLongRoarAudio);
                int num = Random.Range(0, longRoarClips.Length);
                modCreatureVoice.PlayOneShot(longRoarClips[num]);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, longRoarClips[num]);
            }
        }

        public void OnBitePlayer(CrawlerAI instance, GameNetcodeStuff.PlayerControllerB bittenPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(BiteAudioAction.WorkingClip(vanillaBitePlayerAudio));
            }
        }

        public void OnEatPlayer(CrawlerAI instance, DeadBodyInfo currentlyHeldBody)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.pitch = Random.Range(0.85f, 1.1f);
                modCreatureVoice.PlayOneShot(EatPlayerAudioAction.WorkingClip(vanillaEatPlayerAudio));
            }

        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (VoiceSilenced)
            {
                AudioClip[] hitClips = HitAudioAction.WorkingClips(vanillaHitAudio);
                AudioClip hitSound = hitClips[Random.Range(0, hitClips.Length)];
                modCreatureVoice.PlayOneShot(hitSound);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, hitSound);
            }

        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }
    }

}