using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BaboonHawkSkinner : BaseSkinner, BaboonHawkEventHandler
    {
        protected const string LOD0_PATH = "BaboonBirdModel/BodyLOD0";
        protected const string LOD1_PATH = "BaboonBirdModel/BodyLOD1";
        protected const string LOD2_PATH = "BaboonBirdModel/BodyLOD2";
        protected const string ANCHOR_PATH = "BaboonBirdModel/AnimContainer/metarig";
        protected const string ANIM_EFFECTS_PATH = "BaboonBirdModel/AnimContainer";

        protected Material vanillaBodyMaterial;
        protected AudioClip[] vanillaScreamAudio;
        protected AudioClip[] vanillaLaughAudio;
        protected AudioClip vanillaIntimidateAudio;
        protected AudioClip vanillaIntimidateVoice;
        protected AudioClip vanillaEnterFightAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaStabAudio;
        protected AudioClip vanillaDeathAudio;
        protected AudioClip vanillaHitBodyAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioListAction ScreamAudioAction { get; }
        protected AudioListAction LaughAudioAction { get; }
        protected AudioAction IntimidateAction { get; }
        protected AudioAction IntimidateVoiceAction { get; }
        protected AudioAction EnterFightAction { get; }
        protected AudioAction KillPlayerAudioAction { get; }
        protected AudioAction StabAudioAction { get; }
        protected AudioAction DeathAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioListAction FootstepsAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;
        protected AudioSource modAggressionAudio;

        public BaboonHawkSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioListAction screamAction,
            AudioListAction laughAction,
            AudioAction intimidateAudioAction,
            AudioAction intimidateVoiceAction,
            AudioAction enterFightAction,
            AudioAction killPlayerAudioAction,
            AudioAction stabAudioAction,
            AudioAction deathAudioAction,
            AudioAction hitBodyAudioAction,
            AudioListAction footstepsAudioAction
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            ScreamAudioAction = screamAction;
            LaughAudioAction = laughAction;
            Attachments = attachments;
            IntimidateAction = intimidateAudioAction;
            IntimidateVoiceAction = intimidateVoiceAction;
            EnterFightAction = enterFightAction;
            KillPlayerAudioAction = killPlayerAudioAction;
            StabAudioAction = stabAudioAction;
            DeathAudioAction = deathAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            FootstepsAudioAction = footstepsAudioAction;
        }

        protected bool VoiceSilenced => IntimidateVoiceAction.actionType != AudioActionType.RETAIN || KillPlayerAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => StabAudioAction.actionType != AudioActionType.RETAIN || HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected bool AggressionSilenced => EnterFightAction.actionType != AudioActionType.RETAIN || IntimidateAction.actionType != AudioActionType.RETAIN;

        public override void Apply(GameObject enemy)
        {
            BaboonBirdAI bbhawk = enemy.GetComponent<BaboonBirdAI>();
            BaboonHawkAudioEvents animAudioEvents = enemy.transform.Find(ANIM_EFFECTS_PATH)?.gameObject?.GetComponent<BaboonHawkAudioEvents>();
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(bbhawk.creatureVoice, "modVoice");
                bbhawk.creatureVoice.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(bbhawk.creatureSFX, "modEffects");
                bbhawk.creatureSFX.mute = true;
                if(animAudioEvents!=null)
                {
                    animAudioEvents.audioToPlay = modCreatureEffects;
                }
            }
            if (AggressionSilenced)
            {
                modAggressionAudio = CreateModdedAudioSource(bbhawk.aggressionAudio, "modAggressionAudio");
                bbhawk.aggressionAudio.mute = true;
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaScreamAudio = ScreamAudioAction.Apply(ref bbhawk.cawScreamSFX);
            vanillaLaughAudio = LaughAudioAction.Apply(ref bbhawk.cawLaughSFX);
            if(animAudioEvents!=null)
            {
                vanillaFootstepsAudio = FootstepsAudioAction.Apply(ref animAudioEvents.randomClips);
            }

            vanillaIntimidateVoice = bbhawk.enemyType.audioClips[1];
            vanillaIntimidateAudio = bbhawk.enemyType.audioClips[2];
            vanillaEnterFightAudio = bbhawk.enemyType.audioClips[3];
            vanillaKillPlayerAudio = bbhawk.enemyType.audioClips[4];
            vanillaDeathAudio = bbhawk.enemyType.audioClips[5];

            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(bbhawk, this);
        }

        public override void Remove(GameObject enemy)
        {
            BaboonBirdAI bbhawk = enemy.GetComponent<BaboonBirdAI>();
            BaboonHawkAudioEvents animAudioEvents = enemy.transform.Find(ANIM_EFFECTS_PATH)?.gameObject?.GetComponent<BaboonHawkAudioEvents>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bbhawk, this);
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                bbhawk.creatureVoice.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                bbhawk.creatureSFX.mute = false;
                if(animAudioEvents!=null)
                {
                    animAudioEvents.audioToPlay = bbhawk.creatureSFX;
                }
            }
            if (AggressionSilenced)
            {
                DestroyModdedAudioSource(modAggressionAudio);
                bbhawk.aggressionAudio.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            ScreamAudioAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawScreamSFX, vanillaScreamAudio);
            LaughAudioAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawLaughSFX, vanillaLaughAudio);
            if (animAudioEvents != null)
            {
                FootstepsAudioAction.Remove(ref animAudioEvents.randomClips, vanillaFootstepsAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnEnterAttackMode(BaboonBirdAI baboonHawk)
        {
            if (AggressionSilenced)
            {
                modAggressionAudio.clip = EnterFightAction.WorkingClip(vanillaEnterFightAudio);
                modAggressionAudio.Play();
            }
        }

        public void OnIntimidate(BaboonBirdAI baboonHawk)
        {
            if (VoiceSilenced)
            {
                RoundManager.PlayRandomClip(modCreatureVoice, ScreamAudioAction.WorkingClips(vanillaScreamAudio), randomize: true, 1f, 1105);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, IntimidateVoiceAction.WorkingClip(vanillaIntimidateVoice));
            }
            if(AggressionSilenced)
            {
                modAggressionAudio.clip = IntimidateAction.WorkingClip(vanillaIntimidateAudio);
                modAggressionAudio.Play();
            }
        }

        public void OnAttackPlayer(BaboonBirdAI baboonHawk, GameNetcodeStuff.PlayerControllerB player)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(StabAudioAction.WorkingClip(vanillaStabAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, StabAudioAction.WorkingClip(vanillaStabAudio));
            }
        }

        public void OnAttackEnemy(BaboonBirdAI baboonHawk, EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(StabAudioAction.WorkingClip(vanillaStabAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, StabAudioAction.WorkingClip(vanillaStabAudio));
            }
        }

        public void OnKillPlayer(BaboonBirdAI baboonHawk, GameNetcodeStuff.PlayerControllerB player)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
            }
        }

        public void OnEnemyUpdate(EnemyAI enemy)
        {
            if (AggressionSilenced)
            {
                modAggressionAudio.volume = (enemy as BaboonBirdAI).aggressionAudio.volume;
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(DeathAudioAction.WorkingClip(vanillaDeathAudio));
            }
        }

        public void OnHit(EnemyAI enemy, PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(vanillaHitBodyAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(vanillaHitBodyAudio));
            }
        }
    }
}