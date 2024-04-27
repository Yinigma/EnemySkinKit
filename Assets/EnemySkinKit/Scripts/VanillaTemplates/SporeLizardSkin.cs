using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SporeLizardSkin", menuName = "EnemySkinKit/Skins/SporeLizardSkin", order = 14)]
    public class SporeLizardSkin : BaseSkin
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
        private AudioListAction frightenedAudioListAction;
        [SerializeField]
        private AudioAction stompAudioAction;
        [SerializeField]
        private AudioAction angryAudioAction;
        [SerializeField]
        private AudioAction nervousMumbleAudioAction;
        [SerializeField]
        private AudioAction puffAudioAction;
        [SerializeField]
        private AudioAction rattleTailAudioAction;
        [SerializeField]
        private AudioAction biteAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioListAction footstepsAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SPORE_LIZARD_ID;

        public override Skinner CreateSkinner()
        {
            return new SporeLizardSkinner
            (
                attachments,
                bodyMaterialAction, 
                bodyMeshAction,
                frightenedAudioListAction,
                stompAudioAction,
                angryAudioAction,
                puffAudioAction,
                nervousMumbleAudioAction,
                rattleTailAudioAction,
                biteAudioAction,
                hitBodyAudioAction,
                footstepsAudioListAction
            );
        }
    }
}