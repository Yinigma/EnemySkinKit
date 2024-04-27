using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SnareFleaSkin", menuName = "EnemySkinKit/Skins/SnareFleaSkin", order = 13)]
    public class SnareFleaSkin : BaseSkin
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
        private AudioAction clingToCeilingAudioAction;
        [SerializeField]
        private AudioAction crawlAudioAction;
        [SerializeField]
        private AudioAction fallShriekAudioAction;
        [SerializeField]
        private AudioAction hitGroundAudioAction;
        [SerializeField]
        private AudioListAction shrieksAudioListAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction hitBody2AudioAction;
        [SerializeField]
        private AudioAction clingToPlayerAudioAction;
        [SerializeField]
        private AudioAction clingToLocalPlayerAudioAction;
        [SerializeField]
        private AudioAction deathAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SNARE_FLEA_ID;

        public override Skinner CreateSkinner()
        {
            return new SnareFleaSkinner
            (
                attachments,
                bodyMaterialAction,
                bodyMeshAction,
                clingToCeilingAudioAction,
                crawlAudioAction,
                fallShriekAudioAction,
                hitGroundAudioAction,
                shrieksAudioListAction,
                clingToPlayerAudioAction,
                clingToLocalPlayerAudioAction,
                hitBodyAudioAction,
                hitBody2AudioAction,
                deathAudioAction
            );
        }
    }
}