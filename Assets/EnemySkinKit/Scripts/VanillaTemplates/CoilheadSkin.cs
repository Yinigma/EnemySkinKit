using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "CoilheadSkin", menuName = "EnemySkinKit/Skins/CoilheadSkin", order = 1)]
    public class CoilheadSkin : BaseSkin
    {
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        //Static Meshes
        [SerializeField]
        private StaticMeshAction headMeshAction;

        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction rustMaterialAction;
        [SerializeField]
        private MaterialAction headMaterialAction;

        [SerializeField]
        private AudioListAction springNoisesAudioListAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.COILHEAD_ID;

        public override Skinner CreateSkinner()
        {
            return new CoilHeadSkinner(muteEffects, muteVoice, attachments, bodyMeshAction, headMeshAction, bodyMaterialAction, rustMaterialAction, headMaterialAction, springNoisesAudioListAction);
        }
    }
}