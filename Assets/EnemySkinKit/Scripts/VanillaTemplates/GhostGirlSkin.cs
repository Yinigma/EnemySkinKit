using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "GhostGirlSkin", menuName = "EnemySkinKit/Skins/GhostGirl", order = 8)]
    public class GhostGirlSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leftEyeMaterialAction;
        [SerializeField]
        private MaterialAction rightEyeMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        private StaticMeshAction leftEyeMeshAction;
        [SerializeField]
        private StaticMeshAction rightEyeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction hauntingCuesAudioListAction;
        [SerializeField]
        private AudioAction breatheAudioAction;
        [SerializeField]
        private AudioListAction skipAndWalkAudioListAction;
        [SerializeField]
        private AudioAction heartBeatAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.GHOST_GIRL_ID;

        public override Skinner CreateSkinner()
        {
            return new GhostGirlSkinner
            (
                attachments,
                bodyMaterialAction, 
                leftEyeMaterialAction, 
                rightEyeMaterialAction, 
                bodyMeshAction, 
                leftEyeMeshAction, 
                rightEyeMeshAction,
                hauntingCuesAudioListAction,
                breatheAudioAction,
                skipAndWalkAudioListAction,
                heartBeatAudioAction
            );
        }
    }
}