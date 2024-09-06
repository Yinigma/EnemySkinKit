using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "EarthLeviathanSkin", menuName = "EnemySkinKit/Skins/EarthLeviathan", order = 5)]
    public class EarthLeviathanSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction groundRumbleAudioListAction;
        [SerializeField]
        protected AudioListAction ambientRumbleAudioListAction;
        [SerializeField]
        protected AudioListAction roarAudioListAction;
        [SerializeField]
        protected AudioAction hitGroundAudioAction;
        [SerializeField]
        protected AudioAction emergeAudioAction;
        [Space(10)]
        
        [Header("Particles")]
        [SerializeField]
        protected MaterialAction preEmergeParticleMaterialAction;
        [SerializeField]
        protected ParticleSystemAction preEmergeParticleAction;
        [SerializeField]
        protected MaterialAction emergeParticleMaterialAction;
        [SerializeField]
        protected ParticleSystemAction emergeParticleAction;
        [SerializeField]
        protected MaterialAction emergeShockwaveParticleMaterialAction;
        [SerializeField]
        protected ParticleSystemAction emergeShockwaveParticleAction;
        [SerializeField]
        protected MaterialAction submergeParticleMaterialAction;
        [SerializeField]
        protected ParticleSystemAction submergeParticleAction;
        [SerializeField]
        protected MaterialAction submergeShockwaveParticleMaterialAction;
        [SerializeField]
        protected ParticleSystemAction submergeShockwaveParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioListAction GroundRumbleAudioListAction => groundRumbleAudioListAction;
        public AudioListAction AmbientRumbleAudioListAction => ambientRumbleAudioListAction;
        public AudioListAction RoarAudioListAction => roarAudioListAction;
        public AudioAction HitGroundAudioAction => hitGroundAudioAction;
        public AudioAction EmergeAudioAction => emergeAudioAction;
        public MaterialAction PreEmergeParticleMaterialAction => preEmergeParticleMaterialAction;
        public ParticleSystemAction PreEmergeParticleAction => preEmergeParticleAction;
        public MaterialAction EmergeParticleMaterialAction => emergeParticleMaterialAction;
        public ParticleSystemAction EmergeParticleAction => emergeParticleAction;
        public MaterialAction EmergeShockwaveParticleMaterialAction => emergeShockwaveParticleMaterialAction;
        public ParticleSystemAction EmergeShockwaveParticleAction => emergeShockwaveParticleAction;
        public MaterialAction SubmergeParticleMaterialAction => submergeParticleMaterialAction;
        public ParticleSystemAction SubmergeParticleAction => submergeParticleAction;
        public MaterialAction SubmergeShockwaveParticleMaterialAction => submergeShockwaveParticleMaterialAction;
        public ParticleSystemAction SubmergeShockwaveParticleAction => submergeShockwaveParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.EARTH_LEVIATHAN_ID;

        public override Skinner CreateSkinner()
        {
            return new EarthLeviathanSkinner(this);
        }
    }
}