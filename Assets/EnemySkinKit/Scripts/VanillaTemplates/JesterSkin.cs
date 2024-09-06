using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "JesterSkin", menuName = "EnemySkinKit/Skins/JesterSkin", order = 11)]
    public class JesterSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        protected MaterialAction skullMaterialAction;
        [SerializeField]
        protected MaterialAction jawMaterialAction;
        [SerializeField]
        protected MaterialAction lidMaterialAction;
        [SerializeField]
        protected MaterialAction crankMaterialAction;
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        protected StaticMeshAction skullLOD0Action;
        [SerializeField]
        protected StaticMeshAction skullLOD1Action;
        [SerializeField]
        protected StaticMeshAction skullLOD2Action;

        [SerializeField]
        protected StaticMeshAction jawLOD0Action;
        [SerializeField]
        protected StaticMeshAction jawLOD1Action;
        [SerializeField]
        protected StaticMeshAction lidMeshAction;
        [SerializeField]
        protected StaticMeshAction crankMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction popGoesTheWeaselMusicAudioAction;
        [SerializeField]
        protected AudioAction popUpAudioAction;
        [SerializeField]
        protected AudioAction screamingAudioAction;
        [SerializeField]
        protected AudioAction killPlayerAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction footstepAudioAction;
        [SerializeField]
        protected AudioListAction crankAudioListAction;
        [SerializeField]
        protected AudioListAction stompAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction SkullMaterialAction => skullMaterialAction;
        public MaterialAction JawMaterialAction => jawMaterialAction;
        public MaterialAction LidMaterialAction => lidMaterialAction;
        public MaterialAction CrankMaterialAction => crankMaterialAction;
        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction SkullLOD0Action => skullLOD0Action;
        public StaticMeshAction SkullLOD1Action => skullLOD1Action;
        public StaticMeshAction SkullLOD2Action => skullLOD2Action;
        public StaticMeshAction JawLOD0Action => jawLOD0Action;
        public StaticMeshAction JawLOD1Action => jawLOD1Action;
        public StaticMeshAction LidMeshAction => lidMeshAction;
        public StaticMeshAction CrankMeshAction => crankMeshAction;
        public AudioAction PopGoesTheWeaselMusicAudioAction => popGoesTheWeaselMusicAudioAction;
        public AudioAction PopUpAudioAction => popUpAudioAction;
        public AudioAction ScreamingAudioAction => screamingAudioAction;
        public AudioAction KillPlayerAudioAction => killPlayerAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction FootstepAudioAction => footstepAudioAction;
        public AudioListAction CrankAudioListAction => crankAudioListAction;
        public AudioListAction StompAudioListAction => stompAudioListAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.JESTER_ID;

        public override Skinner CreateSkinner()
        {
            return new JesterSkinner(this);
        }
    }
}