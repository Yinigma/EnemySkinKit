using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class HoarderBugSkinner : BaseSkinner
    {
        protected const string ANCHOR_PATH = "HoarderBugModel/AnimContainer";
        protected const string BODY_PATH = "HoarderBugModel/Cube";
        protected const string ORGANS_PATH = "HoarderBugModel/Cube.001";
        protected const string LEFT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/LeftWing";
        protected const string RIGHT_WING_PATH = "HoarderBugModel/AnimContainer/Armature/Abdomen/Chest/Head/RightWing";

        protected Material vanillaBodyMaterial;
        protected Material vanillaLeftWingMaterial;
        protected Material vanillaRightWingMaterial;

        protected Mesh vanillaLeftWingMesh;
        protected Mesh vanillaRightWingMesh;

        protected AudioClip[] vanillaChitterAudio;
        protected AudioClip[] vanillaAngryScreechAudio;
        protected AudioClip vanillaAngryVoiceAudio;
        protected AudioClip vanillaFlyAudio;
        protected AudioClip vanillaHitPlayerAudio;
        protected List<GameObject> activeAttachments;

        protected MaterialAction BodyMaterialAction { get; }
        protected MaterialAction LeftWingMaterialAction { get; }
        protected MaterialAction RightWingMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected StaticMeshAction LeftWingMeshAction { get; }
        protected StaticMeshAction RightWingMeshAction { get; }
        protected AudioListAction ChitterAudioAction { get; }
        protected AudioListAction AngryScreechAudioAction { get; }
        protected AudioAction AngryAudioAction { get; }
        protected AudioAction FlyAudioAction { get; }
        protected AudioAction HitPlayerAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        public HoarderBugSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            MaterialAction leftWingMaterialAction, 
            MaterialAction rightWingMaterialAction, 
            SkinnedMeshAction bodyMeshAction, 
            StaticMeshAction leftWingMeshAction, 
            StaticMeshAction rightWingMeshAction,
            AudioListAction chitterAudioAction,
            AudioListAction angryScreechAudioAction,
            AudioAction angryAudioAction,
            AudioAction flyAudioAction,
            AudioAction hitPlayerAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            LeftWingMaterialAction = leftWingMaterialAction;
            RightWingMaterialAction = rightWingMaterialAction;
            BodyMeshAction = bodyMeshAction;
            LeftWingMeshAction = leftWingMeshAction;
            RightWingMeshAction = rightWingMeshAction;
            ChitterAudioAction = chitterAudioAction;
            AngryScreechAudioAction = angryScreechAudioAction;
            AngryAudioAction = angryAudioAction;
            FlyAudioAction = flyAudioAction;
            HitPlayerAction = hitPlayerAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaLeftWingMaterial = LeftWingMaterialAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaRightWingMaterial = RightWingMaterialAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLeftWingMesh = LeftWingMeshAction.Apply(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaRightWingMesh = RightWingMeshAction.Apply(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaAngryScreechAudio = AngryScreechAudioAction.Apply(ref enemy.GetComponent<HoarderBugAI>().angryScreechSFX);
            vanillaChitterAudio = ChitterAudioAction.Apply(ref enemy.GetComponent<HoarderBugAI>().chitterSFX);
            vanillaAngryVoiceAudio = AngryAudioAction.Apply(ref enemy.GetComponent<HoarderBugAI>().angryVoiceSFX);
            vanillaFlyAudio = FlyAudioAction.Apply(ref enemy.GetComponent<HoarderBugAI>().bugFlySFX);
            vanillaHitPlayerAudio = HitPlayerAction.Apply(ref enemy.GetComponent<HoarderBugAI>().hitPlayerSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "Armature", enemy.transform.Find($"{ANCHOR_PATH}/Armature") } }
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(false);
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            LeftWingMaterialAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLeftWingMaterial);
            RightWingMaterialAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaRightWingMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            LeftWingMeshAction.Remove(enemy.transform.Find(RIGHT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLeftWingMesh);
            RightWingMeshAction.Remove(enemy.transform.Find(LEFT_WING_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaRightWingMesh);

            AngryScreechAudioAction.Remove(ref enemy.GetComponent<HoarderBugAI>().angryScreechSFX, vanillaAngryScreechAudio);
            ChitterAudioAction.Remove(ref enemy.GetComponent<HoarderBugAI>().chitterSFX, vanillaChitterAudio);
            AngryAudioAction.Remove(ref enemy.GetComponent<HoarderBugAI>().angryVoiceSFX, vanillaAngryVoiceAudio);
            FlyAudioAction.Remove(ref enemy.GetComponent<HoarderBugAI>().bugFlySFX, vanillaFlyAudio);
            HitPlayerAction.Remove(ref enemy.GetComponent<HoarderBugAI>().hitPlayerSFX, vanillaHitPlayerAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            enemy.transform.Find(ORGANS_PATH)?.gameObject.SetActive(true);
        }
    }
}