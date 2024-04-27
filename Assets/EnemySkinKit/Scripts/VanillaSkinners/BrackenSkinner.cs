using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BrackenSkinner : BaseSkinner, BrackenEventHandler
    {
        protected const string MESH_PATH = "FlowermanModel/LOD1";
        protected const string LEFT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/LeftEye";
        protected const string RIGHT_EYE_PATH = "FlowermanModel/AnimContainer/metarig/Torso1/Torso2/Torso3/Neck1/Neck2/Head1/RightEye";
        protected const string ANCHOR_PATH = "FlowermanModel/AnimContainer";
        protected Material vanillaBodyMaterial;
        protected Material vanillaLeafMaterial;
        protected Material vanillaRightEyeMaterial;
        protected Material vanillaLeftEyeMaterial;

        protected Mesh vanillaRightEyeMesh;
        protected Mesh vanillaLeftEyeMesh;

        protected AudioClip vanillaAngerSound;
        protected AudioClip vanillaKillSound;
        protected AudioClip vanillaFoundSound;
        protected AudioClip[] vanillaLeafRustleSounds;
        protected List<GameObject> activeAttachments;

        protected MaterialAction LeafMaterialAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftEyeMaterialAction { get; }
        protected MaterialAction RightEyeMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftEyeMeshAction { get; }
        protected StaticMeshAction RightEyeMeshAction { get; }
        protected AudioAction AngerAudioAction { get; }
        protected AudioAction SnapNeckAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        protected AudioAction FoundAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioAction StunAudioAction { get; }
        protected AudioListAction LeafRustleAudioListAction { get; }

        protected bool VoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        public BrackenSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction leafMaterialAction,
            MaterialAction bodyMaterialAction,
            MaterialAction leftEyeMaterialAction,
            MaterialAction rightEyeMaterialAction,
            SkinnedMeshAction bodyMeshAction,
            StaticMeshAction leftEyeMeshAction,
            StaticMeshAction rightEyeMeshAction,
            AudioAction angerAudioAction,
            AudioAction snapNeckAudioAction,
            AudioAction foundAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction stunAudioAction,
            AudioListAction leafRustleAudioListAction
        )
        {
            LeafMaterialAction = leafMaterialAction;
            BodyMaterialAction = bodyMaterialAction;
            LeftEyeMaterialAction = leftEyeMaterialAction;
            RightEyeMaterialAction = rightEyeMaterialAction;
            BodyMeshAction = bodyMeshAction;
            LeftEyeMeshAction = leftEyeMeshAction;
            RightEyeMeshAction = rightEyeMeshAction;
            AngerAudioAction = angerAudioAction;
            SnapNeckAudioAction = snapNeckAudioAction;
            Attachments = attachments;
            FoundAudioAction = foundAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            StunAudioAction = stunAudioAction;
            LeafRustleAudioListAction = leafRustleAudioListAction;
        }

        public override void Apply(GameObject enemy)
        {
            FlowermanAI bracken = enemy.GetComponent<FlowermanAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if(VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(bracken.creatureVoice, "modVoice");
                bracken.creatureVoice.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(bracken.creatureSFX, "modEffects");
                bracken.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                }
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() );
            vanillaLeafMaterial = LeafMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(MESH_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaLeftEyeMaterial = LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaLeftEyeMesh = LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngerSound = AngerAudioAction.ApplyToSource(bracken.creatureAngerVoice);
            vanillaKillSound = SnapNeckAudioAction.Apply(ref bracken.crackNeckSFX);
            if (audioAnimEvents != null)
            {
                vanillaFoundSound = FoundAudioAction.Apply(ref audioAnimEvents.audioClip);
                vanillaLeafRustleSounds = LeafRustleAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(bracken, this);
        }

        public override void Remove(GameObject enemy)
        {
            FlowermanAI bracken = enemy.GetComponent<FlowermanAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bracken, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                bracken.creatureVoice.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                bracken.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = bracken.creatureSFX;
                }
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            LeafMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeafMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<Renderer>(), 1, vanillaBodyMaterial);
            LeftEyeMaterialAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftEyeMaterial);
            RightEyeMaterialAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightEyeMaterial);
            BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            LeftEyeMeshAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftEyeMesh);
            RightEyeMeshAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightEyeMesh);
            AngerAudioAction.RemoveFromSource(bracken.creatureAngerVoice, vanillaAngerSound);
            SnapNeckAudioAction.Remove(ref bracken.crackNeckSFX, vanillaKillSound);
            if(audioAnimEvents != null)
            {
                FoundAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaFoundSound);
                LeafRustleAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaLeafRustleSounds);
            }
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.Stop();
            }
            if (VoiceSilenced)
            {
                modCreatureVoice.Stop();
            }
        }
    }
}