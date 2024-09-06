using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class HoarderBugSkinner : BaseSkinner, HoarderBugEventHandler
    {
        protected const string ANCHOR_PATH = "HoarderBugModel/AnimContainer";
        protected const string BODY_PATH = "HoarderBugModel/Cube";
        protected const string ORGANS_PATH = "HoarderBugModel/Cube.001";
        protected const string LEFT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/LeftWing";
        protected const string RIGHT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/RightWing";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaLeftWingMaterial;
        protected VanillaMaterial vanillaRightWingMaterial;

        protected Mesh vanillaLeftWingMesh;
        protected Mesh vanillaRightWingMesh;

        protected AudioClip[] vanillaChitterAudio;
        protected AudioClip[] vanillaAngryScreechAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaAngryVoiceAudio;
        protected AudioClip vanillaFlyAudio;
        protected AudioClip vanillaHitPlayerAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        protected HoarderBugSkin SkinData { get; }

        public HoarderBugSkinner(HoarderBugSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            HoarderBugAI bug = enemy.GetComponent<HoarderBugAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(bug.creatureVoice, "modVoice");
                bug.creatureVoice.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(bug.creatureSFX, "modEffects");
                bug.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                }
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaLeftWingMaterial = SkinData.LeftWingMaterialAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightWingMaterial = SkinData.RightWingMaterialAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftWingMesh = SkinData.LeftWingMeshAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightWingMesh = SkinData.RightWingMeshAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngryScreechAudio = SkinData.AngryChirpsAudioListAction.Apply(ref bug.angryScreechSFX);
            vanillaChitterAudio = SkinData.ChitterAudioListAction.Apply(ref bug.chitterSFX);
            vanillaAngryVoiceAudio = SkinData.BeginAttackAudioAction.Apply(ref bug.angryVoiceSFX);
            vanillaFlyAudio = SkinData.FlyAudioAction.Apply(ref bug.bugFlySFX);
            vanillaHitPlayerAudio = SkinData.HitPlayerAudioAction.Apply(ref bug.hitPlayerSFX);
            if (audioAnimEvents != null)
            {
                vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") } }
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(false);
            EnemySkinRegistry.RegisterEnemyEventHandler(bug, this);
        }

        public override void Remove(GameObject enemy)
        {
            HoarderBugAI bug = enemy.GetComponent<HoarderBugAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bug, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                bug.creatureVoice.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                bug.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = bug.creatureSFX;
                }
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.LeftWingMaterialAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftWingMaterial);
            SkinData.RightWingMaterialAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightWingMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.LeftWingMeshAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftWingMesh);
            SkinData.RightWingMeshAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightWingMesh);

            SkinData.AngryChirpsAudioListAction.Remove(ref bug.angryScreechSFX, vanillaAngryScreechAudio);
            SkinData.ChitterAudioListAction.Remove(ref bug.chitterSFX, vanillaChitterAudio);
            SkinData.BeginAttackAudioAction.Remove(ref bug.angryVoiceSFX, vanillaAngryVoiceAudio);
            SkinData.FlyAudioAction.Remove(ref bug.bugFlySFX, vanillaFlyAudio);
            SkinData.HitPlayerAudioAction.Remove(ref bug.hitPlayerSFX, vanillaHitPlayerAudio);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(true);
        }

        //public void ChitterAtPlayer

        //public void MakeAngryNoise

        public void OnEnterChasingState(HoarderBugAI instance)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.clip = SkinData.FlyAudioAction.WorkingClip(vanillaFlyAudio);
                modCreatureVoice.Play();
                AudioClip[] angryClips = SkinData.AngryChirpsAudioListAction.WorkingClips(vanillaAngryScreechAudio);
                AudioClip angryClip = angryClips[UnityEngine.Random.Range(0, angryClips.Length)];
                modCreatureEffects.PlayOneShot(angryClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, angryClip);
            }
        }

        public void OnHitPlayer(HoarderBugAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if (VoiceSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitPlayerAudioAction.WorkingClip(vanillaHitPlayerAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitPlayerAudioAction.WorkingClip(vanillaHitPlayerAudio));
            }
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnSwitchLookAtPlayer(HoarderBugAI instance, GameNetcodeStuff.PlayerControllerB watchingPlayer)
        {
            if (VoiceSilenced && watchingPlayer != null)
            {
                AudioClip[] chitterClips = SkinData.ChitterAudioListAction.WorkingClips(vanillaChitterAudio);
                AudioClip chitter = chitterClips[UnityEngine.Random.Range(0, chitterClips.Length)];
                modCreatureVoice.PlayOneShot(chitter, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, chitter, 0.85f);
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (VoiceSilenced && playSoundEffect)
            {
                modCreatureVoice.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects?.Stop();
            }
            if (VoiceSilenced)
            {
                modCreatureVoice?.Stop();
            }
        }
    }
}