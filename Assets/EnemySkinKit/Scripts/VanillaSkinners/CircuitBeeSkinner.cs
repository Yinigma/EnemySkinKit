using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class CircuitBeesSkinner : BaseSkinner, CircuitBeeEventHandler
    {
        public const string MESH_PROPERTY = "BugMesh";
        public const string TEXTURE_PROPERTY = "BugTexture";

        private Texture vanillaBeeTexture = null;
        private Mesh vanillaBeeMesh;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected AudioReflector modAngry;
        protected AudioReflector modCreatureEffects;
        protected AudioReflector modDefensive;
        protected AudioReflector modIdle;
        protected AudioReflector modZap;

        protected CircuitBeesSkin SkinData {get;}

        public CircuitBeesSkinner(CircuitBeesSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            RedLocustBees bees = enemy.GetComponent<RedLocustBees>();
            vanillaBeeTexture = SkinData.BeeTextureAction.ApplyToVisualEffect(bees.beeParticles, TEXTURE_PROPERTY);
            vanillaBeeMesh = SkinData.BeeMeshAction.ApplyToVisualEffect(bees.beeParticles, MESH_PROPERTY);

            SkinData.IdleAudioAction.ApplyToMap(bees.beesIdle.clip, clipMap);
            SkinData.AngryAudioAction.ApplyToMap(bees.beesAngry.clip, clipMap);
            SkinData.DefensiveAudioAction.ApplyToMap(bees.beesDefensive.clip, clipMap);
            SkinData.ZapConstantAudioAction.ApplyToMap(bees.beeZapAudio.clip, clipMap);
            SkinData.LeaveAudioAction.ApplyToMap(bees.enemyType.audioClips[0], clipMap);

            modCreatureEffects = CreateAudioReflector(bees.creatureSFX, clipMap, bees.NetworkObjectId);
            bees.creatureSFX.mute = true;
            modZap = CreateAudioReflector(bees.beeZapAudio, clipMap, bees.NetworkObjectId);
            bees.beeZapAudio.mute = true;
            modAngry = CreateAudioReflector(bees.beesAngry, clipMap, bees.NetworkObjectId);
            bees.beesAngry.mute = true;
            modDefensive = CreateAudioReflector(bees.beesDefensive, clipMap, bees.NetworkObjectId);
            bees.beesDefensive.mute = true;
            modIdle = CreateAudioReflector(bees.beesIdle, clipMap, bees.NetworkObjectId);
            bees.beesIdle.mute = true;

            EnemySkinRegistry.RegisterEnemyEventHandler(bees, this);
        }

        public override void Remove(GameObject enemy)
        {
            RedLocustBees bees = enemy.GetComponent<RedLocustBees>();
            EnemySkinRegistry.RegisterEnemyEventHandler(bees, this);

            DestroyAudioReflector(modCreatureEffects);
            bees.creatureSFX.mute = false;
            DestroyAudioReflector(modZap);
            bees.beeZapAudio.mute = false;
            DestroyAudioReflector(modAngry);
            bees.beesAngry.mute = false;
            DestroyAudioReflector(modDefensive);
            bees.beesDefensive.mute = false;
            DestroyAudioReflector(modIdle);
            bees.beesIdle.mute = false;

            SkinData.BeeTextureAction.RemoveFromVisualEffect(bees.beeParticles, TEXTURE_PROPERTY, vanillaBeeTexture);
            SkinData.BeeMeshAction.RemoveFromVisualEffect(bees.beeParticles, vanillaBeeMesh, MESH_PROPERTY);
        }
    }
}