using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SnareFleaSkin", menuName = "EnemySkinKit/Skins/SnareFleaSkin", order = 1)]
    public class SnareFleaSkin : BaseSkin
    {
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private AudioListAction shrieksAudioListAction;
        [SerializeField]
        private AudioAction fallShriekAudioAction;
        [SerializeField]
        private AudioAction hitGroundAudioAction;
        [SerializeField]
        private AudioAction hitAudioAction;
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