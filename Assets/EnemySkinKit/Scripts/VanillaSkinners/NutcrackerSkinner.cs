using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class NutcrackerSkinner : BaseSkinner
    {
        protected const string LOD0_PATH = "MeshContainer/LOD0";
        protected const string LOD1_PATH = "MeshContainer/LOD1";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer/metarig";
        protected Material vanillaBodyMaterial;
        protected AudioClip[] vanillaTorsoTurnAudio;
        protected AudioClip vanillaAimAudio;
        protected AudioClip vanillaKickAudio;
        protected List<GameObject> activeAttachments;
        

        protected MaterialAction BodyMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected AudioListAction TorsoTurnAudioAction { get; }
        protected AudioAction AimAudioAction { get; }
        protected AudioAction KickAudioAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        

        public NutcrackerSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioListAction torsoTurnAudioAction,
            AudioAction aimAudioAction,
            AudioAction kickAudioAction
        ) : base(muteSoundEffects, muteVoice)
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            TorsoTurnAudioAction = torsoTurnAudioAction;
            AimAudioAction = aimAudioAction;
            KickAudioAction = kickAudioAction;
            Attachments = attachments;
        }

        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTorsoTurnAudio = TorsoTurnAudioAction.Apply(ref enemy.GetComponent<NutcrackerEnemyAI>().torsoFinishTurningClips);
            vanillaAimAudio = AimAudioAction.Apply(ref enemy.GetComponent<NutcrackerEnemyAI>().aimSFX);
            vanillaKickAudio = KickAudioAction.Apply(ref enemy.GetComponent<NutcrackerEnemyAI>().kickSFX);
            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>() { { "spinecontainer", enemy.transform.Find($"{ ANCHOR_PATH }/spinecontainer") } }
            );
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            TorsoTurnAudioAction.Remove(ref enemy.GetComponent<NutcrackerEnemyAI>().torsoFinishTurningClips, vanillaTorsoTurnAudio);
            AimAudioAction.Remove(ref enemy.GetComponent<NutcrackerEnemyAI>().aimSFX, vanillaAimAudio);
            KickAudioAction.Remove(ref enemy.GetComponent<NutcrackerEnemyAI>().kickSFX, vanillaKickAudio);
            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }
    }

}