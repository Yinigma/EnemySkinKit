using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SnareFleaSkin", menuName = "EnemySkinKit/Skins/SnareFleaSkin", order = 13)]
    public class SnareFleaSkin : BaseSkin
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
        protected AudioAction clingToCeilingAudioAction;
        [SerializeField]
        protected AudioAction crawlAudioAction;
        [SerializeField]
        protected AudioAction fallShriekAudioAction;
        [SerializeField]
        protected AudioAction hitGroundAudioAction;
        [SerializeField]
        protected AudioListAction shrieksAudioListAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction hitBody2AudioAction;
        [SerializeField]
        protected AudioAction clingToPlayerAudioAction;
        [SerializeField]
        protected AudioAction clingToLocalPlayerAudioAction;
        [SerializeField]
        protected AudioAction deathAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction silkMaterialAction;
        [SerializeField]
        protected ParticleSystemAction silkParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioAction ClingToCeilingAudioAction => clingToCeilingAudioAction;
        public AudioAction CrawlAudioAction => crawlAudioAction;
        public AudioAction FallShriekAudioAction => fallShriekAudioAction;
        public AudioAction HitGroundAudioAction => hitGroundAudioAction;
        public AudioListAction ShrieksAudioListAction => shrieksAudioListAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction HitBody2AudioAction => hitBody2AudioAction;
        public AudioAction ClingToPlayerAudioAction => clingToPlayerAudioAction;
        public AudioAction ClingToLocalPlayerAudioAction => clingToLocalPlayerAudioAction;
        public AudioAction DeathAudioAction => deathAudioAction;
        public MaterialAction SilkMaterialAction => silkMaterialAction;
        public ParticleSystemAction SilkParticleAction => silkParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.SNARE_FLEA_ID;

        public override Skinner CreateSkinner()
        {
            return new SnareFleaSkinner(this);
        }
    }
}