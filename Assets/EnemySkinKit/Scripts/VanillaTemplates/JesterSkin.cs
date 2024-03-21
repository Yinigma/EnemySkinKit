using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "JesterSkin", menuName = "EnemySkinKit/Skins/JesterSkin", order = 1)]
    public class JesterSkin : BaseSkin
    {
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

        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private AudioAction popGoesTheWeaselMusicAudioAction;
        [SerializeField]
        private AudioAction popUpAudioAction;
        [SerializeField]
        private AudioAction screamingAudioAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.JESTER_ID;

        public override Skinner CreateSkinner()
        {
            return new JesterSkinner
            (
                muteEffects, 
                muteVoice,
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
                killPlayerAudioAction
            );
        }
    }
}