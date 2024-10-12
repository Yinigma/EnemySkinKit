using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class SnareFleaSkinner : BaseSkinner, SnareFleaEventHandler
    {
        protected const string LOD0_PATH = "CentipedeModel/LOD1";
        protected const string LOD1_PATH = "CentipedeModel/LOD2";
        protected const string ANCHOR_PATH = "CentipedeModel/AnimContainer/Armature";
        protected const string SILK_ROAD = "CentipedeModel/AnimContainer/Armature/Head/Body/Body.001/Body.002/Body.003/SilkParticle";

        protected VanillaMaterial vanillaBodyMaterial;
        protected ParticleSystem vanillaSilkParticle;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;
        protected VanillaMaterial vanillaSilkMaterial;
        protected ParticleSystem replacementSilkParticle;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modClingToPlayer;

        protected SnareFleaSkin SkinData { get; }

        public SnareFleaSkinner(SnareFleaSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            CentipedeAI flea = enemy.GetComponent<CentipedeAI>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            SkinData.ClingToCeilingAudioAction.ApplyToMap(flea.enemyBehaviourStates[1].SFXClip, clipMap);
            SkinData.CrawlAudioAction.ApplyToMap(flea.enemyBehaviourStates[2].SFXClip, clipMap);
            SkinData.FallShriekAudioAction.ApplyToMap(flea.enemyBehaviourStates[2].VoiceClip, clipMap);
            SkinData.HitGroundAudioAction.ApplyToMap(flea.hitGroundSFX, clipMap);
            SkinData.HitBody2AudioAction.ApplyToMap(flea.hitCentipede, clipMap);
            SkinData.ShrieksAudioListAction.ApplyToMap(flea.shriekClips, clipMap);
            SkinData.ClingToPlayerAudioAction.ApplyToMap(flea.clingToPlayer3D, clipMap);
            SkinData.ClingToLocalPlayerAudioAction.ApplyToMap(flea.clingingToPlayer2DAudio.clip, clipMap);
            SkinData.DeathAudioAction.ApplyToMap(flea.dieSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(flea.enemyType.hitBodySFX, clipMap);

            modCreatureEffects = CreateAudioReflector(flea.creatureSFX, clipMap, flea.NetworkObjectId); 
            flea.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(flea.creatureVoice, clipMap, flea.NetworkObjectId); 
            flea.creatureVoice.mute = true;
            modClingToPlayer = CreateAudioReflector(flea.clingingToPlayer2DAudio, clipMap, flea.NetworkObjectId); 
            flea.clingingToPlayer2DAudio.mute = true;

            vanillaSilkParticle = flea.transform.Find(SILK_ROAD).GetComponent<ParticleSystem>();

            vanillaSilkMaterial = SkinData.SilkMaterialAction.Apply(vanillaSilkParticle?.GetComponent<ParticleSystemRenderer>(), 0);
            replacementSilkParticle = SkinData.SilkParticleAction.Apply(vanillaSilkParticle);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(flea, this);
        }

        public override void Remove(GameObject enemy)
        {
            CentipedeAI flea = enemy.GetComponent<CentipedeAI>();
            DestroyAudioReflector(modCreatureEffects);
            flea.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            flea.creatureVoice.mute = false;
            DestroyAudioReflector(modClingToPlayer);
            flea.clingingToPlayer2DAudio.mute = false;

            if (vanillaSilkParticle != null)
            {
                SkinData.SilkParticleAction.Remove(vanillaSilkParticle, replacementSilkParticle);
                SkinData.SilkMaterialAction.Remove(vanillaSilkParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSilkMaterial);
            }

            EnemySkinRegistry.RemoveEnemyEventHandler(flea, this);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
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