using AntlerShed.EnemySkinKit.AudioReflection;
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

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected AudioReflector modCreatureVoice;
        protected AudioReflector modCreatureEffects;

        protected ThumperSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        

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

            SkinData.ShortRoarAudioAction.ApplyToMap(thumper.shortRoar, clipMap);
            SkinData.WallHitsAudioListAction.ApplyToMap(thumper.hitWallSFX, clipMap);
            SkinData.BiteAudioAction.ApplyToMap(thumper.bitePlayerSFX, clipMap);
            SkinData.EatPlayerAudioAction.ApplyToMap(thumper.eatPlayerSFX, clipMap);
            SkinData.HitsAudioAction.ApplyToMap(thumper.hitCrawlerSFX, clipMap);
            SkinData.LongRoarsAudioListAction.ApplyToMap(thumper.longRoarSFX, clipMap);
            SkinData.StunAudioAction.ApplyToMap(thumper.enemyType.stunSFX, clipMap);
            if(audioAnimEvents!=null)
            {
                SkinData.StompAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }

            modCreatureEffects = CreateAudioReflector(thumper.creatureSFX, clipMap, thumper.NetworkObjectId); 
            thumper.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(thumper.creatureVoice, clipMap, thumper.NetworkObjectId); 
            thumper.creatureVoice.mute = true;

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
            EnemySkinRegistry.RemoveEnemyEventHandler(thumper, this);
            ArmatureAttachment.RemoveAttachments(activeAttachments);

            DestroyAudioReflector(modCreatureEffects);
            thumper.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            thumper.creatureVoice.mute = false;

            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }
    }

}