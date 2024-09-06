using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ForestKeeperSkin", menuName = "EnemySkinKit/Skins/ForestKeeper", order = 7)]
    public class ForestKeeperSkin : BaseSkin
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
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioAction roarAudioAction;
        [SerializeField]
        protected AudioAction eatPlayerAudioAction;
        [SerializeField]
        protected AudioAction fallAudioAction;
        [SerializeField]
        protected AudioAction deathCryAudioAction;
        [SerializeField]
        protected AudioListAction stompAudioListAction;
        [SerializeField]
        protected AudioListAction rumbleAudioListAction;
        [SerializeField]
        protected AudioAction burnAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction bloodMaterialAction;
        [SerializeField]
        protected ParticleSystemAction bloodParticleAction;
        [SerializeField]
        protected MaterialAction smokeMaterialAction;
        [SerializeField]
        protected ParticleSystemAction smokeParticleAction;
        [SerializeField]
        protected MaterialAction fireMaterialAction;
        [SerializeField]
        protected ParticleSystemAction fireParticleAction;
        [SerializeField]
        protected MaterialAction flashMaterialAction;
        [SerializeField]
        protected ParticleSystemAction flashParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioAction RoarAudioAction => roarAudioAction;
        public AudioAction EatPlayerAudioAction => eatPlayerAudioAction;
        public AudioAction FallAudioAction => fallAudioAction;
        public AudioAction DeathCryAudioAction => deathCryAudioAction;
        public AudioListAction StompAudioListAction => stompAudioListAction;
        public AudioListAction RumbleAudioListAction => rumbleAudioListAction;
        public AudioAction BurnAudioAction => burnAudioAction;
        public MaterialAction BloodMaterialAction => bloodMaterialAction;
        public ParticleSystemAction BloodParticleAction => bloodParticleAction;
        public MaterialAction SmokeMaterialAction => smokeMaterialAction;
        public ParticleSystemAction SmokeParticleAction => smokeParticleAction;
        public MaterialAction FireMaterialAction => fireMaterialAction;
        public ParticleSystemAction FireParticleAction => fireParticleAction;
        public MaterialAction FlashMaterialAction => flashMaterialAction;
        public ParticleSystemAction FlashParticleAction => flashParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.FOREST_KEEPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ForestKeeperSkinner(this);
        }
    }

}
