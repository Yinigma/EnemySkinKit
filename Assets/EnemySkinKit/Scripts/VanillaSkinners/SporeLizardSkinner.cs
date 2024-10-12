using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class SporeLizardSkinner : BaseSkinner, SporeLizardEventHandler
    {
        protected const string BODY_PATH = "PufferModel/BezierCurve";
        protected const string ANCHOR_PATH = "PufferModel/AnimContainer/Armature";
        protected const string ANIM_EVENT_PATH = "PufferModel/AnimContainer";
        protected const string FOOTSTEP_AUDIO_PATH = "FootstepSFX";
        protected VanillaMaterial vanillaBodyMaterial;

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modFootsteps;
        protected AudioReflector modCreatureVoice;
        protected SporeLizardSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        

        public SporeLizardSkinner(SporeLizardSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            PufferAI lizard = enemy.GetComponent<PufferAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            SkinData.FrightenedAudioListAction.ApplyToMap(lizard.frightenSFX, clipMap);
            SkinData.StompAudioAction.ApplyToMap(lizard.stomp, clipMap);
            SkinData.AngryAudioAction.ApplyToMap(lizard.angry, clipMap);
            SkinData.PuffAudioAction.ApplyToMap(lizard.puff, clipMap);
            SkinData.NervousMumbleAudioAction.ApplyToMap(lizard.nervousMumbling, clipMap);
            SkinData.RattleTailAudioAction.ApplyToMap(lizard.rattleTail, clipMap);
            SkinData.BiteAudioAction.ApplyToMap(lizard.bitePlayerSFX, clipMap);
            SkinData.FootstepsAudioListAction.ApplyToMap(lizard.footstepsSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(lizard.enemyType.hitBodySFX, clipMap);

            AudioSource footstepsAudio = lizard.transform.Find(FOOTSTEP_AUDIO_PATH)?.GetComponent<AudioSource>();
            if(footstepsAudio != null)
            {
                modFootsteps = CreateAudioReflector(footstepsAudio, clipMap, lizard.NetworkObjectId); 
                footstepsAudio.mute = true;
            }
            modCreatureEffects = CreateAudioReflector(lizard.creatureSFX, clipMap, lizard.NetworkObjectId); 
            lizard.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(lizard.creatureVoice, clipMap, lizard.NetworkObjectId); 
            lizard.creatureVoice.mute = true;

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(lizard, this);
        }

        public override void Remove(GameObject enemy)
        {
            PufferAI lizard = enemy.GetComponent<PufferAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(lizard, this);

            AudioSource footstepsAudio = lizard.transform.Find(FOOTSTEP_AUDIO_PATH)?.GetComponent<AudioSource>();
            if (footstepsAudio != null)
            {
                DestroyAudioReflector(modFootsteps);
                footstepsAudio.mute = false;
            }
            DestroyAudioReflector(modCreatureEffects);
            lizard.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            lizard.creatureVoice.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }
    }

}