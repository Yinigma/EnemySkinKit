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
        protected Material vanillaBodyMaterial;
        protected AudioClip[] vanillaTorsoTurnFinishAudio;
        protected AudioClip[] vanillaJointSqueakAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaAimAudio;
        protected AudioClip vanillaKickAudio;
        protected AudioClip vanillaAngryAudio;
        protected AudioClip vanillaTorsoTurnAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioListAction TorsoFinishTurnAudioAction { get; }
        protected AudioListAction JointSqueaksAudioAction { get; }
        protected AudioListAction FootstepsAudioAction { get; }
        protected AudioAction AimAudioAction { get; }
        protected AudioAction TorsoTurnAudioAction { get; }
        protected AudioAction KickAudioAction { get; }
        protected AudioAction HeadPopUpAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioAction HitEyeAudioAction { get; }
        protected AudioAction ReloadAudioAction { get; }
        protected AudioAction AngryAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool LongRangeSilenced => HeadPopUpAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN || HitEyeAudioAction.actionType != AudioActionType.RETAIN || ReloadAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modLongRange;

        public NutcrackerSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction torsoTurnAudioAction,
            AudioListAction torsoFinishTurnAudioAction,
            AudioAction aimAudioAction,
            AudioAction kickAudioAction,
            AudioAction headPopUpAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction hitEyeAudioAction,
            AudioAction reloadAudioAction,
            AudioAction angryAudioAction,
            AudioListAction jointSqueaksAudioAction,
            AudioListAction footstepsAudioAction
            
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            TorsoTurnAudioAction = torsoTurnAudioAction;
            TorsoFinishTurnAudioAction = torsoFinishTurnAudioAction;
            AimAudioAction = aimAudioAction;
            KickAudioAction = kickAudioAction;
            HeadPopUpAudioAction = headPopUpAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            HitEyeAudioAction = hitEyeAudioAction;
            ReloadAudioAction = reloadAudioAction;
            AngryAudioAction = angryAudioAction;
            JointSqueaksAudioAction = jointSqueaksAudioAction;
            FootstepsAudioAction = footstepsAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            NutcrackerEnemyAI nutcracker = enemy.GetComponent<NutcrackerEnemyAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTorsoTurnAudio = TorsoTurnAudioAction.ApplyToSource(nutcracker.torsoTurnAudio);
            vanillaTorsoTurnFinishAudio = TorsoFinishTurnAudioAction.Apply(ref nutcracker.torsoFinishTurningClips);
            vanillaAimAudio = AimAudioAction.Apply(ref nutcracker.aimSFX);
            vanillaKickAudio = KickAudioAction.Apply(ref nutcracker.kickSFX);
            vanillaAngryAudio = AngryAudioAction.ApplyToSource(nutcracker.creatureVoice);
            if(audioAnimEvents!=null)
            {
                vanillaJointSqueakAudio = JointSqueaksAudioAction.Apply(ref audioAnimEvents.randomClips);
                vanillaFootstepsAudio = FootstepsAudioAction.Apply(ref audioAnimEvents.randomClips2);
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
            BodyMeshAction.Apply
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
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            TorsoTurnAudioAction.RemoveFromSource(nutcracker.torsoTurnAudio, vanillaTorsoTurnAudio);
            TorsoFinishTurnAudioAction.Remove(ref nutcracker.torsoFinishTurningClips, vanillaTorsoTurnFinishAudio);
            AimAudioAction.Remove(ref nutcracker.aimSFX, vanillaAimAudio);
            KickAudioAction.Remove(ref nutcracker.kickSFX, vanillaKickAudio);
            AngryAudioAction.RemoveFromSource(nutcracker.creatureVoice, vanillaAngryAudio);
            if (audioAnimEvents != null)
            {
                JointSqueaksAudioAction.Remove(ref audioAnimEvents.randomClips, vanillaJointSqueakAudio);
                FootstepsAudioAction.Remove(ref audioAnimEvents.randomClips2, vanillaFootstepsAudio);
            }
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

        public void OnReloadShotgun(NutcrackerEnemyAI nutcracker)
        {
            if (LongRangeSilenced)
            {
                modCreatureEffects.PlayOneShot(HitEyeAudioAction.WorkingClip(nutcracker.enemyType.audioClips[2]));
            }
        }

        public void OnEnterInspectState(NutcrackerEnemyAI nutcracker, bool headPopUp)
        {
            if(LongRangeSilenced)
            {
                modCreatureEffects.PlayOneShot(HitEyeAudioAction.WorkingClip(nutcracker.enemyType.audioClips[3]));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (!enemy.isEnemyDead && EffectsSilenced)
            {
                if (enemy.currentBehaviourStateIndex == 2)
                {
                    modCreatureEffects.PlayOneShot(HitEyeAudioAction.WorkingClip(enemy.enemyType.audioClips[0]));
                }
                else
                {
                    modCreatureEffects.PlayOneShot(HitEyeAudioAction.WorkingClip(enemy.enemyType.audioClips[1]));
                }
            }
        }

        public void OnKickPlayer(NutcrackerEnemyAI nutcracker, GameNetcodeStuff.PlayerControllerB player)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
                modCreatureEffects.PlayOneShot(KickAudioAction.WorkingClip(vanillaKickAudio));
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