using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
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

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaLeftEyeMaterial;
        protected VanillaMaterial vanillaRightEyeMaterial;
        protected Mesh vanillaLeftEyeMesh;
        protected Mesh vanillaRightEyeMesh;

        protected AudioClip[] vanillaSkipWalkSounds;
        protected AudioClip vanillaBreatheSound;
        protected AudioClip vanillaHeartbeatSound;
        protected AudioClip[] vanillaHauntingSounds;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        private GhostGirlDisappearEventHandler eventHandler;

        protected GhostGirlSkin SkinData { get; }

        public GhostGirlSkinner(GhostGirlSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            DressGirlAI girl = enemy.GetComponent<DressGirlAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftEyeMaterial = SkinData.LeftEyeMaterialAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightEyeMaterial = SkinData.RightEyeMaterialAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaLeftEyeMesh = SkinData.LeftEyeMeshAction.Apply(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightEyeMesh = SkinData.RightEyeMeshAction.Apply(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaHauntingSounds = SkinData.HauntingCuesAudioListAction.Apply(ref girl.appearStaringSFX);
            if(audioAnimEvents!=null)
            {
                vanillaSkipWalkSounds = SkinData.SkipAndWalkAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            vanillaBreatheSound = SkinData.BreatheAudioAction.Apply(ref girl.breathingSFX);
            vanillaHeartbeatSound = SkinData.HeartBeatAudioAction.ApplyToSource(girl.heartbeatMusic);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig") } }
            );

            eventHandler = new GhostGirlDisappearEventHandler(skinnedMeshReplacement, activeAttachments);

            if(skinnedMeshReplacement!=null)
            {
                foreach (Renderer renderer in skinnedMeshReplacement.GetComponentsInChildren<Renderer>())
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("EnemiesNotRendered");
                }
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(girl, eventHandler);
        }

        public override void Remove(GameObject enemy)
        {
            DressGirlAI girl = enemy.GetComponent<DressGirlAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANCHOR_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(girl, eventHandler);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.LeftEyeMaterialAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftEyeMaterial);
            SkinData.RightEyeMaterialAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightEyeMaterial);
            SkinData.LeftEyeMeshAction.Remove(enemy.transform.Find(LEFT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftEyeMesh);
            SkinData.RightEyeMeshAction.Remove(enemy.transform.Find(RIGHT_EYE_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightEyeMesh);
            SkinData.HauntingCuesAudioListAction.Remove(ref girl.appearStaringSFX, vanillaHauntingSounds);
            if(audioAnimEvents!=null)
            {
                SkinData.SkipAndWalkAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaSkipWalkSounds);
            }
            SkinData.BreatheAudioAction.Remove(ref girl.breathingSFX, vanillaBreatheSound);
            SkinData.HeartBeatAudioAction.RemoveFromSource(girl.heartbeatMusic, vanillaHeartbeatSound);

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

    class GhostGirlDisappearEventHandler : GhostGirlEventHandler
    {

        private GameObject ModdedMeshGO { get; }
        private List<GameObject> ActiveAttachments { get; }

        internal GhostGirlDisappearEventHandler(GameObject ghostGirl, List<GameObject> activeAttachments)
        {
            ModdedMeshGO = ghostGirl;
            ActiveAttachments = activeAttachments;
        }

        public void OnHide(DressGirlAI ghostGirl)
        {
            if (EnemySkinKit.LogLevelSetting >= LogLevel.INFO) EnemySkinKit.SkinKitLogger.LogInfo("Got Ghost Girl Hide Event");
            if (ModdedMeshGO != null)
            {
                Renderer[] renderers = ModdedMeshGO.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("EnemiesNotRendered");
                }
            }
            foreach(GameObject attachment in ActiveAttachments)
            {
                Renderer[] renderers = attachment.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.gameObject.layer = LayerMask.NameToLayer("EnemiesNotRendered");
                }
            }
        }

        public void OnShow(DressGirlAI ghostGirl)
        {
            if (ghostGirl.hauntingLocalPlayer)
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.INFO) EnemySkinKit.SkinKitLogger.LogInfo("Got Ghost Girl Show Event");
                if (ModdedMeshGO != null)
                {
                    Renderer[] renderers = ModdedMeshGO.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                    }
                }
                foreach (GameObject attachment in ActiveAttachments)
                {
                    Renderer[] renderers = attachment.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                    }
                }
            }
        }
    }
}