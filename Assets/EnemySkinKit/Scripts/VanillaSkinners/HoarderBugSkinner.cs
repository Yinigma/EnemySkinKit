using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class HoarderBugSkinner : BaseSkinner, HoarderBugEventHandler
    {
        protected const string ANCHOR_PATH = "HoarderBugModel/AnimContainer";
        protected const string BODY_PATH = "HoarderBugModel/Cube";
        protected const string ORGANS_PATH = "HoarderBugModel/Cube.001";
        protected const string LEFT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/LeftWing";
        protected const string RIGHT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/RightWing";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaLeftWingMaterial;
        protected VanillaMaterial vanillaRightWingMaterial;

        protected Mesh vanillaLeftWingMesh;
        protected Mesh vanillaRightWingMesh;

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected HoarderBugSkin SkinData { get; }

        public HoarderBugSkinner(HoarderBugSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            HoarderBugAI bug = enemy.GetComponent<HoarderBugAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaLeftWingMaterial = SkinData.LeftWingMaterialAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightWingMaterial = SkinData.RightWingMaterialAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftWingMesh = SkinData.LeftWingMeshAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightWingMesh = SkinData.RightWingMeshAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            SkinData.AngryChirpsAudioListAction.ApplyToMap(bug.angryScreechSFX, clipMap);
            SkinData.ChitterAudioListAction.ApplyToMap(bug.chitterSFX, clipMap);
            SkinData.BeginAttackAudioAction.ApplyToMap(bug.angryVoiceSFX, clipMap);
            SkinData.FlyAudioAction.ApplyToMap(bug.bugFlySFX, clipMap);
            SkinData.HitPlayerAudioAction.ApplyToMap(bug.hitPlayerSFX, clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(bug.enemyType.hitBodySFX, clipMap);
            SkinData.StunAudioAction.ApplyToMap(bug.enemyType.stunSFX, clipMap);
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }

            modCreatureEffects = CreateAudioReflector(bug.creatureSFX, clipMap, bug.NetworkObjectId); 
            bug.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(bug.creatureVoice, clipMap, bug.NetworkObjectId); 
            bug.creatureVoice.mute = true;

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") } }
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(false);
            EnemySkinRegistry.RegisterEnemyEventHandler(bug, this);
        }

        public override void Remove(GameObject enemy)
        {
            HoarderBugAI bug = enemy.GetComponent<HoarderBugAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(bug, this);


            DestroyAudioReflector(modCreatureEffects);
            bug.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            bug.creatureVoice.mute = false;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.LeftWingMaterialAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftWingMaterial);
            SkinData.RightWingMaterialAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightWingMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.LeftWingMeshAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftWingMesh);
            SkinData.RightWingMeshAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightWingMesh);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(true);
        }
    }
}