using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "CoilheadSkin", menuName = "EnemySkinKit/Skins/CoilheadSkin", order = 4)]
    public class CoilheadSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction rustMaterialAction;
        [SerializeField]
        private MaterialAction headMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        private StaticMeshAction headMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction springNoisesAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.COILHEAD_ID;

        public override Skinner CreateSkinner()
        {
            return new CoilHeadSkinner(muteEffects, muteVoice, attachments, bodyMeshAction, headMeshAction, bodyMaterialAction, rustMaterialAction, headMaterialAction, springNoisesAudioListAction);
        }
    }
}