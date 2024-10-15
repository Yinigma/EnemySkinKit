using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ThumperSkinner : BaseSkinner, ThumperEventHandler
    {
        protected const string BODY_PATH = "CrawlerModel/Cube.002";
        protected const string ANCHOR_PATH = "CrawlerModel/AnimContainer/metarig";
        protected const string ANIM_EVENT_PATH = "CrawlerModel/AnimContainer";
        protected VanillaMaterial vanillaBodyMaterial;

        protected AudioClip vanillaShortRoarAudio;
        protected AudioClip[] vanillaHitWallAudio;
        protected AudioClip vanillaBitePlayerAudio;
        protected AudioClip vanillaEatPlayerAudio;
        protected AudioClip[] vanillaHitAudio;
        protected AudioClip[] vanillaLongRoarAudio;
        protected AudioClip[] vanillaStompAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureVoice;

        protected ThumperSkin SkinData { get; }

        public ThumperSkinner(ThumperSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            CrawlerAI thumper = enemy.GetComponent<CrawlerAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaShortRoarAudio = SkinData.ShortRoarAudioAction.Apply(ref thumper.shortRoar);
            vanillaHitWallAudio = SkinData.WallHitsAudioListAction.Apply(ref thumper.hitWallSFX);
            vanillaBitePlayerAudio = SkinData.BiteAudioAction.Apply(ref thumper.bitePlayerSFX);
            vanillaEatPlayerAudio = SkinData.EatPlayerAudioAction.Apply(ref thumper.eatPlayerSFX);
            vanillaHitAudio = SkinData.HitsAudioAction.Apply(ref thumper.hitCrawlerSFX);
            vanillaLongRoarAudio = SkinData.LongRoarsAudioListAction.Apply(ref thumper.longRoarSFX);
            if(audioAnimEvents!=null)
            {
                vanillaStompAudio = SkinData.StompAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(thumper.creatureVoice, "modVoice");
                thumper.creatureVoice.mute = true;
            }
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
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
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.ShortRoarAudioAction.Remove(ref thumper.shortRoar, vanillaShortRoarAudio);
            SkinData.WallHitsAudioListAction.Remove(ref thumper.hitWallSFX, vanillaHitWallAudio);
            SkinData.BiteAudioAction.Remove(ref thumper.bitePlayerSFX, vanillaBitePlayerAudio);
            SkinData.EatPlayerAudioAction.Remove(ref thumper.eatPlayerSFX, vanillaEatPlayerAudio);
            SkinData.HitsAudioAction.Remove(ref thumper.hitCrawlerSFX, vanillaHitAudio);
            SkinData.LongRoarsAudioListAction.Remove(ref thumper.longRoarSFX, vanillaLongRoarAudio);
            if (audioAnimEvents != null)
            {
                SkinData.StompAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaStompAudio);
            }
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnScreech(CrawlerAI instance)
        {
            if (VoiceSilenced)
            {
                AudioClip[] longRoarClips = SkinData.LongRoarsAudioListAction.WorkingClips(vanillaLongRoarAudio);
                int num = Random.Range(0, longRoarClips.Length);
                modCreatureVoice.PlayOneShot(longRoarClips[num]);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, longRoarClips[num]);
            }
        }

        public void OnBitePlayer(CrawlerAI instance, GameNetcodeStuff.PlayerControllerB bittenPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.BiteAudioAction.WorkingClip(vanillaBitePlayerAudio));
            }
        }

        public void OnEatPlayer(CrawlerAI instance, DeadBodyInfo currentlyHeldBody)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.pitch = Random.Range(0.85f, 1.1f);
                modCreatureVoice.PlayOneShot(SkinData.EatPlayerAudioAction.WorkingClip(vanillaEatPlayerAudio));
            }

        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (VoiceSilenced)
            {
                AudioClip[] hitClips = SkinData.HitsAudioAction.WorkingClips(vanillaHitAudio);
                AudioClip hitSound = hitClips[Random.Range(0, hitClips.Length)];
                modCreatureVoice.PlayOneShot(hitSound);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, hitSound);
            }

        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }
    }

}