using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class BunkerSpiderSkinner : BaseSkinner, BunkerSpiderEventHandler
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
        protected AudioClip vanillaHitHissSound;
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
        protected AudioAction HitHissAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioAction StunAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool isVoiceSilenced => StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool isEffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        public BunkerSpiderSkinner
        (

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
            AudioAction hitHissAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction stunAudioAction
        )
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
            HitHissAudioAction = hitHissAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            StunAudioAction = stunAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            SandSpiderAI spider = enemy.GetComponent<SandSpiderAI>();
            if (isVoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(spider.creatureVoice, "modVoice");
                spider.creatureVoice.mute = true;
            }
            if (isEffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(spider.creatureSFX, "modEffects");
                spider.creatureSFX.mute = true;
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMaterial = LeftFangMaterialAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightFangMaterial = RightFangMaterialAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMesh = LeftFangMeshAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightFangMesh = RightFangMeshAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAttackSound = AttackAudioAction.Apply(ref spider.attackSFX);
            vanillaSpoolSound = SpoolPlayerAudioAction.Apply(ref spider.spoolPlayerSFX);
            vanillaHangSound = HangPlayerAudioAction.Apply(ref spider.hangPlayerSFX);
            vanillaHitHissSound = HitHissAudioAction.Apply(ref spider.hitSpiderSFX);
            vanillaFootstepSounds = FootstepsAction.Apply(ref spider.footstepSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") } }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(spider, this);
        }

        public override void Remove(GameObject enemy)
        {
            SandSpiderAI spider = enemy.GetComponent<SandSpiderAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(spider, this);
            if (isVoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                spider.creatureVoice.mute = false;
            }
            if (isEffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                spider.creatureSFX.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            LeftFangMaterialAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftFangMaterial);
            RightFangMaterialAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightFangMaterial);
            LeftFangMeshAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftFangMesh);
            RightFangMeshAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightFangMesh);
            AttackAudioAction.Remove(ref spider.attackSFX, vanillaAttackSound);
            SpoolPlayerAudioAction.Remove(ref spider.spoolPlayerSFX, vanillaSpoolSound);
            HangPlayerAudioAction.Remove(ref spider.hangPlayerSFX, vanillaHangSound);
            HitHissAudioAction.Remove(ref spider.hitSpiderSFX, vanillaHitHissSound);
            FootstepsAction.Remove(ref spider.footstepSFX, vanillaFootstepSounds);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (isVoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (isEffectsSilenced)
            {
                if(!enemy.isEnemyDead)
                {
                    modCreatureEffects.PlayOneShot(HitHissAudioAction.WorkingClip(vanillaHitHissSound));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitHissAudioAction.WorkingClip(vanillaHitHissSound));
                }
                if(playSoundEffect)
                {
                    modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                }
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (isEffectsSilenced)
            {
                modCreatureEffects.Stop();
            }
            if (isVoiceSilenced)
            {
                modCreatureVoice.Stop();
            }
        }

        public void OnAttackPlayer(SandSpiderAI spider, GameNetcodeStuff.PlayerControllerB player)
        {
            modCreatureEffects.PlayOneShot(AttackAudioAction.WorkingClip(vanillaAttackSound));
            WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, AttackAudioAction.WorkingClip(vanillaAttackSound));
        }

        public void OnWrapBody(SandSpiderAI spider, DeadBodyInfo spooledBody)
        {
            modCreatureEffects.PlayOneShot(SpoolPlayerAudioAction.WorkingClip(vanillaSpoolSound));
        }
    }
}