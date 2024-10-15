using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class RoamingLocustSkinner : BaseSkinner, RoamingLocustEventHandler
    {
        public const string MESH_PROPERTY = "BugMesh";
        public const string TEXTURE_PROPERTY = "BugTexture";

        private Texture vanillaLocustTexture = null;
        private Mesh vanillaLocustMesh;
        private AudioClip vanillaChirpAudio;

        protected bool EffectsSilenced => SkinData.DisperseAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureEffects;

        protected RoamingLocustSkin SkinData { get; }

        public RoamingLocustSkinner(RoamingLocustSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            DocileLocustBeesAI locusts = enemy.GetComponent<DocileLocustBeesAI>();
            vanillaLocustTexture = SkinData.LocustTextureAction.ApplyToVisualEffect(locusts.bugsEffect, TEXTURE_PROPERTY);
            vanillaLocustMesh = SkinData.LocustMeshAction.ApplyToVisualEffect(locusts.bugsEffect, MESH_PROPERTY);
            vanillaChirpAudio = SkinData.ChirpAudioAction.ApplyToSource(enemy.transform.Find("ConstantSFX")?.gameObject.GetComponent<AudioSource>());

            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(locusts.creatureSFX, "modEffects");
                locusts.creatureSFX.mute = true;
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(locusts, this);
        }

        public override void Remove(GameObject enemy)
        {
            DocileLocustBeesAI locusts = enemy.GetComponent<DocileLocustBeesAI>();
            EnemySkinRegistry.RegisterEnemyEventHandler(locusts, this);
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                locusts.creatureSFX.mute = false;
            }
            SkinData.LocustTextureAction.RemoveFromVisualEffect(locusts.bugsEffect, TEXTURE_PROPERTY, vanillaLocustTexture);
            SkinData.LocustMeshAction.RemoveFromVisualEffect(locusts.bugsEffect, vanillaLocustMesh, MESH_PROPERTY);
            SkinData.ChirpAudioAction.RemoveFromSource(enemy.transform.Find("ConstantSFX")?.gameObject.GetComponent<AudioSource>(), vanillaChirpAudio);
        }

        public void OnDisperse(DocileLocustBeesAI locusts)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.DisperseAudioAction.WorkingClip(locusts.enemyType.audioClips[0]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.DisperseAudioAction.WorkingClip(locusts.enemyType.audioClips[0]), 0.8f);
            }
        }
    }
}