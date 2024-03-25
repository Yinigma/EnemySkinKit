using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ForestKeeperSkin", menuName = "EnemySkinKit/Skins/ForestKeeper", order = 7)]
    public class ForestKeeperSkin : BaseSkin
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
        private AudioAction farWideAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.FOREST_KEEPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ForestKeeperSkinner(muteEffects, muteVoice, attachments, bodyMaterialAction, bodyMeshAction, farWideAudioAction);
        }
    }

}
