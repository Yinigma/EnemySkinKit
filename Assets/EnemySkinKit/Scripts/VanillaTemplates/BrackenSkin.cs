using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BrackenSkin", menuName = "EnemySkinKit/Skins/BrackenSkin", order = 2)]
    public class BrackenSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leafMaterialAction;
        [SerializeField]
        private MaterialAction leftEyeMaterialAction;
        [SerializeField]
        private MaterialAction rightEyeMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        private StaticMeshAction leftEyeMeshAction;
        [SerializeField]
        private StaticMeshAction rightEyeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioAction angerAudioAction;
        [SerializeField]
        private AudioAction neckSnapAudioAction;
        [SerializeField]
        private AudioAction foundAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction stunAudioAction;
        [SerializeField]
        private AudioListAction leafRustleAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.BRACKEN_ID;

        public override Skinner CreateSkinner()
        {
            return new BrackenSkinner
            (
                attachments,
                leafMaterialAction,
                bodyMaterialAction,
                leftEyeMaterialAction,
                rightEyeMaterialAction,
                bodyMeshAction,
                leftEyeMeshAction,
                rightEyeMeshAction,
                angerAudioAction,
                neckSnapAudioAction,
                foundAudioAction,
                hitBodyAudioAction,
                stunAudioAction,
                leafRustleAudioListAction
            );
        }
    }
}
