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

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaRustMaterial;
        protected VanillaMaterial vanillaHeadMaterial;
        protected Mesh vanillaHeadMesh;
        protected AudioClip[] vanillaSpringAudio;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip vanillaCooldown;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

        protected CoilheadSkin SkinData {get;}

        public CoilHeadSkinner(CoilheadSkin skinData)
        {
            SkinData = skinData;
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
            vanillaCooldown = SkinData.CooldownAudioAction.Apply(ref coilhead.enterCooldownSFX);
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaRustMaterial = SkinData.RustMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaHeadMaterial = SkinData.HeadMaterialAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            vanillaSpringAudio = SkinData.SpringNoisesAudioListAction.Apply(ref coilhead.springNoises);
            if(audioAnimEvents!=null)
            {
                vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            vanillaHeadMesh = SkinData.HeadMeshAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>());
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
            SkinData.CooldownAudioAction.Remove(ref coilhead.enterCooldownSFX, vanillaCooldown);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaBodyMaterial);
            SkinData.RustMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRustMaterial);
            SkinData.HeadMaterialAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaHeadMaterial);
            SkinData.SpringNoisesAudioListAction.Remove(ref coilhead.springNoises, vanillaSpringAudio);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudio);
            }
            SkinData.BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, skinnedMeshReplacement);
            SkinData.HeadMeshAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaHeadMesh);
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }
}