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
        private AudioListAction shrieksAudioListAction;
        [SerializeField]
        private AudioAction fallShriekAudioAction;
        [SerializeField]
        private AudioAction hitGroundAudioAction;
        [SerializeField]
        private AudioAction hitAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SNARE_FLEA_ID;

        public override Skinner CreateSkinner()
        {
            return new SnareFleaSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMaterialAction,
                bodyMeshAction,
                fallShriekAudioAction,
                hitGroundAudioAction,
                hitAudioAction,
                shrieksAudioListAction
            );
        }
    }
}