using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class RoamingLocustSkinner : BaseSkinner, RoamingLocustEventHandler
    {
        public const string MESH_PROPERTY = "BugMesh";
        public const string TEXTURE_PROPERTY = "BugTexture";

        private Texture vanillaLocustTexture = null;
        private Mesh vanillaLocustMesh;

        protected RoamingLocustSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        private AudioReflector modConstant;
        private AudioReflector modSwarmLeave;

        public RoamingLocustSkinner(RoamingLocustSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            DocileLocustBeesAI locusts = enemy.GetComponent<DocileLocustBeesAI>();

            vanillaLocustTexture = SkinData.LocustTextureAction.ApplyToVisualEffect(locusts.bugsEffect, TEXTURE_PROPERTY);
            vanillaLocustMesh = SkinData.LocustMeshAction.ApplyToVisualEffect(locusts.bugsEffect, MESH_PROPERTY);

            SkinData.DisperseAudioAction.ApplyToMap(locusts.enemyType.audioClips[0], clipMap);

            AudioSource constant = enemy.transform.Find("ConstantSFX")?.gameObject.GetComponent<AudioSource>();
            if(constant != null)
            {
                SkinData.ChirpAudioAction.ApplyToMap(constant.clip, clipMap);
                modConstant = CreateAudioReflector(constant, clipMap, locusts.NetworkObjectId); 
                constant.mute = true;
            }
            AudioSource swarmLeave = enemy.transform.Find("SwarmLeaveSFX")?.gameObject.GetComponent<AudioSource>();
            if (swarmLeave != null)
            {
                modSwarmLeave = CreateAudioReflector(swarmLeave, clipMap, locusts.NetworkObjectId);
                swarmLeave.mute = true;
            }


            EnemySkinRegistry.RegisterEnemyEventHandler(locusts, this);
        }

        public override void Remove(GameObject enemy)
        {
            DocileLocustBeesAI locusts = enemy.GetComponent<DocileLocustBeesAI>();
            EnemySkinRegistry.RegisterEnemyEventHandler(locusts, this);
            SkinData.LocustTextureAction.RemoveFromVisualEffect(locusts.bugsEffect, TEXTURE_PROPERTY, vanillaLocustTexture);
            SkinData.LocustMeshAction.RemoveFromVisualEffect(locusts.bugsEffect, vanillaLocustMesh, MESH_PROPERTY);

            AudioSource constant = enemy.transform.Find("ConstantSFX")?.gameObject.GetComponent<AudioSource>();
            if (constant != null)
            {
                DestroyAudioReflector(modConstant);
                constant.mute = false;
            }
            AudioSource swarmLeave = enemy.transform.Find("SwarmLeaveSFX")?.gameObject.GetComponent<AudioSource>();
            if (swarmLeave != null)
            {
                DestroyAudioReflector(modSwarmLeave);
                swarmLeave.mute = false;
            }


        }
    }
}