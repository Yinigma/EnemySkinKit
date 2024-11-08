using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BaboonHawkSkin", menuName = "EnemySkinKit/Skins/BaboonHawkSkin", order = 1)]
    public class BaboonHawkSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction screamAudioListAction;
        [SerializeField]
        private AudioListAction laughAudioListAction;
        [SerializeField]
        private AudioAction intimidateAudioAction;
        [SerializeField]
        private AudioAction intimidateVoiceAction;
        [SerializeField]
        private AudioAction enterFightAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private AudioAction stabAudioAction;
        [SerializeField]
        private AudioAction deathAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioListAction footstepsAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        private MaterialAction bloodMaterialAction;
        [SerializeField]
        private ParticleSystemAction bloodParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioListAction ScreamAudioListAction => screamAudioListAction;
        public AudioListAction LaughAudioListAction => laughAudioListAction;
        public AudioAction IntimidateAudioAction => intimidateAudioAction;
        public AudioAction IntimidateVoiceAction => intimidateVoiceAction;
        public AudioAction EnterFightAction => enterFightAction;
        public AudioAction KillPlayerAudioAction => killPlayerAudioAction;
        public AudioAction StabAudioAction => stabAudioAction;
        public AudioAction DeathAudioAction => deathAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioListAction FootstepsAudioAction => footstepsAudioAction;
        public MaterialAction BloodMaterialAction => bloodMaterialAction;
        public ParticleSystemAction BloodParticleAction => bloodParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.BABOON_HAWK_ID;

        public override Skinner CreateSkinner()
        {
            return new BaboonHawkSkinner(this);
        }
    }
}