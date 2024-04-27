using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BaboonHawkSkin", menuName = "EnemySkinKit/Skins/BaboonHawkSkin", order = 1)]
    public class BaboonHawkSkin : BaseSkin
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
        private AudioListAction screamAudioListAction;
        [SerializeField]
        private AudioListAction laughAudioListAction;
        [SerializeField]
        private AudioAction intimidateAudioAction;
        [SerializeField]
        private AudioAction intimidateVoiceAction;
        [SerializeField]
        private AudioAction enterFightAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private AudioAction stabAudioAction;
        [SerializeField]
        private AudioAction deathAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioListAction footstepsAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.BABOON_HAWK_ID;

        public override Skinner CreateSkinner()
        {
            return new BaboonHawkSkinner
            (
                attachments, 
                bodyMaterialAction, 
                bodyMeshAction, 
                screamAudioListAction, 
                laughAudioListAction,
                intimidateAudioAction,
                intimidateVoiceAction,
                enterFightAction,
                killPlayerAudioAction,
                stabAudioAction,
                deathAudioAction,
                hitBodyAudioAction,
                footstepsAudioAction
            );
        }
    }
}