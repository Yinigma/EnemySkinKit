using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BunkerSpiderSkin", menuName = "EnemySkinKit/Skins/BunkerSpider", order = 3)]
    public class BunkerSpiderSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction leftFangMaterialAction;
        [SerializeField]
        protected MaterialAction rightFangMaterialAction;
        [SerializeField]
        protected MaterialAction safetyTextMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        protected StaticMeshAction leftFangMeshAction;
        [SerializeField]
        protected StaticMeshAction rightFangMeshAction;
        [SerializeField]
        protected StaticMeshAction safetyTextMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction footstepsAction;
        [SerializeField]
        protected AudioAction attackAudioAction;
        [SerializeField]
        protected AudioAction spoolPlayerAudioAction;
        [SerializeField]
        protected AudioAction hangPlayerAudioAction;
        [SerializeField]
        protected AudioAction hitHissAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction stunAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction LeftFangMaterialAction => leftFangMaterialAction;
        public MaterialAction RightFangMaterialAction => rightFangMaterialAction;
        public MaterialAction SafetyTextMaterialAction => safetyTextMaterialAction;

        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction LeftFangMeshAction => leftFangMeshAction;
        public StaticMeshAction RightFangMeshAction => rightFangMeshAction;
        public StaticMeshAction SafetyTextMeshAction => safetyTextMeshAction;

        public AudioListAction FootstepsAction => footstepsAction;
        public AudioAction AttackAudioAction => attackAudioAction;
        public AudioAction SpoolPlayerAudioAction => spoolPlayerAudioAction;
        public AudioAction HangPlayerAudioAction => hangPlayerAudioAction;
        public AudioAction HitHissAudioAction => hitHissAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;

        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.SPIDER_ID;

        public override Skinner CreateSkinner()
        {
            return new BunkerSpiderSkinner(this);
        }
    }
}