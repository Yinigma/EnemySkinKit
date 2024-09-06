using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ButlerSkin", menuName = "EnemySkinKit/Skins/ButlerSkin", order = 16)]
    public class ButlerSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction broomMaterialAction;
        [SerializeField]
        protected MaterialAction teethMaterialAction;
        [SerializeField]
        protected MaterialAction hairMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        protected StaticMeshAction broomMeshAction;
        [SerializeField]
        protected StaticMeshAction teethMeshAction;
        [SerializeField]
        protected StaticMeshAction hairMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction footstepsAudioAction;
        [SerializeField]
        protected AudioListAction sweepsAudioAction;
        [SerializeField]
        protected AudioAction popReverbAudioAction;
        [SerializeField]
        protected AudioAction murderMusicAudioAction;
        [SerializeField]
        protected AudioAction defaultAmbienceAudioAction;
        [SerializeField]
        protected AudioAction buzzingAmbienceAudioAction;
        [SerializeField]
        protected AudioAction stabPlayerAudioAction;
        [SerializeField]
        protected AudioAction coatRustleAudioAction;
        [SerializeField]
        protected AudioAction brandishKnifeAudioAction;
        [SerializeField]
        protected AudioAction popAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction inflateAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction popMaterialAction;
        [SerializeField]
        protected ParticleSystemAction popParticleAction;
        [SerializeField]
        protected MaterialAction stabBloodMaterialAction;
        [SerializeField]
        protected ParticleSystemAction stabBloodParticleAction;
        [SerializeField]
        protected MaterialAction bloodSpurtMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodSpurtParticleAction;

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;
        [Space(10)]

        [Header("Hornets")]
        [SerializeField]
        protected StaticMeshAction hornetMeshAction;
        [SerializeField]
        protected TextureAction hornetTextureAction;
        [SerializeField]
        protected GameObject hornetReplacementPrefab;
        [SerializeField]
        protected AudioAction hornetBuzzAudioAction;

        //Materials
        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction BroomMaterialAction => broomMaterialAction;
        public MaterialAction TeethMaterialAction => teethMaterialAction;
        public MaterialAction HairMaterialAction => hairMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction BroomMeshAction => broomMeshAction;
        public StaticMeshAction TeethMeshAction => teethMeshAction;
        public StaticMeshAction HairMeshAction => hairMeshAction;
        public AudioListAction FootstepsAudioAction => footstepsAudioAction;
        public AudioListAction SweepsAudioAction => sweepsAudioAction;
        public AudioAction PopReverbAudioAction => popReverbAudioAction;
        public AudioAction MurderMusicAudioAction => murderMusicAudioAction;
        public AudioAction DefaultAmbienceAudioAction => defaultAmbienceAudioAction;
        public AudioAction BuzzingAmbienceAudioAction => buzzingAmbienceAudioAction;
        public AudioAction StabPlayerAudioAction => stabPlayerAudioAction;
        public AudioAction CoatRustleAudioAction => coatRustleAudioAction;
        public AudioAction BrandishKnifeAudioAction => brandishKnifeAudioAction;
        public AudioAction PopAudioAction => popAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction InflateAudioAction => inflateAudioAction;
        public MaterialAction PopMaterialAction => popMaterialAction;
        public ParticleSystemAction PopParticleAction => popParticleAction;
        public MaterialAction StabBloodMaterialAction => stabBloodMaterialAction;
        public ParticleSystemAction StabBloodParticleAction => stabBloodParticleAction;
        public MaterialAction BloodSpurtMaterialAction => bloodSpurtMaterialAction;
        public ParticleSystemAction BloodSpurtParticleAction => bloodSpurtParticleAction;
        public ArmatureAttachment[] Attachments => attachments;
        public StaticMeshAction HornetMeshAction => hornetMeshAction;
        public TextureAction HornetTextureAction => hornetTextureAction;
        public GameObject HornetReplacementPrefab => hornetReplacementPrefab;
        public AudioAction HornetBuzzAudioAction => hornetBuzzAudioAction;

        public override string EnemyId => EnemySkinRegistry.BUTLER_ID;

        public override Skinner CreateSkinner()
        {
            return new ButlerSkinner(this);
        }
    }
}
