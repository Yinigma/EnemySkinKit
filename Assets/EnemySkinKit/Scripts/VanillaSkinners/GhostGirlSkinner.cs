using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class GhostGirlSkinner : BaseSkinner
    {
        protected const string LEFT_EYE_PATH = "DressGirlModel/AnimContainer/metarig/spine/spine.002/spine.003/spine.004/spine.006/Icosphere";
        protected const string RIGHT_EYE_PATH = "DressGirlModel/AnimContainer/metarig/spine/spine.002/spine.003/spine.004/spine.006/Icosphere.001";
        protected const string BODY_PATH = "DressGirlModel/basemesh";
        protected const string ANCHOR_PATH = "DressGirlModel/AnimContainer";

        protected Material vanillaBodyMaterial;
        protected Material vanillaLeftEyeMaterial;
        protected Material vanillaRightEyeMaterial;
        protected Mesh vanillaLeftEyeMesh;
        protected Mesh vanillaRightEyeMesh;

        protected AudioClip vanillaSkipWalkSound;
        protected AudioClip vanillaBreatheSound;
        protected AudioClip vanillaHeartbeatSound;
        protected AudioClip[] vanillaHauntingSounds;
        protected List<GameObject> activeAttachments;


        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftEyeMaterialAction { get; }
        protected MaterialAction RightEyeMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftEyeMeshAction { get; }
        protected StaticMeshAction RightEyeMeshAction { get; }

        protected AudioAction SkipWalkSoundAction { get; }
        protected AudioAction BreatheSoundAction { get; }
        protected AudioListAction HauntingSoundsAction { get; }
        protected AudioAction HeartbeatSoundAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        private GhostGirlDisappearEventHandler eventHandler;

        public GhostGirlSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            MaterialAction leftEyeMaterialAction, 
            MaterialAction rightEyeMaterialAction, 
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction leftEyeMeshAction, 
            StaticMeshAction rightEyeMeshAction,
            AudioListAction hauntingSoundsAction,
            AudioAction breatheSoundAction,
            AudioAction skipWalkSoundAction,
            AudioAction heartbeatSoundAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            LeftEyeMaterialAction = leftEyeMaterialAction;
            RightEyeMaterialAction = rightEyeMaterialAction;
            BodyMeshAction = bodyMeshAction;
            LeftEyeMeshAction = leftEyeMeshAction;
            RightEyeMeshAction = rightEyeMeshAction;
            SkipWalkSoundAction = skipWalkSoundAction;
            BreatheSoundAction = breatheSoundAction;
            HauntingSoundsAction = hauntingSoundsAction;
            HeartbeatSoundAction = heartbeatSoundAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftEyeMaterial = LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaLeftEyeMesh = LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaHauntingSounds = HauntingSoundsAction.Apply(ref enemy.GetComponent<DressGirlAI>().appearStaringSFX);
            vanillaSkipWalkSound = SkipWalkSoundAction.Apply(ref enemy.GetComponent<DressGirlAI>().skipWalkSFX);
            vanillaBreatheSound = BreatheSoundAction.Apply(ref enemy.GetComponent<DressGirlAI>().breathingSFX);
            vanillaHeartbeatSound = HeartbeatSoundAction.ApplyToSource(enemy.GetComponent<DressGirlAI>().heartbeatMusic);

            GameObject moddedGO = BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig") } }
            );

            eventHandler = new GhostGirlDisappearEventHandler(moddedGO);

            EnemySkinRegistry.RegisterEnemyEventHandler(enemy.GetComponent<DressGirlAI>(), eventHandler);
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            LeftEyeMaterialAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftEyeMaterial);
            RightEyeMaterialAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightEyeMaterial);

            LeftEyeMeshAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftEyeMesh);
            RightEyeMeshAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightEyeMesh);

            HauntingSoundsAction.Remove(ref enemy.GetComponent<DressGirlAI>().appearStaringSFX, vanillaHauntingSounds);
            SkipWalkSoundAction.Remove(ref enemy.GetComponent<DressGirlAI>().skipWalkSFX, vanillaSkipWalkSound);
            BreatheSoundAction.Remove(ref enemy.GetComponent<DressGirlAI>().breathingSFX, vanillaBreatheSound);
            HeartbeatSoundAction.RemoveFromSource(enemy.GetComponent<DressGirlAI>().heartbeatMusic, vanillaHeartbeatSound);

            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );

            EnemySkinRegistry.RemoveEnemyEventHandler(enemy.GetComponent<DressGirlAI>(), eventHandler);
        }

    }

    class GhostGirlDisappearEventHandler : GhostGirlEventHandler
    {

        private GameObject ModdedMeshGO { get; }

        internal GhostGirlDisappearEventHandler(GameObject ghostGirl)
        {
            ModdedMeshGO = ghostGirl;
        }

        public void OnHit(EnemyAI enemy, PlayerControllerB attackingPlayer) { }

        public void OnStun(EnemyAI enemy, PlayerControllerB attackingPlayer) { }

        public void OnKilled(EnemyAI enemy) { }

        public void OnSpawn(EnemyAI enemy) { }

        public void OnSpawnFinished(EnemyAI enemy) { }

        public void OnTargetPlayer(EnemyAI enemy, PlayerControllerB target) { }

        public void Update(EnemyAI enemy) { }

        public void OnDestroy(EnemyAI enemy) { }

        public void OnChoosePlayer(DressGirlAI ghostGirl, PlayerControllerB target) { }

        public void OnStartChasing(DressGirlAI ghostGirl) { }

        public void OnStopChasing(DressGirlAI ghostGirl) { }

        public void OnKillPlayer(DressGirlAI ghostGirl, PlayerControllerB player) { }

        public void OnHide(DressGirlAI ghostGirl)
        {
            if (EnemySkinKit.LogLevelSetting >= LogLevel.INFO) EnemySkinKit.SkinKitLogger.LogInfo("Got Ghost Girl Hide Event");
            SkinnedMeshRenderer renderer = ModdedMeshGO?.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                renderer.gameObject.layer = LayerMask.NameToLayer("EnemiesNotRendered");
            }
        }

        public void OnShow(DressGirlAI ghostGirl)
        {
            if (EnemySkinKit.LogLevelSetting >= LogLevel.INFO) EnemySkinKit.SkinKitLogger.LogInfo("Got Ghost Girl Show Event");
            SkinnedMeshRenderer renderer = ModdedMeshGO?.GetComponentInChildren<SkinnedMeshRenderer>();
            if(renderer != null)
            {
                renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
            }
        }

        public void OnStartToDisappear(DressGirlAI ghostGirl) { }
    }
}