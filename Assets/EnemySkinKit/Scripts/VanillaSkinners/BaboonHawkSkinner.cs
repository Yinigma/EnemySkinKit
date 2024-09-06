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
        protected const string SECONDARY_BLOOD_PARTICLE_PATH = "BaboonBirdModel/AnimContainer/metarig/spine/spine.001/spine.003/spine.004/HeadBoneContainer/spine.005/BloodSpurtParticle (1)/BloodParticle";

        protected VanillaMaterial vanillaBodyMaterial;
        protected AudioClip[] vanillaScreamAudio;
        protected AudioClip[] vanillaLaughAudio;
        protected AudioClip vanillaIntimidateAudio;
        protected AudioClip vanillaIntimidateVoice;
        protected AudioClip vanillaEnterFightAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaStabAudio;
        protected AudioClip vanillaDeathAudio;
        protected VanillaMaterial vanillaBloodMat;
        private VanillaMaterial vanillaSecondaryBloodMat;
        private VanillaMaterial vanillaBloodStabMat;
        protected AudioClip vanillaHitBodyAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected List<GameObject> activeAttachments;
        protected ParticleSystem vanillaBloodParticle;
        protected GameObject skinnedMeshReplacement;

        protected BaboonHawkSkin SkinData { get; }

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;
        protected AudioSource modAggressionAudio;

        public BaboonHawkSkinner
        (
            BaboonHawkSkin skinData
        )
        {
            SkinData = skinData;
        }

        protected bool VoiceSilenced => SkinData.IntimidateVoiceAction.actionType != AudioActionType.RETAIN || SkinData.KillPlayerAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.StabAudioAction.actionType != AudioActionType.RETAIN || SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected bool AggressionSilenced => SkinData.EnterFightAction.actionType != AudioActionType.RETAIN || SkinData.IntimidateAudioAction.actionType != AudioActionType.RETAIN;

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
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaScreamAudio = SkinData.ScreamAudioListAction.Apply(ref bbhawk.cawScreamSFX);
            vanillaLaughAudio = SkinData.LaughAudioListAction.Apply(ref bbhawk.cawLaughSFX);
            if(animAudioEvents!=null)
            {
                vanillaFootstepsAudio = SkinData.FootstepsAudioAction.Apply(ref animAudioEvents.randomClips);
            }

            vanillaIntimidateVoice = bbhawk.enemyType.audioClips[1];
            vanillaIntimidateAudio = bbhawk.enemyType.audioClips[2];
            vanillaEnterFightAudio = bbhawk.enemyType.audioClips[3];
            vanillaKillPlayerAudio = bbhawk.enemyType.audioClips[4];
            vanillaDeathAudio = bbhawk.enemyType.audioClips[5];

            ParticleSystem secBloodParticle = bbhawk.transform.Find(SECONDARY_BLOOD_PARTICLE_PATH).GetComponent<ParticleSystem>();

            vanillaBloodMat = SkinData.BloodMaterialAction.Apply(animAudioEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSecondaryBloodMat = SkinData.BloodMaterialAction.Apply(secBloodParticle?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBloodStabMat = SkinData.BloodMaterialAction.Apply(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0);

            vanillaBloodParticle = SkinData.BloodParticleAction.ApplyRef(ref animAudioEvents.particle);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
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
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.ScreamAudioListAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawScreamSFX, vanillaScreamAudio);
            SkinData.LaughAudioListAction.Remove(ref enemy.GetComponent<BaboonBirdAI>().cawLaughSFX, vanillaLaughAudio);

            SkinData.BloodParticleAction.RemoveRef(ref animAudioEvents.particle, vanillaBloodParticle);

            ParticleSystem secBloodParticle = bbhawk.transform.Find(SECONDARY_BLOOD_PARTICLE_PATH).GetComponent<ParticleSystem>();

            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodMat);
            SkinData.BloodMaterialAction.Remove(secBloodParticle?.GetComponent<ParticleSystemRenderer>(), 0, vanillaSecondaryBloodMat);
            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0, vanillaBloodStabMat);

            vanillaBloodParticle = SkinData.BloodParticleAction.ApplyRef(ref animAudioEvents.particle);

            SkinData.BloodMaterialAction.Remove(animAudioEvents.particle.subEmitters.GetSubEmitterSystem(0).GetComponent<Renderer>(), 0, vanillaBloodStabMat);

            if (animAudioEvents != null)
            {
                SkinData.FootstepsAudioAction.Remove(ref animAudioEvents.randomClips, vanillaFootstepsAudio);
            }
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnEnterAttackMode(BaboonBirdAI baboonHawk)
        {
            if (AggressionSilenced)
            {
                modAggressionAudio.clip = SkinData.EnterFightAction.WorkingClip(vanillaEnterFightAudio);
                modAggressionAudio.Play();
            }
        }

        public void OnIntimidate(BaboonBirdAI baboonHawk)
        {
            if (VoiceSilenced)
            {
                RoundManager.PlayRandomClip(modCreatureVoice, SkinData.ScreamAudioListAction.WorkingClips(vanillaScreamAudio), randomize: true, 1f, 1105);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.IntimidateVoiceAction.WorkingClip(vanillaIntimidateVoice));
            }
            if(AggressionSilenced)
            {
                modAggressionAudio.clip = SkinData.IntimidateAudioAction.WorkingClip(vanillaIntimidateAudio);
                modAggressionAudio.Play();
            }
        }

        public void OnAttackPlayer(BaboonBirdAI baboonHawk, PlayerControllerB player)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.StabAudioAction.WorkingClip(vanillaStabAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.StabAudioAction.WorkingClip(vanillaStabAudio));
            }
        }

        public void OnAttackEnemy(BaboonBirdAI baboonHawk, EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.StabAudioAction.WorkingClip(vanillaStabAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.StabAudioAction.WorkingClip(vanillaStabAudio));
            }
        }

        public void OnKillPlayer(BaboonBirdAI baboonHawk, PlayerControllerB player)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
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
                modCreatureVoice.PlayOneShot(SkinData.DeathAudioAction.WorkingClip(vanillaDeathAudio));
            }
        }

        public void OnHit(EnemyAI enemy, PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(vanillaHitBodyAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(vanillaHitBodyAudio));
            }
        }
    }
}