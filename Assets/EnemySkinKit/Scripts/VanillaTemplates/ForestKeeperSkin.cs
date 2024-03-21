using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ForestKeeperSkin", menuName = "EnemySkinKit/Skins/ForestKeeper", order = 1)]
    public class ForestKeeperSkin : BaseSkin
    {
        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private AudioAction farWideAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.FOREST_KEEPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ForestKeeperSkinner(muteEffects, muteVoice, attachments, bodyMaterialAction, bodyMeshAction, farWideAudioAction);
        }
    }

}
