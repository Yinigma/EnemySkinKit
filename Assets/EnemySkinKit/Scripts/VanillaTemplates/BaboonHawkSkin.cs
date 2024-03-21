using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BaboonHawkSkin", menuName = "EnemySkinKit/Skins/BaboonHawkSkin", order = 1)]
    public class BaboonHawkSkin : BaseSkin
    {
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private AudioListAction screamAudioListAction;

        [SerializeField]
        private AudioListAction laughAudioListAction;

        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.BABOON_HAWK_ID;

        public override Skinner CreateSkinner()
        {
            return new BaboonHawkSkinner(muteEffects, muteVoice, attachments, bodyMaterialAction, bodyMeshAction, screamAudioListAction, laughAudioListAction);
        }
    }
}