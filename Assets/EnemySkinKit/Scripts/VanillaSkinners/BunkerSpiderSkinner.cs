using AntlerShed.EnemySkinKit.AudioReflection;
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
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modFootsteps;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

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
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftFangMaterial = SkinData.LeftFangMaterialAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightFangMaterial = SkinData.RightFangMaterialAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaSafetyTextMaterial = SkinData.SafetyTextMaterialAction.Apply(enemy.transform.Find(TEXT_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaLeftFangMesh = SkinData.LeftFangMeshAction.Apply(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightFangMesh = SkinData.RightFangMeshAction.Apply(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSafetyTextMesh = SkinData.SafetyTextMeshAction.Apply(enemy.transform.Find(TEXT_PATH)?.gameObject.GetComponent<MeshFilter>());


            SkinData.AttackAudioAction.ApplyToMap(spider.attackSFX, clipMap);
            SkinData.SpoolPlayerAudioAction.ApplyToMap(spider.spoolPlayerSFX, clipMap);
            SkinData.HangPlayerAudioAction.ApplyToMap(spider.hangPlayerSFX, clipMap);
            SkinData.HitHissAudioAction.ApplyToMap(spider.hitSpiderSFX, clipMap);
            SkinData.FootstepsAction.ApplyToMap(spider.footstepSFX, clipMap);
            SkinData.StunAudioAction.ApplyToMap(spider.enemyType.stunSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(spider.enemyType.hitBodySFX, clipMap);

            modCreatureVoice = CreateAudioReflector(spider.creatureVoice, clipMap, spider.NetworkObjectId);
            spider.creatureVoice.mute = true;
            modFootsteps = CreateAudioReflector(spider.footstepAudio, clipMap, spider.NetworkObjectId);
            spider.footstepAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(spider.creatureSFX, clipMap, spider.NetworkObjectId);
            spider.creatureSFX.mute = true;

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

            DestroyAudioReflector(modCreatureVoice);
            spider.creatureVoice.mute = false;
            DestroyAudioReflector(modFootsteps);
            spider.footstepAudio.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            spider.creatureSFX.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.LeftFangMaterialAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaLeftFangMaterial);
            SkinData.RightFangMaterialAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaRightFangMaterial);
            SkinData.SafetyTextMaterialAction.Remove(enemy.transform.Find(TEXT_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaSafetyTextMaterial);
            SkinData.LeftFangMeshAction.Remove(enemy.transform.Find(LEFT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaLeftFangMesh);
            SkinData.RightFangMeshAction.Remove(enemy.transform.Find(RIGHT_FANG_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaRightFangMesh);
            SkinData.SafetyTextMeshAction.Remove(enemy.transform.Find(TEXT_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSafetyTextMesh);

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