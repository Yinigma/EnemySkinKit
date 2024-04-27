using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class CoilHeadSkinner : BaseSkinner, CoilheadEventHandler
    {
        protected const string BODY_PATH = "SpringManModel/Body";
        protected const string HEAD_PATH = "SpringManModel/Head";
        protected const string ANCHOR_PATH = "SpringManModel/AnimContainer/metarig";
        protected const string ANIM_EVENT_PATH = "SpringManModel/AnimContainer";

        protected Material vanillaBodyMaterial;
        protected Material vanillaRustMaterial;
        protected Material vanillaHeadMaterial;
        protected Mesh vanillaHeadMesh;
        protected AudioClip[] vanillaSpringAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected List<GameObject> activeAttachments;

        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction HeadMeshAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction RustMaterialAction { get; }
        protected MaterialAction HeadMaterialAction { get; }

        protected AudioListAction SpringAudioAction { get; }
        protected AudioListAction FootstepsAudioListAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

        public CoilHeadSkinner
        (
            ArmatureAttachment[] attachments, 
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction headMeshAction, 
            MaterialAction bodyMaterialAction, 
            MaterialAction rustMaterialAction, 
            MaterialAction headMaterialAction, 
            AudioListAction springAudioAction,
            AudioListAction footstepsAudioListAction,
            AudioAction hitBodyAudioAction
        )
        {
            BodyMeshAction = bodyMeshAction;
            HeadMeshAction = headMeshAction;
            BodyMaterialAction = bodyMaterialAction;
            RustMaterialAction = rustMaterialAction;
            HeadMaterialAction = headMaterialAction;
            SpringAudioAction = springAudioAction;
            FootstepsAudioListAction = footstepsAudioListAction;
            HitBodyAudioAction = hitBodyAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            SpringManAI coilhead = enemy.GetComponent<SpringManAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(coilhead.creatureSFX, "modEffects");
                coilhead.creatureSFX.mute = true;
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRustMaterial = RustMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaHeadMaterial = HeadMaterialAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaSpringAudio = SpringAudioAction.Apply(ref coilhead.springNoises);
            if(audioAnimEvents!=null)
            {
                vanillaFootstepsAudio = SpringAudioAction.Apply(ref audioAnimEvents.randomClips);
            }
            vanillaHeadMesh = HeadMeshAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>());
            EnemySkinRegistry.RegisterEnemyEventHandler(coilhead, this);
        }

        public override void Remove(GameObject enemy)
        {
            SpringManAI coilhead = enemy.GetComponent<SpringManAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(coilhead, this);
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                coilhead.creatureSFX.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            RustMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaRustMaterial);
            HeadMaterialAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaHeadMaterial);
            SpringAudioAction.Remove(ref coilhead.springNoises, vanillaSpringAudio);
            if (audioAnimEvents != null)
            {
                SpringAudioAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            HeadMeshAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaHeadMesh);
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }
}