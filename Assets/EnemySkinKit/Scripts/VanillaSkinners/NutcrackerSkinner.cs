using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class NutcrackerSkinner : BaseSkinner, NutcrackerEventHandler
    {
        protected const string LOD0_PATH = "MeshContainer/LOD0";
        protected const string LOD1_PATH = "MeshContainer/LOD1";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer/metarig";
        protected const string ANIM_EVENTS_PATH = "MeshContainer/AnimContainer";
        protected const string BLOOD_FOUNTAIN_PARTICLE = "MeshContainer/AnimContainer/metarig/spinecontainer/spine/spine.001/BloodSpurtParticle (1)/BloodSpurtParticle";
        protected VanillaMaterial vanillaBodyMaterial;
        protected AudioClip[] vanillaTorsoTurnFinishAudio;
        protected AudioClip[] vanillaJointSqueakAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaAimAudio;
        protected AudioClip vanillaKickAudio;
        protected AudioClip vanillaAngryAudio;
        protected AudioClip vanillaTorsoTurnAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool LongRangeSilenced => SkinData.HeadPopUpAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN || SkinData.HitEyeAudioAction.actionType != AudioActionType.RETAIN || SkinData.ReloadAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modLongRange;
        protected VanillaMaterial vanillaSpurtMat;
        protected VanillaMaterial vanillaFountainMat;
        protected ParticleSystem vanillaSpurtParticle;
        protected ParticleSystem replacementFountainParticle;

        protected NutcrackerSkin SkinData { get; }

        public NutcrackerSkinner( NutcrackerSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            NutcrackerEnemyAI nutcracker = enemy.GetComponent<NutcrackerEnemyAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTorsoTurnAudio = SkinData.TorsoTurnAudioAction.ApplyToSource(nutcracker.torsoTurnAudio);
            vanillaTorsoTurnFinishAudio = SkinData.TorsoFinishTurningAudioListAction.Apply(ref nutcracker.torsoFinishTurningClips);
            vanillaAimAudio = SkinData.AimAudioAction.Apply(ref nutcracker.aimSFX);
            vanillaKickAudio = SkinData.KickAudioAction.Apply(ref nutcracker.kickSFX);
            vanillaAngryAudio = SkinData.AngryDrumsAudioAction.ApplyToSource(nutcracker.creatureVoice);
            if(audioAnimEvents!=null)
            {
                vanillaJointSqueakAudio = SkinData.JointSqueaksAudioListAction.Apply(ref audioAnimEvents.randomClips);
                vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips2);
            }
            if (LongRangeSilenced)
            {
                modLongRange = CreateModdedAudioSource(nutcracker.longRangeAudio, "modLongRange");
                nutcracker.longRangeAudio.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(nutcracker.creatureSFX, "modEffects");
                nutcracker.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                    audioAnimEvents.audioToPlayB = modCreatureEffects;
                }
            }

            vanillaSpurtMat = SkinData.BloodSpurtMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaFountainMat = SkinData.BloodFountainMaterialAction.Apply(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystemRenderer>(), 0);

            vanillaSpurtParticle = SkinData.BloodSpurtParticleAction.ApplyRef(ref audioAnimEvents.particle);
            replacementFountainParticle = SkinData.BloodFountainParticleAction.Apply(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystem>());

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "spinecontainer", enemy.transform.Find($"{ ANCHOR_PATH }/spinecontainer") } }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(nutcracker, this);
        }

        public override void Remove(GameObject enemy)
        {
            NutcrackerEnemyAI nutcracker = enemy.GetComponent<NutcrackerEnemyAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(nutcracker, this);
            if (LongRangeSilenced)
            {
                DestroyModdedAudioSource(modLongRange);
                nutcracker.longRangeAudio.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                nutcracker.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = nutcracker.creatureSFX;
                    audioAnimEvents.audioToPlayB = nutcracker.creatureSFX;
                }
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.TorsoTurnAudioAction.RemoveFromSource(nutcracker.torsoTurnAudio, vanillaTorsoTurnAudio);
            SkinData.TorsoFinishTurningAudioListAction.Remove(ref nutcracker.torsoFinishTurningClips, vanillaTorsoTurnFinishAudio);
            SkinData.AimAudioAction.Remove(ref nutcracker.aimSFX, vanillaAimAudio);
            SkinData.KickAudioAction.Remove(ref nutcracker.kickSFX, vanillaKickAudio);
            SkinData.AngryDrumsAudioAction.RemoveFromSource(nutcracker.creatureVoice, vanillaAngryAudio);
            if (audioAnimEvents != null)
            {
                SkinData.JointSqueaksAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaJointSqueakAudio);
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips2, vanillaFootstepsAudio);
            }

            SkinData.BloodSpurtParticleAction.RemoveRef(ref audioAnimEvents.particle, vanillaSpurtParticle);
            SkinData.BloodFountainParticleAction.Remove(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystem>(), replacementFountainParticle);

            SkinData.BloodSpurtMaterialAction.Remove(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSpurtMat);
            SkinData.BloodFountainMaterialAction.Remove(nutcracker.transform.Find(BLOOD_FOUNTAIN_PARTICLE)?.GetComponent<ParticleSystemRenderer>(), 0, vanillaFountainMat);

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

        public void OnReloadShotgun(NutcrackerEnemyAI nutcracker)
        {
            if (LongRangeSilenced)
            {
                modLongRange.PlayOneShot(SkinData.HitEyeAudioAction.WorkingClip(nutcracker.enemyType.audioClips[2]));
            }
        }

        public void OnEnterInspectState(NutcrackerEnemyAI nutcracker, bool headPopUp)
        {
            if(LongRangeSilenced)
            {
                modLongRange.PlayOneShot(SkinData.HitEyeAudioAction.WorkingClip(nutcracker.enemyType.audioClips[3]));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (!enemy.isEnemyDead && EffectsSilenced)
            {
                if (enemy.currentBehaviourStateIndex == 2)
                {
                    modCreatureEffects.PlayOneShot(SkinData.HitEyeAudioAction.WorkingClip(enemy.enemyType.audioClips[0]));
                }
                else
                {
                    modCreatureEffects.PlayOneShot(SkinData.HitEyeAudioAction.WorkingClip(enemy.enemyType.audioClips[1]));
                }
            }
        }

        public void OnKickPlayer(NutcrackerEnemyAI nutcracker, GameNetcodeStuff.PlayerControllerB player)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
                modCreatureEffects.PlayOneShot(SkinData.KickAudioAction.WorkingClip(vanillaKickAudio));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
            }
        }
    }

}