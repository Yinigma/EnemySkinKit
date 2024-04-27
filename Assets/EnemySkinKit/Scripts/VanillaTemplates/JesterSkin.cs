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
        private MaterialAction skullMaterialAction;
        [SerializeField]
        private MaterialAction jawMaterialAction;
        [SerializeField]
        private MaterialAction lidMaterialAction;
        [SerializeField]
        private MaterialAction crankMaterialAction;
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private StaticMeshAction skullLOD0Action;
        [SerializeField]
        private StaticMeshAction skullLOD1Action;
        [SerializeField]
        private StaticMeshAction skullLOD2Action;

        [SerializeField]
        private StaticMeshAction jawLOD0Action;
        [SerializeField]
        private StaticMeshAction jawLOD1Action;
        [SerializeField]
        private StaticMeshAction lidMeshAction;
        [SerializeField]
        private StaticMeshAction crankMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioAction popGoesTheWeaselMusicAudioAction;
        [SerializeField]
        private AudioAction popUpAudioAction;
        [SerializeField]
        private AudioAction screamingAudioAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction footstepAudioAction;
        [SerializeField]
        private AudioListAction crankAudioListAction;
        [SerializeField]
        private AudioListAction stompAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.JESTER_ID;

        public override Skinner CreateSkinner()
        {
            return new JesterSkinner
            (
                attachments,
                skullMaterialAction,
                jawMaterialAction,
                lidMaterialAction,
                crankMaterialAction,
                bodyMaterialAction,
                skullLOD0Action,
                skullLOD1Action,
                skullLOD2Action,
                jawLOD0Action,
                jawLOD1Action,
                lidMeshAction,
                crankMeshAction,
                bodyMeshAction,
                popGoesTheWeaselMusicAudioAction,
                popUpAudioAction,
                screamingAudioAction,
                killPlayerAudioAction,
                hitBodyAudioAction,
                footstepAudioAction,
                crankAudioListAction,
                stompAudioListAction
            );
        }
    }
}