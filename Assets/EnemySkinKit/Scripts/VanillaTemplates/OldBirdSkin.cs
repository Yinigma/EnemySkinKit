using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "OldBirdSkin", menuName = "EnemySkinKit/Skins/OldBirdSkin", order = 17)]
    public class OldBirdSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction defaultMaterialAction;
        [SerializeField]
        private MaterialAction spotlightActiveMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction brainwashingAudioListAction;
        [SerializeField]
        private AudioListAction stompAudioListAction;
        [SerializeField]
        private AudioListAction shootGunAudioListAction;
        [SerializeField]
        private AudioListAction explosionAudioListAction;
        [SerializeField]
        private AudioAction spotlightActivateAudioAction;
        [SerializeField]
        private AudioAction spotlightDectivateAudioAction;
        [SerializeField]
        private AudioAction spotlightFlickerAudioAction;
        [SerializeField]
        private AudioAction blowtorchAudioAction;
        [SerializeField]
        private AudioAction alarmAudioAction;
        [SerializeField]
        private AudioAction thrusterStartAudioAction;
        [SerializeField]
        private AudioAction thrusterCloseAudioAction;
        [SerializeField]
        private AudioAction thrusterFarAudioAction;
        [SerializeField]
        private AudioAction engineHumAudioAction;
        [SerializeField]
        private AudioAction chargeAudioAction;
        [SerializeField]
        private AudioAction wakeAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.OLD_BIRD_ID;

        public override Skinner CreateSkinner()
        {
            return new OldBirdSkinner
            (
                attachments,
                defaultMaterialAction,
                spotlightActiveMaterialAction,
                bodyMeshAction,
                brainwashingAudioListAction,
                stompAudioListAction,
                shootGunAudioListAction,
                explosionAudioListAction,
                spotlightActivateAudioAction,
                spotlightDectivateAudioAction,
                spotlightFlickerAudioAction,
                blowtorchAudioAction,
                alarmAudioAction,
                thrusterStartAudioAction,
                thrusterCloseAudioAction,
                thrusterFarAudioAction,
                engineHumAudioAction,
                chargeAudioAction,
                wakeAudioAction
            );
        }
    }
}