using AntlerShed.EnemySkinKit.SkinAction;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class EarthLeviathanSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "MeshContainer/Renderer";
        protected const string ANCHOR_PATH = "MeshContainer/Armature";

        protected Material vanillaBodyMaterial;
        protected AudioClip[] vanillaGroundRumbleAudio;
        protected AudioClip[] vanillaAmbientRumbleAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaEmergeAudio;
        protected AudioClip[] vanillaRoarAudio;
        protected List<GameObject> activeAttachments;

        protected SkinnedMeshAction BodyMeshAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected AudioListAction GroundRumbleAudioAction { get; }
        protected AudioListAction AmbientRumbleAudioAction { get; }
        protected AudioListAction RoarAudioAction { get; }
        protected AudioAction EmergeAudioAction { get; }
        protected AudioAction HitGroundAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public EarthLeviathanSkinner
        (
            ArmatureAttachment[] attachments,
            SkinnedMeshAction bodyMeshAction, 
            MaterialAction bodyMaterialAction,
            AudioListAction groundRumbleAudioAction,
            AudioListAction ambientRumbleAudioAction,
            AudioListAction roarAudioAction,
            AudioAction emergeAudioAction,
            AudioAction hitGroundAudioAction
        )
        {
            BodyMeshAction = bodyMeshAction;
            BodyMaterialAction = bodyMaterialAction;
            GroundRumbleAudioAction = groundRumbleAudioAction;
            AmbientRumbleAudioAction = ambientRumbleAudioAction;
            RoarAudioAction = roarAudioAction;
            EmergeAudioAction = emergeAudioAction;
            HitGroundAudioAction = hitGroundAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaAmbientRumbleAudio = AmbientRumbleAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().ambientRumbleSFX);
            vanillaGroundRumbleAudio = GroundRumbleAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().groundRumbleSFX);
            vanillaRoarAudio = RoarAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().roarSFX);
            vanillaEmergeAudio = EmergeAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().emergeFromGroundSFX);
            vanillaHitGroundAudio = HitGroundAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().hitGroundSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[] 
                { 
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() 
                }, 
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public override void Remove(GameObject enemy)
        {
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            AmbientRumbleAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().ambientRumbleSFX, vanillaAmbientRumbleAudio);
            GroundRumbleAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().groundRumbleSFX, vanillaGroundRumbleAudio);
            RoarAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().roarSFX, vanillaRoarAudio);
            EmergeAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().emergeFromGroundSFX, vanillaEmergeAudio);
            HitGroundAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().hitGroundSFX, vanillaHitGroundAudio);
            BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, enemy.transform.Find(ANCHOR_PATH));
        }
    }
}