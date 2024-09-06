using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class CircuitBeesSkinner : BaseSkinner, CircuitBeeEventHandler
    {
        public const string MESH_PROPERTY = "BugMesh";
        public const string TEXTURE_PROPERTY = "BugTexture";

        private Texture vanillaBeeTexture = null;
        private Mesh vanillaBeeMesh;
        private AudioClip vanillaIdleAudio;
        private AudioClip vanillaDefensiveAudio;
        private AudioClip vanillaAngryAudio;
        private AudioClip vanillaZapConstantAudio;

        protected bool EffectsSilenced => SkinData.LeaveAudioAction.actionType != AudioActionType.RETAIN;
        protected bool ZapSilenced => SkinData.ZapAudioListAction.actionType != AudioListActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modZapAudio;

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
            vanillaIdleAudio = SkinData.IdleAudioAction.ApplyToSource(bees.beesIdle);
            vanillaAngryAudio = SkinData.AngryAudioAction.ApplyToSource(bees.beesAngry);
            vanillaDefensiveAudio = SkinData.DefensiveAudioAction.ApplyToSource(bees.beesDefensive);
            vanillaZapConstantAudio = SkinData.ZapConstantAudioAction.ApplyToSource(bees.beeZapAudio);
            if(EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(bees.creatureSFX, "modEffects");
                bees.creatureSFX.mute = true;
            }
            if (ZapSilenced)
            {
                modZapAudio = CreateModdedAudioSource(bees.beeZapAudio, "modZapAudio");
                modZapAudio.clip = SkinData.ZapConstantAudioAction.WorkingClip(vanillaZapConstantAudio);
                bees.beeZapAudio.mute = true;
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(bees, this);
        }

        public override void Remove(GameObject enemy)
        {
            RedLocustBees bees = enemy.GetComponent<RedLocustBees>();
            EnemySkinRegistry.RegisterEnemyEventHandler(bees, this);
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                bees.creatureSFX.mute = false;
            }
            if (ZapSilenced)
            {
                DestroyModdedAudioSource(modZapAudio);
                bees.beeZapAudio.mute = false;
            }
            SkinData.BeeTextureAction.RemoveFromVisualEffect(bees.beeParticles, TEXTURE_PROPERTY, vanillaBeeTexture);
            SkinData.BeeMeshAction.RemoveFromVisualEffect(bees.beeParticles, vanillaBeeMesh, MESH_PROPERTY);
            SkinData.IdleAudioAction.RemoveFromSource(bees.beesIdle, vanillaIdleAudio);
            SkinData.AngryAudioAction.RemoveFromSource(bees.beesAngry, vanillaAngryAudio);
            SkinData.DefensiveAudioAction.RemoveFromSource(bees.beesDefensive, vanillaDefensiveAudio);
            SkinData.ZapConstantAudioAction.RemoveFromSource(bees.beeZapAudio, vanillaZapConstantAudio);
        }

        public void OnZapAudioCue(RedLocustBees bees)
        {
            if (ZapSilenced)
            {
                modZapAudio.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
                AudioClip[] zapClips = SkinData.ZapAudioListAction.WorkingClips(bees.enemyType.audioClips);
                AudioClip zapClip = zapClips[UnityEngine.Random.Range(0, zapClips.Length)];
                modZapAudio.PlayOneShot(zapClip, UnityEngine.Random.Range(0.6f, 1f));
            }
        }

        public void OnZapAudioStart(RedLocustBees bees)
        {
            if (ZapSilenced)
            {
                if (!modZapAudio.isPlaying)
                {
                    modZapAudio.Play();
                    modZapAudio.pitch = 1f;
                }
            }
        }

        public void OnZapAudioStop(RedLocustBees bees)
        {
            if (ZapSilenced)
            {
                modZapAudio.Stop();
            }
        }

        public void OnLeaveLevel(RedLocustBees bees)
        {
            modZapAudio.PlayOneShot(SkinData.LeaveAudioAction.WorkingClip(bees.enemyType.audioClips[0]), 0.5f);
        }
    }
}