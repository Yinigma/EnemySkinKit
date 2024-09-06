using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ManticoilSkin", menuName = "EnemySkinKit/Skins/ManticoilSkin", order = 24)]
    public class ManticoilSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]        
        protected SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]        
        protected AudioAction flyingAudioAction;
        [SerializeField]        
        protected AudioListAction screechAudioListAction;
        [SerializeField]        
        protected AudioListAction flapAudioListAction;
        [SerializeField]        
        protected AudioAction hitGroundAudioAction;
        [SerializeField]        
        protected AudioAction stunAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioAction FlyingAudioAction => flyingAudioAction;
        public AudioListAction ScreechAudioListAction => screechAudioListAction;
        public AudioListAction FlapAudioListAction => flapAudioListAction;
        public AudioAction HitGroundAudioAction => hitGroundAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.MANTICOIL_ID;

        public override Skinner CreateSkinner()
        {
            return new ManticoilSkinner(this);
        }
    }

}
