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

        protected Material vanillaBodyMaterial;
        protected AudioClip vanillaClingToCeilingAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaHitAudio;
        protected AudioClip vanillaClingAudio;
        protected AudioClip vanillaClingLocalAudio;
        protected AudioClip vanillaCrawlAudio;
        protected AudioClip vanillaFallShriekAudio;
        protected AudioClip vanillaDeathAudio;
        protected AudioClip[] vanillaShrieksAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioAction ClingToCeilingAudioAction { get; }
        protected AudioAction HitGroundAudioAction { get; }
        protected AudioAction ClingToPlayerAudioAction { get; }
        protected AudioAction ClingToLocalPlayerAudioAction { get; }
        protected AudioAction HitBody2AudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioListAction ShrieksAudioAction { get; }
        protected AudioAction CrawlAudioAction { get; }
        protected AudioAction FallShriekAudioAction { get; }
        protected AudioAction DeathAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

        public SnareFleaSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction clingToCeilingAudioAction,
            AudioAction crawlAudioAction,
            AudioAction fallShriekAudioAction,
            AudioAction hitGroundAudioAction,
            AudioListAction shrieksAudioAction,
            AudioAction clingToPlayerAudioAction,
            AudioAction clingToLocalAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction hitBody2AudioAction,
            AudioAction deathAudioAction
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;

            ClingToCeilingAudioAction = clingToCeilingAudioAction;
            HitGroundAudioAction = hitGroundAudioAction;
            ShrieksAudioAction = shrieksAudioAction;
            CrawlAudioAction = crawlAudioAction;
            FallShriekAudioAction = fallShriekAudioAction;
            ClingToPlayerAudioAction = clingToPlayerAudioAction;
            ClingToLocalPlayerAudioAction = clingToLocalAudioAction;
            HitBody2AudioAction = hitBody2AudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            DeathAudioAction = deathAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            CentipedeAI flea = enemy.GetComponent<CentipedeAI>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaClingToCeilingAudio = ClingToCeilingAudioAction.Apply(ref flea.enemyBehaviourStates[1].SFXClip);
            vanillaCrawlAudio = CrawlAudioAction.Apply(ref flea.enemyBehaviourStates[2].SFXClip);
            vanillaFallShriekAudio = FallShriekAudioAction.Apply(ref flea.enemyBehaviourStates[2].VoiceClip);
            vanillaHitGroundAudio = HitGroundAudioAction.Apply(ref flea.hitGroundSFX);
            vanillaHitAudio = HitBody2AudioAction.Apply(ref flea.hitCentipede);
            vanillaShrieksAudio = ShrieksAudioAction.Apply(ref flea.shriekClips);
            vanillaClingAudio = ClingToPlayerAudioAction.Apply(ref flea.clingToPlayer3D);
            vanillaClingLocalAudio = ClingToLocalPlayerAudioAction.ApplyToSource(flea.clingingToPlayer2DAudio);
            vanillaDeathAudio = DeathAudioAction.Apply(ref flea.dieSFX);

            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(flea.creatureSFX, "modEffects");
                flea.creatureSFX.mute = true;
            }
            BodyMeshAction.Apply
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
            EnemySkinRegistry.RemoveEnemyEventHandler(flea, this);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            ClingToCeilingAudioAction.Remove(ref flea.enemyBehaviourStates[1].SFXClip, vanillaClingToCeilingAudio);
            CrawlAudioAction.Remove(ref flea.enemyBehaviourStates[2].SFXClip, vanillaCrawlAudio);
            FallShriekAudioAction.Remove(ref flea.enemyBehaviourStates[2].VoiceClip, vanillaFallShriekAudio);
            HitGroundAudioAction.Remove(ref flea.hitGroundSFX, vanillaHitGroundAudio);
            HitBody2AudioAction.Remove(ref flea.hitCentipede, vanillaHitAudio);
            ShrieksAudioAction.Remove(ref flea.shriekClips, vanillaShrieksAudio);
            ClingToPlayerAudioAction.Remove(ref flea.clingToPlayer3D, vanillaClingAudio);
            DeathAudioAction.Remove(ref flea.dieSFX, vanillaDeathAudio);
            ClingToLocalPlayerAudioAction.RemoveFromSource(flea.clingingToPlayer2DAudio, vanillaClingLocalAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnClingToPlayer(CentipedeAI instance, GameNetcodeStuff.PlayerControllerB clingingToPlayer)
        {
            if(EffectsSilenced)
            { 
                if (!clingingToPlayer.IsLocalPlayer)
                {
                    modCreatureEffects.clip = ClingToPlayerAudioAction.WorkingClip(vanillaClingAudio);
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
                modCreatureEffects.PlayOneShot(HitGroundAudioAction.WorkingClip(vanillaHitGroundAudio));
            }
        }

        public void OnFallFromCeiling(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(CrawlAudioAction.WorkingClip(vanillaCrawlAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, CrawlAudioAction.WorkingClip(vanillaCrawlAudio));
            }
        }

        public void OnBeginAttackMovement(CentipedeAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] shriekClips = ShrieksAudioAction.WorkingClips(vanillaShrieksAudio);
                AudioClip shriekClip = shriekClips[Random.Range(0, shriekClips.Length)];
                modCreatureEffects.PlayOneShot(shriekClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, shriekClip);
            }
        }

        public void OnClingToCeiling(CentipedeAI instance)
        {
            modCreatureEffects.clip = ClingToCeilingAudioAction.WorkingClip(vanillaClingToCeilingAudio);
            modCreatureEffects.Play();
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(HitBody2AudioAction.WorkingClip(vanillaHitAudio));
                if (playSoundEffect)
                {
                    modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                }
            }
        }
    }
}