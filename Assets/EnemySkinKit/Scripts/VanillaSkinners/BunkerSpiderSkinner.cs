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
        protected const string TEXT_PATH = "MeshContainer/AnimContainer/Armature/Abdomen/SpiderText";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaLeftFangMaterial;
        protected VanillaMaterial vanillaRightFangMaterial;
        protected VanillaMaterial vanillaSafetyTextMaterial;
        protected Mesh vanillaRightFangMesh;
        protected Mesh vanillaLeftFangMesh;
        protected Mesh vanillaSafetyTextMesh;
        protected AudioClip[] vanillaFootstepSounds;
        protected AudioClip vanillaAttackSound;
        protected AudioClip vanillaSpoolSound;
        protected AudioClip vanillaHangSound;
        protected AudioClip vanillaHitHissSound;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool isVoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool isEffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;
        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        protected BunkerSpiderSkin SkinData { get; }

        public BunkerSpiderSkinner
        (
            BunkerSpiderSkin skinData
        )
        {
            SkinData = skinData;
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
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMaterial = SkinData.LeftFangMaterialAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightFangMaterial = SkinData.RightFangMaterialAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaSafetyTextMaterial = SkinData.SafetyTextMaterialAction.Apply(enemy.transform.Find(TEXT_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaLeftFangMesh = SkinData.LeftFangMeshAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightFangMesh = SkinData.RightFangMeshAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSafetyTextMesh = SkinData.SafetyTextMeshAction.Apply(enemy.transform.Find(TEXT_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAttackSound = SkinData.AttackAudioAction.Apply(ref spider.attackSFX);
            vanillaSpoolSound = SkinData.SpoolPlayerAudioAction.Apply(ref spider.spoolPlayerSFX);
            vanillaHangSound = SkinData.HangPlayerAudioAction.Apply(ref spider.hangPlayerSFX);
            vanillaHitHissSound = SkinData.HitHissAudioAction.Apply(ref spider.hitSpiderSFX);
            vanillaFootstepSounds = SkinData.FootstepsAction.Apply(ref spider.footstepSFX);
            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
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
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.LeftFangMaterialAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftFangMaterial);
            SkinData.RightFangMaterialAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightFangMaterial);
            SkinData.SafetyTextMaterialAction.Remove(enemy.transform.Find(TEXT_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaSafetyTextMaterial);
            SkinData.LeftFangMeshAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftFangMesh);
            SkinData.RightFangMeshAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightFangMesh);
            SkinData.SafetyTextMeshAction.Remove(enemy.transform.Find(TEXT_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSafetyTextMesh);
            SkinData.AttackAudioAction.Remove(ref spider.attackSFX, vanillaAttackSound);
            SkinData.SpoolPlayerAudioAction.Remove(ref spider.spoolPlayerSFX, vanillaSpoolSound);
            SkinData.HangPlayerAudioAction.Remove(ref spider.hangPlayerSFX, vanillaHangSound);
            SkinData.HitHissAudioAction.Remove(ref spider.hitSpiderSFX, vanillaHitHissSound);
            SkinData.FootstepsAction.Remove(ref spider.footstepSFX, vanillaFootstepSounds);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if (isVoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (isEffectsSilenced)
            {
                if(!enemy.isEnemyDead)
                {
                    modCreatureEffects.PlayOneShot(SkinData.HitHissAudioAction.WorkingClip(vanillaHitHissSound));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitHissAudioAction.WorkingClip(vanillaHitHissSound));
                }
                if(playSoundEffect)
                {
                    modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                    WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
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
            if(isEffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.AttackAudioAction.WorkingClip(vanillaAttackSound));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.AttackAudioAction.WorkingClip(vanillaAttackSound));
            }
        }

        public void OnWrapBody(SandSpiderAI spider, DeadBodyInfo spooledBody)
        {
            if (isEffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.SpoolPlayerAudioAction.WorkingClip(vanillaSpoolSound));
            }
        }
    }
}