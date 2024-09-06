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
        protected MaterialAction rustMaterialAction;
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction headMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        protected StaticMeshAction headMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction springNoisesAudioListAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction RustMaterialAction => rustMaterialAction;
        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction HeadMaterialAction => headMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction HeadMeshAction => headMeshAction;
        public AudioListAction SpringNoisesAudioListAction => springNoisesAudioListAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.COILHEAD_ID;

        public override Skinner CreateSkinner()
        {
            return new CoilHeadSkinner(this);
        }
    }
}