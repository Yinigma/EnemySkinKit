using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BarberSkin", menuName = "EnemySkinKit/Skins/BarberSkin", order = 19)]
    public class BarberSkin : BaseSkin
    {
        [Header("Options")]
        [SerializeField]
        [Tooltip("Whether or not modded materials fade at a distance like the vanilla one does")]
        private bool doFade;

        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction upperScissorsMaterialAction;
        [SerializeField]
        protected MaterialAction lowerScissorsMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        protected StaticMeshAction upperScissorsMeshAction;
        [SerializeField]
        protected StaticMeshAction lowerScissorsMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction snipAudioAction;
        [SerializeField]
        protected AudioAction drumRoll;
        [SerializeField]
        protected AudioListAction moveAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction UpperScissorsMaterialAction => upperScissorsMaterialAction;
        public MaterialAction LowerScissorsMaterialAction => lowerScissorsMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction UpperScissorsMeshAction => upperScissorsMeshAction;
        public StaticMeshAction LowerScissorsMeshAction => lowerScissorsMeshAction;
        public AudioAction SnipAudioAction => snipAudioAction;
        public AudioAction DrumRoll => drumRoll;
        public AudioListAction MoveAudioListAction => moveAudioListAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.BARBER_ID;

        public override Skinner CreateSkinner()
        {
            return new BarberSkinner(this);
        }
    }
}
