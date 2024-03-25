using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BunkerSpiderSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "MeshContainer/MeshRenderer";
        protected const string LEFT_FANG_PATH = "MeshContainer/AnimContainer/Armature/Head/LeftFang";
        protected const string RIGHT_FANG_PATH = "MeshContainer/AnimContainer/Armature/Head/RightFang";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer";

        protected Material vanillaBodyMaterial;
        protected Material vanillaLeftFangMaterial;
        protected Material vanillaRightFangMaterial;
        protected Mesh vanillaRightFangMesh;
        protected Mesh vanillaLeftFangMesh;
        protected AudioClip[] vanillaFootstepSounds;
        protected AudioClip vanillaAttackSound;
        protected AudioClip vanillaSpoolSound;
        protected AudioClip vanillaHangSound;
        protected AudioClip vanillaHitSound;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftFangMaterialAction { get; }
        protected MaterialAction RightFangMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftFangMeshAction { get; }
        protected StaticMeshAction RightFangMeshAction { get; }
        protected AudioListAction FootstepsAction { get; }
        protected AudioAction AttackAudioAction { get; }
        protected AudioAction SpoolPlayerAudioAction { get; }
        protected AudioAction HangPlayerAudioAction { get; }
        protected AudioAction HitAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public BunkerSpiderSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction,
            MaterialAction leftFangMaterialAction,
            MaterialAction rightFangMaterialAction,
            SkinnedMeshAction bodyMeshAction,
            StaticMeshAction leftFangMeshAction,
            StaticMeshAction rightFangMeshAction,
            AudioListAction footstepsAudioAction,
            AudioAction attackAudioAction,
            AudioAction spoolAudioAction,
            AudioAction hangAudioAction,
            AudioAction hitAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            RightFangMaterialAction = rightFangMaterialAction;
            LeftFangMaterialAction = leftFangMaterialAction;
            BodyMeshAction = bodyMeshAction;
            RightFangMeshAction = rightFangMeshAction;
            LeftFangMeshAction = leftFangMeshAction;
            FootstepsAction = footstepsAudioAction;
            AttackAudioAction = attackAudioAction;
            SpoolPlayerAudioAction = spoolAudioAction;
            HangPlayerAudioAction = hangAudioAction;
            HitAudioAction = hitAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMaterial = LeftFangMaterialAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightFangMaterial = RightFangMaterialAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMesh = LeftFangMeshAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightFangMesh = RightFangMeshAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAttackSound = AttackAudioAction.Apply(ref enemy.GetComponent<SandSpiderAI>().attackSFX);
            vanillaSpoolSound = SpoolPlayerAudioAction.Apply(ref enemy.GetComponent<SandSpiderAI>().spoolPlayerSFX);
            vanillaHangSound = HangPlayerAudioAction.Apply(ref enemy.GetComponent<SandSpiderAI>().hangPlayerSFX);
            vanillaHitSound = HitAudioAction.Apply(ref enemy.GetComponent<SandSpiderAI>().hitSpiderSFX);
            vanillaFootstepSounds = FootstepsAction.Apply(ref enemy.GetComponent<SandSpiderAI>().footstepSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") } }
            );
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            LeftFangMaterialAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftFangMaterial);
            RightFangMaterialAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightFangMaterial);
            LeftFangMeshAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftFangMesh);
            RightFangMeshAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightFangMesh);
            AttackAudioAction.Remove(ref enemy.GetComponent<SandSpiderAI>().attackSFX, vanillaAttackSound);
            SpoolPlayerAudioAction.Remove(ref enemy.GetComponent<SandSpiderAI>().spoolPlayerSFX, vanillaSpoolSound);
            HangPlayerAudioAction.Remove(ref enemy.GetComponent<SandSpiderAI>().hangPlayerSFX, vanillaHangSound);
            HitAudioAction.Remove(ref enemy.GetComponent<SandSpiderAI>().hitSpiderSFX, vanillaHitSound);
            FootstepsAction.Remove(ref enemy.GetComponent<SandSpiderAI>().footstepSFX, vanillaFootstepSounds);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }
}