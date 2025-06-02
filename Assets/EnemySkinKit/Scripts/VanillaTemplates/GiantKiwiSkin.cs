using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "GiantKiwiSkin", menuName = "EnemySkinKit/Skins/GiantKiwiSkin", order = 1)]
    public class GiantKiwiSkin : BaseSkin
    {
        [Header("Baby stuff (untested)")]
        [SerializeField]
        private SkinnedMeshAction eggBodyMeshAction;
        [SerializeField]
        private StaticMeshAction birdNestMeshAction;
        [SerializeField]
        private MaterialAction eggBodyMaterialAction;
        [SerializeField]
        private AudioAction eggScreamAudioAction;
        [SerializeField]
        private AudioAction eggScream3AudioAction;
        [SerializeField]
        private AudioAction eggPeepAudioAction;
        [SerializeField]
        private AudioAction eggBreakAudioAction;
        [SerializeField]
        private ArmatureAttachment[] eggAttachments;

        [Space(10)]

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
        private AudioListAction attackAudioListAction;
        [SerializeField]
        private AudioListAction screamAudioListAction;
        [SerializeField]
        private AudioListAction footstepsAudioListAction;
        [SerializeField]
        private AudioListAction footstepsBassAudioListAction;
        [SerializeField]
        private AudioAction squawkAudioAction;
        [SerializeField]
        private AudioAction peckTreeAudioAction;
        [SerializeField]
        private AudioAction breakAndEnterShipAudioAction;
        [SerializeField]
        private AudioAction shipAlarmAudioAction;
        [SerializeField]
        private AudioAction birdNestAudioAction;
        [SerializeField]
        private AudioAction deathAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        private ParticleSystemAction woodChipParticleAction;
        [SerializeField]
        private ParticleSystemAction rocksParticleAction;
        [SerializeField]
        private ParticleSystemAction runningParticleAction;
        [SerializeField]
        private ParticleSystemAction feathersParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] parentAttachments;

        // Egg
        public SkinnedMeshAction EggBodyMeshAction => eggBodyMeshAction;
        public StaticMeshAction BirdNestMeshAction => birdNestMeshAction;
        public MaterialAction EggBodyMaterialAction => eggBodyMaterialAction;
        public AudioAction EggScreamAudioAction => eggScreamAudioAction;
        public AudioAction EggScream3AudioAction => eggScream3AudioAction;
        public AudioAction EggPeepAudioAction => eggPeepAudioAction;
        public AudioAction EggBreakAudioAction => eggBreakAudioAction;
        public ArmatureAttachment[] EggAttachments => eggAttachments;

        // Parent
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public AudioListAction FootstepsBassAudioListAction => footstepsBassAudioListAction;
        public AudioListAction AttackAudioListAction => attackAudioListAction;
        public AudioListAction ScreamAudioListAction => screamAudioListAction;
        public AudioAction SquawkAudioAction => squawkAudioAction;
        public AudioAction PeckTreeAudioAction => peckTreeAudioAction;
        public AudioAction BreakAndEnterShipAudioAction => breakAndEnterShipAudioAction;
        public AudioAction ShipAlarmAudioAction => shipAlarmAudioAction;
        public AudioAction BirdNestAudioAction => birdNestAudioAction;
        public AudioAction DeathAudioAction => deathAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public ParticleSystemAction WoodChipParticleAction => woodChipParticleAction;
        public ParticleSystemAction RocksParticleAction => rocksParticleAction;
        public ParticleSystemAction RunningParticleAction => runningParticleAction;
        public ParticleSystemAction FeathersParticleAction => feathersParticleAction;
        public ArmatureAttachment[] ParentAttachments => parentAttachments;

        public override string EnemyId => EnemySkinRegistry.GIANTKIWI_ID;

        public override Skinner CreateSkinner()
        {
            return new GiantKiwiSkinner(this);
        }
    }
}