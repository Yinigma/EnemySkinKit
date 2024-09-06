using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ThumperSkin", menuName = "EnemySkinKit/Skins/ThumperSkin", order = 15)]
    public class ThumperSkin : BaseSkin
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
        protected AudioListAction longRoarsAudioListAction;
        [SerializeField]
        protected AudioListAction hitsAudioAction;
        [SerializeField]
        protected AudioListAction wallHitsAudioListAction;
        [SerializeField]
        protected AudioAction shortRoarAudioAction;
        [SerializeField]
        protected AudioAction eatPlayerAudioAction;
        [SerializeField]
        protected AudioAction biteAudioAction;
        [SerializeField]
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioListAction stompAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioListAction LongRoarsAudioListAction => longRoarsAudioListAction;
        public AudioListAction HitsAudioAction => hitsAudioAction;
        public AudioListAction WallHitsAudioListAction => wallHitsAudioListAction;
        public AudioAction ShortRoarAudioAction => shortRoarAudioAction;
        public AudioAction EatPlayerAudioAction => eatPlayerAudioAction;
        public AudioAction BiteAudioAction => biteAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioListAction StompAudioListAction => stompAudioListAction;
        public ArmatureAttachment[] Attachments => attachments;


        public override string EnemyId => EnemySkinRegistry.THUMPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ThumperSkinner( this );
        }
    }
}