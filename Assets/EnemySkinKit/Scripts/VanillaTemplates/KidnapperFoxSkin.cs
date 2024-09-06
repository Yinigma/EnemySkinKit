using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "KidnapperFoxSkin", menuName = "EnemySkinKit/Skins/KidnapperFoxSkin", order = 20)]
    public class KidnapperFoxSkin : BaseSkin
    {
        [Header("Materials")]
        //Textures
        [SerializeField]
        protected TextureAction diffuseTextureAction;
        [SerializeField]
        protected TextureAction furMaskTextureAction;
        //Materials
        [SerializeField]
        protected MaterialAction furMaterialAction;
        [SerializeField]
        protected MaterialAction teethMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioAction hitAudioAction;
        [SerializeField]
        protected AudioAction dieAudioAction;
        [SerializeField]
        protected AudioAction dragSnarlAudioAction;
        [SerializeField]
        protected AudioListAction growlAudioListAction;
        [SerializeField]
        protected AudioAction shootTongueAudioAction;
        [SerializeField]
        protected AudioAction tongueShootingAudioAction;
        [SerializeField]
        protected AudioAction killPlayerAudioAction;
        [SerializeField]
        protected AudioListAction nearCallAudioListAction;
        [SerializeField]
        protected AudioListAction farCallAudioListAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction bloodSpurtMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodSpurtParticleAction;
        [SerializeField]
        protected MaterialAction droolMaterialAction;
        [SerializeField]
        protected ParticleSystemAction droolParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public TextureAction DiffuseTextureAction => diffuseTextureAction;
        public TextureAction FurMaskTextureAction => furMaskTextureAction;
        public MaterialAction FurMaterialAction => furMaterialAction;
        public MaterialAction TeethMaterialAction => teethMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioAction HitAudioAction => hitAudioAction;
        public AudioAction DieAudioAction => dieAudioAction;
        public AudioAction DragSnarlAudioAction => dragSnarlAudioAction;
        public AudioListAction GrowlAudioListAction => growlAudioListAction;
        public AudioAction ShootTongueAudioAction => shootTongueAudioAction;
        public AudioAction TongueShootingAudioAction => tongueShootingAudioAction;
        public AudioAction KillPlayerAudioAction => killPlayerAudioAction;
        public AudioListAction NearCallAudioListAction => nearCallAudioListAction;
        public AudioListAction FarCallAudioListAction => farCallAudioListAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public MaterialAction BloodSpurtMaterialAction => bloodSpurtMaterialAction;
        public ParticleSystemAction BloodSpurtParticleAction => bloodSpurtParticleAction;
        public MaterialAction DroolMaterialAction => droolMaterialAction;
        public ParticleSystemAction DroolParticleAction => droolParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.KIDNAPPER_FOX_ID;

        public override Skinner CreateSkinner()
        {
            return new KidnapperFoxSkinner(this);
        }
    }
}