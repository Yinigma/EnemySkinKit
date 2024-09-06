using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "HoarderBugSkin", menuName = "EnemySkinKit/Skins/HoarderBug", order = 9)]
    public class HoarderBugSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction leftWingMaterialAction;
        [SerializeField]
        protected MaterialAction rightWingMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        protected StaticMeshAction leftWingMeshAction;
        [SerializeField]
        protected StaticMeshAction rightWingMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction chitterAudioListAction;
        [SerializeField]
        protected AudioListAction angryChirpsAudioListAction;
        [SerializeField]
        protected AudioAction beginAttackAudioAction;
        [SerializeField]
        protected AudioAction flyAudioAction;
        [SerializeField]
        protected AudioAction hitPlayerAudioAction;
        [SerializeField]
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction LeftWingMaterialAction => leftWingMaterialAction;
        public MaterialAction RightWingMaterialAction => rightWingMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction LeftWingMeshAction => leftWingMeshAction;
        public StaticMeshAction RightWingMeshAction => rightWingMeshAction;
        public AudioListAction ChitterAudioListAction => chitterAudioListAction;
        public AudioListAction AngryChirpsAudioListAction => angryChirpsAudioListAction;
        public AudioAction BeginAttackAudioAction => beginAttackAudioAction;
        public AudioAction FlyAudioAction => flyAudioAction;
        public AudioAction HitPlayerAudioAction => hitPlayerAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.HOARDER_BUG_ID;

        public override Skinner CreateSkinner()
        {
            return new HoarderBugSkinner(this);
        }
    }
}