using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "NutcrackerSkin", menuName = "EnemySkinKit/Skins/NutcrackerSkin", order = 12)]
    public class NutcrackerSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction torsoTurningAudioListAction;
        [SerializeField]
        private AudioAction aimAudioAction;
        [SerializeField]
        private AudioAction kickAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.NUTCRACKER_ID;

        public override Skinner CreateSkinner()
        {
            return new NutcrackerSkinner(muteEffects, muteVoice, attachments, bodyMaterialAction, bodyMeshAction, torsoTurningAudioListAction, aimAudioAction, kickAudioAction);
        }
    }
}