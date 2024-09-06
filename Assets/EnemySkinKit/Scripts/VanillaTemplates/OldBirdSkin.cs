using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "OldBirdSkin", menuName = "EnemySkinKit/Skins/OldBirdSkin", order = 17)]
    public class OldBirdSkin : BaseNestSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction defaultMaterialAction;
        [SerializeField]
        protected MaterialAction spotlightActiveMaterialAction;
        [SerializeField]
        protected MaterialAction lightMaterialAction;
        [Space(10)]

        [Header("Spotlight Color")]
        [SerializeField]
        protected ColorAction lightColorAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        protected StaticMeshAction searchLightMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction brainwashingAudioListAction;
        [SerializeField]
        protected AudioListAction stompAudioListAction;
        [SerializeField]
        protected AudioListAction shootGunAudioListAction;
        [SerializeField]
        protected AudioListAction explosionAudioListAction;
        [SerializeField]
        protected AudioAction spotlightActivateAudioAction;
        [SerializeField]
        protected AudioAction spotlightDectivateAudioAction;
        [SerializeField]
        protected AudioAction spotlightFlickerAudioAction;
        [SerializeField]
        protected AudioAction blowtorchAudioAction;
        [SerializeField]
        protected AudioAction alarmAudioAction;
        [SerializeField]
        protected AudioAction thrusterStartAudioAction;
        [SerializeField]
        protected AudioAction thrusterCloseAudioAction;
        [SerializeField]
        protected AudioAction thrusterFarAudioAction;
        [SerializeField]
        protected AudioAction engineHumAudioAction;
        [SerializeField]
        protected AudioAction chargeAudioAction;
        [SerializeField]
        protected AudioAction wakeAudioAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction thrusterFireMaterialAction;
        [SerializeField]
        protected ParticleSystemAction stompShockwaveParticleAction;
        [SerializeField]
        protected MaterialAction stompShockwaveMaterialAction;
        [SerializeField]
        protected ParticleSystemAction smokeTrailParticleAction;
        [SerializeField]
        protected MaterialAction smokeTrailMaterialAction;
        [SerializeField]
        protected ParticleSystemAction landShockwaveParticleAction;
        [SerializeField]
        protected MaterialAction landShockwaveMaterialAction;
        [SerializeField]
        protected ParticleSystemAction chargeParticleAction;
        [SerializeField]
        protected MaterialAction chargeMaterialAction;
        [SerializeField]
        protected ParticleSystemAction chargeBlastParticleAction;
        [SerializeField]
        protected MaterialAction chargeBlastMaterialAction;
        [SerializeField]
        protected ParticleSystemAction chargeFireParticleAction;
        [SerializeField]
        protected MaterialAction chargeFireMaterialAction;
        [SerializeField]
        protected ParticleSystemAction chargeSmokeParticleAction;
        [SerializeField]
        protected MaterialAction chargeSmokeMaterialAction;
        [SerializeField]
        protected ParticleSystemAction chargeFlashParticleAction;
        [SerializeField]
        protected MaterialAction chargeFlashMaterialAction;
        [SerializeField]
        protected ParticleSystemAction blueFlameParticleAction;
        [SerializeField]
        protected MaterialAction blueFlameMaterialAction;
        [SerializeField]
        protected ParticleSystemAction redFlameParticleAction;
        [SerializeField]
        protected MaterialAction redFlameMaterialAction;
        [SerializeField]
        protected ParticleSystemAction muzzleFlashParticleAction;
        [SerializeField]
        protected MaterialAction muzzleFlashMaterialAction;

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;
        [Space(10)]

        [Header("Custom Nest")]
        [SerializeField]
        [Tooltip("Set this flag to use the fields below on the Old Bird's \"nest\" prefab. If not set, it will use the same values as the active old bird.")]
        protected bool useCustomNestActions;
        [SerializeField]
        protected SkinnedMeshAction nestBodyMeshAction;
        [SerializeField]
        protected MaterialAction nestBodyMaterialAction;
        [SerializeField]
        protected ArmatureAttachment[] nestAttachments;

        //Materials
        public MaterialAction DefaultMaterialAction => defaultMaterialAction;
        public MaterialAction SpotlightActiveMaterialAction => spotlightActiveMaterialAction;
        public MaterialAction LightMaterialAction => lightMaterialAction;
        public ColorAction LightColorAction => lightColorAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction SearchLightMeshAction => searchLightMeshAction;
        public AudioListAction BrainwashingAudioListAction => brainwashingAudioListAction;
        public AudioListAction StompAudioListAction => stompAudioListAction;
        public AudioListAction ShootGunAudioListAction => shootGunAudioListAction;
        public AudioListAction ExplosionAudioListAction => explosionAudioListAction;
        public AudioAction SpotlightActivateAudioAction => spotlightActivateAudioAction;
        public AudioAction SpotlightDectivateAudioAction => spotlightDectivateAudioAction;
        public AudioAction SpotlightFlickerAudioAction => spotlightFlickerAudioAction;
        public AudioAction BlowtorchAudioAction => blowtorchAudioAction;
        public AudioAction AlarmAudioAction => alarmAudioAction;
        public AudioAction ThrusterStartAudioAction => thrusterStartAudioAction;
        public AudioAction ThrusterCloseAudioAction => thrusterCloseAudioAction;
        public AudioAction ThrusterFarAudioAction => thrusterFarAudioAction;
        public AudioAction EngineHumAudioAction => engineHumAudioAction;
        public AudioAction ChargeAudioAction => chargeAudioAction;
        public AudioAction WakeAudioAction => wakeAudioAction;
        public MaterialAction ThrusterFireMaterialAction => thrusterFireMaterialAction;
        public ParticleSystemAction StompShockwaveParticleAction => stompShockwaveParticleAction;
        public MaterialAction StompShockwaveMaterialAction => stompShockwaveMaterialAction;
        public ParticleSystemAction SmokeTrailParticleAction => smokeTrailParticleAction;
        public MaterialAction SmokeTrailMaterialAction => smokeTrailMaterialAction;
        public ParticleSystemAction LandShockwaveParticleAction => landShockwaveParticleAction;
        public MaterialAction LandShockwaveMaterialAction => landShockwaveMaterialAction;
        public ParticleSystemAction ChargeParticleAction => chargeParticleAction;
        public MaterialAction ChargeMaterialAction => chargeMaterialAction;
        public ParticleSystemAction ChargeBlastParticleAction => chargeBlastParticleAction;
        public MaterialAction ChargeBlastMaterialAction => chargeBlastMaterialAction;
        public ParticleSystemAction ChargeFireParticleAction => chargeFireParticleAction;
        public MaterialAction ChargeFireMaterialAction => chargeFireMaterialAction;
        public ParticleSystemAction ChargeSmokeParticleAction => chargeSmokeParticleAction;
        public MaterialAction ChargeSmokeMaterialAction => chargeSmokeMaterialAction;
        public ParticleSystemAction ChargeFlashParticleAction => chargeFlashParticleAction;
        public MaterialAction ChargeFlashMaterialAction => chargeFlashMaterialAction;
        public ParticleSystemAction BlueFlameParticleAction => blueFlameParticleAction;
        public MaterialAction BlueFlameMaterialAction => blueFlameMaterialAction;
        public ParticleSystemAction RedFlameParticleAction => redFlameParticleAction;
        public MaterialAction RedFlameMaterialAction => redFlameMaterialAction;
        public ParticleSystemAction MuzzleFlashParticleAction => muzzleFlashParticleAction;
        public MaterialAction MuzzleFlashMaterialAction => muzzleFlashMaterialAction;
        public ArmatureAttachment[] Attachments => attachments;
        public SkinnedMeshAction NestBodyMeshAction => useCustomNestActions ? nestBodyMeshAction : bodyMeshAction;
        public MaterialAction NestBodyMaterialAction => useCustomNestActions ? nestBodyMaterialAction : nestBodyMaterialAction;
        public ArmatureAttachment[] NestAttachments => useCustomNestActions ? nestAttachments : attachments;

        public override string EnemyId => EnemySkinRegistry.OLD_BIRD_ID;

        public override Skinner CreateNestSkinner()
        {
            return new OldBirdNestSkinner(this);
        }

        public override Skinner CreateSkinner()
        {
            return new OldBirdSkinner(this);
        }
    }
}