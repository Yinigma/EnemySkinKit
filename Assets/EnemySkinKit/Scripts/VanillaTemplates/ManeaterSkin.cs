using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ManeaterSkin", menuName = "EnemySkinKit/Skins/ManeaterSkin", order = 25)]
    public class ManeaterSkin : BaseSkin
    {
        [Header("Baby")]

        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction babyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction babyBodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction cryingAudioAction;
        [SerializeField]
        protected AudioAction squirmAudioAction;
        [SerializeField]
        protected AudioListAction scaredNoiseAudioListAction;
        [SerializeField]
        protected AudioListAction babyFootstepsAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction tearsMaterialAction;
        [SerializeField]
        protected ParticleSystemAction tearsParticleAction;
        [SerializeField]
        protected MaterialAction pukeMaterialAction;
        [SerializeField]
        protected ParticleSystemAction pukeParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] babyAttachments;
        [Space(10)]

        [Header("Adult")]

        [Header("Materials")]
        [SerializeField]
        protected MaterialAction adultMaterialAction;
        [SerializeField]
        protected MaterialAction deadPlayerFleshMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction adultBodyMeshAction;
        [SerializeField]
        protected StaticMeshAction deadPlayerFleshMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction growlAudioAction;
        [SerializeField]
        protected AudioAction cooldownAudioAction;
        [SerializeField]
        protected AudioAction transformAudioAction;
        [SerializeField]
        protected AudioAction biteAudioAction;
        [SerializeField]
        protected AudioAction adultWalkingAudioAction;
        [SerializeField]
        protected AudioAction clickingMandiblesAudioAction;
        [SerializeField]
        protected AudioAction buzzingAudioAction;
        [SerializeField]
        protected AudioAction leapScreamAudioAction;
        [SerializeField]
        protected AudioListAction fakeCryAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction bloodSpurtMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodSpurtParticleAction;
        [SerializeField]
        protected MaterialAction bloodSpurt2MaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodSpurt2ParticleAction;
        [SerializeField]
        protected MaterialAction transformationGooMaterialAction;
        [SerializeField]
        protected ParticleSystemAction transformationGooParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] adultAttachments;
        


        public MaterialAction BabyMaterialAction => babyMaterialAction;

        //Skinned Meshes
        public SkinnedMeshAction BabyBodyMeshAction => babyBodyMeshAction;
        public AudioAction CryingAudioAction => cryingAudioAction;
        public AudioAction SquirmAudioAction => squirmAudioAction;
        public AudioListAction ScaredNoiseAudioListAction => scaredNoiseAudioListAction;
        public AudioListAction BabyFootstepsAudioListAction => babyFootstepsAudioListAction;
        public MaterialAction TearsMaterialAction => tearsMaterialAction;
        public ParticleSystemAction TearsParticleAction => tearsParticleAction;
        public MaterialAction PukeMaterialAction => pukeMaterialAction;
        public ParticleSystemAction PukeParticleAction => pukeParticleAction;
        public MaterialAction AdultMaterialAction => adultMaterialAction;
        public MaterialAction DeadPlayerFleshMaterialAction => deadPlayerFleshMaterialAction;
        public SkinnedMeshAction AdultBodyMeshAction => adultBodyMeshAction;
        public StaticMeshAction DeadPlayerFleshMeshAction => deadPlayerFleshMeshAction;
        public AudioAction GrowlAudioAction => growlAudioAction;
        public AudioAction CooldownAudioAction => cooldownAudioAction;
        public AudioAction TransformAudioAction => transformAudioAction;
        public AudioAction BiteAudioAction => biteAudioAction;
        public AudioAction AdultWalkingAudioAction => adultWalkingAudioAction;
        public AudioAction ClickingMandiblesAudioAction => clickingMandiblesAudioAction;
        public AudioAction BuzzingAudioAction => buzzingAudioAction;
        public AudioAction LeapScreamAudioAction => leapScreamAudioAction;
        public AudioListAction FakeCryAudioListAction => fakeCryAudioListAction;
        public MaterialAction BloodSpurtMaterialAction => bloodSpurtMaterialAction;
        public ParticleSystemAction BloodSpurtParticleAction => bloodSpurtParticleAction;
        public MaterialAction BloodSpurt2MaterialAction => bloodSpurt2MaterialAction;
        public ParticleSystemAction BloodSpurt2ParticleAction => bloodSpurt2ParticleAction;
        public MaterialAction TransformationGooMaterialAction => transformationGooMaterialAction;
        public ParticleSystemAction TransformationGooParticleAction => transformationGooParticleAction;
        public ArmatureAttachment[] BabyAttachments => babyAttachments;
        public ArmatureAttachment[] AdultAttachments => adultAttachments;

        public override string EnemyId => EnemySkinRegistry.MANEATER_ID;

        public override Skinner CreateSkinner()
        {
            return new ManeaterSkinner(this);
        }
    }
}
