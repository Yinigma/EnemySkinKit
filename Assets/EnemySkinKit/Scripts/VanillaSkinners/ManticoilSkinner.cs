using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ManticoilSkinner : BaseSkinner, ManticoilEventHandler
    {
        protected const string LOD0_PATH = "DoublewingModel/BirdLOD0";
        protected const string LOD1_PATH = "DoublewingModel/BirdLOD1";
        protected const string ANCHOR_PATH = "DoublewingModel";

        protected VanillaMaterial vanillaBodyMaterial;
        protected List<GameObject> activeAttachments;

        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.FlapAudioListAction.actionType != AudioListActionType.RETAIN;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modFlapping;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected ManticoilSkin SkinData { get; }

        public ManticoilSkinner(ManticoilSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            DoublewingAI manticoil = enemy.GetComponent<DoublewingAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            SkinData.ScreechAudioListAction.ApplyToMap(manticoil.birdScreechSFX, clipMap);
            SkinData.HitGroundAudioAction.ApplyToMap(manticoil.birdHitGroundSFX, clipMap);
            SkinData.FlyingAudioAction.ApplyToMap(manticoil.flappingAudio.clip, clipMap);
            SkinData.StunAudioAction.ApplyToMap(manticoil.enemyType.stunSFX, clipMap);
            SkinData.FlapAudioListAction.ApplyToMap(manticoil.enemyType.audioClips, clipMap);

            modFlapping = CreateAudioReflector(manticoil.flappingAudio, clipMap, manticoil.NetworkObjectId); 
            manticoil.flappingAudio.mute = true;
            modCreatureVoice = CreateAudioReflector(manticoil.creatureVoice, clipMap, manticoil.NetworkObjectId); 
            manticoil.creatureVoice.mute = true;
            modCreatureEffects = CreateAudioReflector(manticoil.creatureSFX, clipMap, manticoil.NetworkObjectId); 
            manticoil.creatureSFX.mute = true;

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(manticoil, this);
        }

        public override void Remove(GameObject enemy)
        {
            DoublewingAI manticoil = enemy.GetComponent<DoublewingAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(manticoil, this);

            ArmatureAttachment.RemoveAttachments(activeAttachments);

            DestroyAudioReflector(modFlapping);
            manticoil.flappingAudio.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            manticoil.creatureVoice.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            manticoil.creatureSFX.mute = false;

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }
    }
}