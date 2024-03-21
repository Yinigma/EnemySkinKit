using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BrackenSkin", menuName = "EnemySkinKit/Skins/BrackenSkin", order = 1)]
    public class BrackenSkin : BaseSkin
    {
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        //Static Meshes
        [SerializeField]
        private StaticMeshAction leftEyeMeshAction;
        [SerializeField]
        private StaticMeshAction rightEyeMeshAction;

        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leafMaterialAction;
        [SerializeField]
        private MaterialAction leftEyeMaterialAction;
        [SerializeField]
        private MaterialAction rightEyeMaterialAction;

        [SerializeField]
        private AudioAction angerAudioAction;
        [SerializeField]
        private AudioAction neckSnapAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.BRACKEN_ID;

        public override Skinner CreateSkinner()
        {
            return new BrackenSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                leafMaterialAction,
                bodyMaterialAction,
                leftEyeMaterialAction,
                rightEyeMaterialAction,
                bodyMeshAction,
                leftEyeMeshAction,
                rightEyeMeshAction,
                angerAudioAction,
                neckSnapAudioAction
            );
        }
    }
}
