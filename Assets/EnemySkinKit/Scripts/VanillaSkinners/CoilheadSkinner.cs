using AntlerShed.EnemySkinKit.AudioReflection;
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
        protected const string FOOTSTEP_AUDIO_PATH = "SpringManModel/FootstepSFX";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaRustMaterial;
        protected VanillaMaterial vanillaHeadMaterial;
        protected Mesh vanillaHeadMesh;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modFootsteps;
        protected AudioReflector modCreatureVoice;

        protected CoilheadSkin SkinData { get; }

        public CoilHeadSkinner(CoilheadSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            SpringManAI coilhead = enemy.GetComponent<SpringManAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            vanillaRustMaterial = SkinData.RustMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaHeadMaterial = SkinData.HeadMaterialAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
            AudioSource footStepsAudio = coilhead.transform.Find(FOOTSTEP_AUDIO_PATH)?.gameObject.GetComponent<AudioSource>();

            SkinData.SpringNoisesAudioListAction.ApplyToMap(coilhead.springNoises, clipMap);
            SkinData.CooldownAudioAction.ApplyToMap(coilhead.enterCooldownSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(coilhead.enemyType.hitBodySFX, clipMap);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }

            if (footStepsAudio != null)
            {
                modFootsteps = CreateAudioReflector(footStepsAudio, clipMap, coilhead.NetworkObjectId);
                footStepsAudio.mute = true;
            }
            modCreatureEffects = CreateAudioReflector(coilhead.creatureSFX, clipMap, coilhead.NetworkObjectId);
            coilhead.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(coilhead.creatureVoice, clipMap, coilhead.NetworkObjectId);
            coilhead.creatureVoice.mute = true;
            
            

            vanillaHeadMesh = SkinData.HeadMeshAction.Apply(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>());
            EnemySkinRegistry.RegisterEnemyEventHandler(coilhead, this);
        }

        public override void Remove(GameObject enemy)
        {
            SpringManAI coilhead = enemy.GetComponent<SpringManAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(coilhead, this);
            AudioSource footStepsAudio = coilhead.transform.Find(FOOTSTEP_AUDIO_PATH)?.gameObject.GetComponent<AudioSource>();

            DestroyAudioReflector(modFootsteps);
            if (footStepsAudio != null)
            {
                footStepsAudio.mute = false;
            }
            DestroyAudioReflector(modCreatureEffects);
            coilhead.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            coilhead.creatureVoice.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaBodyMaterial);
            SkinData.RustMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRustMaterial);
            SkinData.HeadMaterialAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaHeadMaterial);

            SkinData.BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, skinnedMeshReplacement);
            SkinData.HeadMeshAction.Remove(enemy.transform.Find(HEAD_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaHeadMesh);
        }
    }
}