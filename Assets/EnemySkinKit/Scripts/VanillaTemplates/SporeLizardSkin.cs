using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SporeLizardSkin", menuName = "EnemySkinKit/Skins/SporeLizardSkin", order = 14)]
    public class SporeLizardSkin : BaseSkin
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
        protected AudioListAction frightenedAudioListAction;
        [SerializeField]
        protected AudioAction stompAudioAction;
        [SerializeField]
        protected AudioAction angryAudioAction;
        [SerializeField]
        protected AudioAction nervousMumbleAudioAction;
        [SerializeField]
        protected AudioAction puffAudioAction;
        [SerializeField]
        protected AudioAction rattleTailAudioAction;
        [SerializeField]
        protected AudioAction biteAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioListAction FrightenedAudioListAction => frightenedAudioListAction;
        public AudioAction StompAudioAction => stompAudioAction;
        public AudioAction AngryAudioAction => angryAudioAction;
        public AudioAction NervousMumbleAudioAction => nervousMumbleAudioAction;
        public AudioAction PuffAudioAction => puffAudioAction;
        public AudioAction RattleTailAudioAction => rattleTailAudioAction;
        public AudioAction BiteAudioAction => biteAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.SPORE_LIZARD_ID;

        public override Skinner CreateSkinner()
        {
            return new SporeLizardSkinner(this);
        }
    }
}