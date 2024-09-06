using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "NutcrackerSkin", menuName = "EnemySkinKit/Skins/NutcrackerSkin", order = 12)]
    public class NutcrackerSkin : BaseSkin
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
        protected AudioAction torsoTurnAudioAction;
        [SerializeField]
        protected AudioListAction torsoFinishTurningAudioListAction;
        [SerializeField]
        protected AudioAction aimAudioAction;
        [SerializeField]
        protected AudioAction kickAudioAction;
        [SerializeField]
        protected AudioAction hitEyeAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction headPopUpAudioAction;
        [SerializeField]
        protected AudioAction reloadAudioAction;
        [SerializeField]
        protected AudioAction angryDrumsAudioAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [SerializeField]
        protected AudioListAction jointSqueaksAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction bloodSpurtMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodSpurtParticleAction;
        [SerializeField]
        protected MaterialAction bloodFountainMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodFountainParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioAction TorsoTurnAudioAction => torsoTurnAudioAction;
        public AudioListAction TorsoFinishTurningAudioListAction => torsoFinishTurningAudioListAction;
        public AudioAction AimAudioAction => aimAudioAction;
        public AudioAction KickAudioAction => kickAudioAction;
        public AudioAction HitEyeAudioAction => hitEyeAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction HeadPopUpAudioAction => headPopUpAudioAction;
        public AudioAction ReloadAudioAction => reloadAudioAction;
        public AudioAction AngryDrumsAudioAction => angryDrumsAudioAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public AudioListAction JointSqueaksAudioListAction => jointSqueaksAudioListAction;
        public MaterialAction BloodSpurtMaterialAction => bloodSpurtMaterialAction;
        public ParticleSystemAction BloodSpurtParticleAction => bloodSpurtParticleAction;
        public MaterialAction BloodFountainMaterialAction => bloodFountainMaterialAction;
        public ParticleSystemAction BloodFountainParticleAction => bloodFountainParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.NUTCRACKER_ID;

        public override Skinner CreateSkinner()
        {
            return new NutcrackerSkinner( this );
        }
    }
}