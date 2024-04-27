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

        protected Material vanillaBodyMaterial;
        protected Material vanillaLeftWingMaterial;
        protected Material vanillaRightWingMaterial;

        protected Mesh vanillaLeftWingMesh;
        protected Mesh vanillaRightWingMesh;

        protected AudioClip[] vanillaChitterAudio;
        protected AudioClip[] vanillaAngryScreechAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaAngryVoiceAudio;
        protected AudioClip vanillaFlyAudio;
        protected AudioClip vanillaHitPlayerAudio;
        protected List<GameObject> activeAttachments;

        protected bool VoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftWingMaterialAction { get; }
        protected MaterialAction RightWingMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftWingMeshAction { get; }
        protected StaticMeshAction RightWingMeshAction { get; }
        protected AudioListAction ChitterAudioAction { get; }
        protected AudioListAction AngryScreechAudioAction { get; }
        protected AudioListAction FootstepsAudioAction { get; }
        protected AudioAction AngryAudioAction { get; }
        protected AudioAction FlyAudioAction { get; }
        protected AudioAction HitPlayerAction { get; }
        protected AudioAction StunAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public HoarderBugSkinner
        (

            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            MaterialAction leftWingMaterialAction, 
            MaterialAction rightWingMaterialAction, 
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction leftWingMeshAction, 
            StaticMeshAction rightWingMeshAction,
            AudioListAction chitterAudioAction,
            AudioListAction angryScreechAudioAction,
            AudioAction angryAudioAction,
            AudioAction flyAudioAction,
            AudioAction hitPlayerAction,
            AudioAction stunAction,
            AudioAction hitBodyAction,
            AudioListAction footstepsAction

        )
        {
            BodyMaterialAction = bodyMaterialAction;
            LeftWingMaterialAction = leftWingMaterialAction;
            RightWingMaterialAction = rightWingMaterialAction;
            BodyMeshAction = bodyMeshAction;
            LeftWingMeshAction = leftWingMeshAction;
            RightWingMeshAction = rightWingMeshAction;
            ChitterAudioAction = chitterAudioAction;
            AngryScreechAudioAction = angryScreechAudioAction;
            AngryAudioAction = angryAudioAction;
            FlyAudioAction = flyAudioAction;
            HitPlayerAction = hitPlayerAction;
            HitBodyAudioAction = hitBodyAction;
            StunAudioAction = stunAction;
            FootstepsAudioAction = footstepsAction;
            Attachments = attachments;
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
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaLeftWingMaterial = LeftWingMaterialAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightWingMaterial = RightWingMaterialAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftWingMesh = LeftWingMeshAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightWingMesh = RightWingMeshAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngryScreechAudio = AngryScreechAudioAction.Apply(ref bug.angryScreechSFX);
            vanillaChitterAudio = ChitterAudioAction.Apply(ref bug.chitterSFX);
            vanillaAngryVoiceAudio = AngryAudioAction.Apply(ref bug.angryVoiceSFX);
            vanillaFlyAudio = FlyAudioAction.Apply(ref bug.bugFlySFX);
            vanillaHitPlayerAudio = HitPlayerAction.Apply(ref bug.hitPlayerSFX);
            if (audioAnimEvents != null)
            {
                vanillaFootstepsAudio = FootstepsAudioAction.Apply(ref audioAnimEvents.randomClips);
            }
            BodyMeshAction.Apply
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
            LeftWingMaterialAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftWingMaterial);
            RightWingMaterialAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightWingMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            LeftWingMeshAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftWingMesh);
            RightWingMeshAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightWingMesh);

            AngryScreechAudioAction.Remove(ref bug.angryScreechSFX, vanillaAngryScreechAudio);
            ChitterAudioAction.Remove(ref bug.chitterSFX, vanillaChitterAudio);
            AngryAudioAction.Remove(ref bug.angryVoiceSFX, vanillaAngryVoiceAudio);
            FlyAudioAction.Remove(ref bug.bugFlySFX, vanillaFlyAudio);
            HitPlayerAction.Remove(ref bug.hitPlayerSFX, vanillaHitPlayerAudio);
            if (audioAnimEvents != null)
            {
                FootstepsAudioAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(true);
        }

        //public void ChitterAtPlayer

        //public void MakeAngryNoise

        public void OnEnterChasingState(HoarderBugAI instance)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.clip = FlyAudioAction.WorkingClip(vanillaFlyAudio);
                modCreatureVoice.Play();
                AudioClip[] angryClips = AngryScreechAudioAction.WorkingClips(vanillaAngryScreechAudio);
                AudioClip angryClip = angryClips[UnityEngine.Random.Range(0, angryClips.Length)];
                modCreatureEffects.PlayOneShot(angryClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, angryClip);
            }
        }

        public void OnHitPlayer(HoarderBugAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if (VoiceSilenced)
            {
                modCreatureEffects.PlayOneShot(HitPlayerAction.WorkingClip(vanillaHitPlayerAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitPlayerAction.WorkingClip(vanillaHitPlayerAudio));
            }
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnSwitchLookAtPlayer(HoarderBugAI instance, GameNetcodeStuff.PlayerControllerB watchingPlayer)
        {
            if (VoiceSilenced && watchingPlayer != null)
            {
                AudioClip[] chitterClips = ChitterAudioAction.WorkingClips(vanillaChitterAudio);
                AudioClip chitter = chitterClips[UnityEngine.Random.Range(0, chitterClips.Length)];
                modCreatureVoice.PlayOneShot(chitter, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, chitter, 0.85f);
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (VoiceSilenced && playSoundEffect)
            {
                modCreatureVoice.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
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