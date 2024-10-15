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
        protected AudioClip vanillaClingToCeilingAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaHitAudio;
        protected AudioClip vanillaClingAudio;
        protected AudioClip vanillaClingLocalAudio;
        protected AudioClip vanillaCrawlAudio;
        protected AudioClip vanillaFallShriekAudio;
        protected AudioClip vanillaDeathAudio;
        protected ParticleSystem vanillaSilkParticle;
        protected AudioClip[] vanillaShrieksAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;
        protected VanillaMaterial vanillaSilkMaterial;
        protected ParticleSystem replacementSilkParticle;

        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

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

            vanillaClingToCeilingAudio = SkinData.ClingToCeilingAudioAction.Apply(ref flea.enemyBehaviourStates[1].SFXClip);
            vanillaCrawlAudio = SkinData.CrawlAudioAction.Apply(ref flea.enemyBehaviourStates[2].SFXClip);
            vanillaFallShriekAudio = SkinData.FallShriekAudioAction.Apply(ref flea.enemyBehaviourStates[2].VoiceClip);
            vanillaHitGroundAudio = SkinData.HitGroundAudioAction.Apply(ref flea.hitGroundSFX);
            vanillaHitAudio = SkinData.HitBody2AudioAction.Apply(ref flea.hitCentipede);
            vanillaShrieksAudio = SkinData.ShrieksAudioListAction.Apply(ref flea.shriekClips);
            vanillaClingAudio = SkinData.ClingToPlayerAudioAction.Apply(ref flea.clingToPlayer3D);
            vanillaClingLocalAudio = SkinData.ClingToLocalPlayerAudioAction.ApplyToSource(flea.clingingToPlayer2DAudio);
            vanillaDeathAudio = SkinData.DeathAudioAction.Apply(ref flea.dieSFX);

            vanillaSilkParticle = flea.transform.Find(SILK_ROAD).GetComponent<ParticleSystem>();

            vanillaSilkMaterial = SkinData.SilkMaterialAction.Apply(vanillaSilkParticle?.GetComponent<ParticleSystemRenderer>(), 0);
            replacementSilkParticle = SkinData.SilkParticleAction.Apply(vanillaSilkParticle);

            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(flea.creatureSFX, "modEffects");
                flea.creatureSFX.mute = true;
            }
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
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                flea.creatureSFX.mute = false;
            }

            if(vanillaSilkParticle != null)
            {
                SkinData.SilkParticleAction.Remove(vanillaSilkParticle, replacementSilkParticle);
                SkinData.SilkMaterialAction.Remove(vanillaSilkParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSilkMaterial);
            }

            EnemySkinRegistry.RemoveEnemyEventHandler(flea, this);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.ClingToCeilingAudioAction.Remove(ref flea.enemyBehaviourStates[1].SFXClip, vanillaClingToCeilingAudio);
            SkinData.CrawlAudioAction.Remove(ref flea.enemyBehaviourStates[2].SFXClip, vanillaCrawlAudio);
            SkinData.FallShriekAudioAction.Remove(ref flea.enemyBehaviourStates[2].VoiceClip, vanillaFallShriekAudio);
            SkinData.HitGroundAudioAction.Remove(ref flea.hitGroundSFX, vanillaHitGroundAudio);
            SkinData.HitBody2AudioAction.Remove(ref flea.hitCentipede, vanillaHitAudio);
            SkinData.ShrieksAudioListAction.Remove(ref flea.shriekClips, vanillaShrieksAudio);
            SkinData.ClingToPlayerAudioAction.Remove(ref flea.clingToPlayer3D, vanillaClingAudio);
            SkinData.DeathAudioAction.Remove(ref flea.dieSFX, vanillaDeathAudio);
            SkinData.ClingToLocalPlayerAudioAction.RemoveFromSource(flea.clingingToPlayer2DAudio, vanillaClingLocalAudio);
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

        public void OnClingToPlayer(CentipedeAI instance, GameNetcodeStuff.PlayerControllerB clingingToPlayer)
        {
            if(EffectsSilenced)
            { 
                if (!clingingToPlayer.IsLocalPlayer)
                {
                    modCreatureEffects.clip = SkinData.ClingToPlayerAudioAction.WorkingClip(vanillaClingAudio);
                    modCreatureEffects.Play();
                }
            }
        }

        public void OnEnterMovingState(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
            }
        }

        public void OnHitGroundFromCeiling(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitGroundAudioAction.WorkingClip(vanillaHitGroundAudio));
            }
        }

        public void OnFallFromCeiling(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.CrawlAudioAction.WorkingClip(vanillaCrawlAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.CrawlAudioAction.WorkingClip(vanillaCrawlAudio));
            }
        }

        public void OnBeginAttackMovement(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] shriekClips = SkinData.ShrieksAudioListAction.WorkingClips(vanillaShrieksAudio);
                AudioClip shriekClip = shriekClips[Random.Range(0, shriekClips.Length)];
                modCreatureEffects.PlayOneShot(shriekClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, shriekClip);
            }
        }

        public void OnClingToCeiling(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.clip = SkinData.ClingToCeilingAudioAction.WorkingClip(vanillaClingToCeilingAudio);
                modCreatureEffects.Play();
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBody2AudioAction.WorkingClip(vanillaHitAudio));
                if (playSoundEffect)
                {
                    modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                }
            }
        }

        public void OnEnemyUpdate()
        {
            if(vanillaSilkParticle!= null && replacementSilkParticle!= null)
            {
                if (vanillaSilkParticle.gameObject.activeSelf != replacementSilkParticle.isPlaying)
                {
                    if(vanillaSilkParticle.gameObject.activeSelf)
                    {
                        replacementSilkParticle.Play();
                    }
                    else
                    {
                        replacementSilkParticle.Stop();
                    }
                }
            }
            
        }
    }
}