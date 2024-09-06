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
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction leftEyeMaterialAction;
        [SerializeField]
        protected MaterialAction rightEyeMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        protected StaticMeshAction leftEyeMeshAction;
        [SerializeField]
        protected StaticMeshAction rightEyeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction hauntingCuesAudioListAction;
        [SerializeField]
        protected AudioAction breatheAudioAction;
        [SerializeField]
        protected AudioListAction skipAndWalkAudioListAction;
        [SerializeField]
        protected AudioAction heartBeatAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction LeftEyeMaterialAction => leftEyeMaterialAction;
        public MaterialAction RightEyeMaterialAction => rightEyeMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction LeftEyeMeshAction => leftEyeMeshAction;
        public StaticMeshAction RightEyeMeshAction => rightEyeMeshAction;
        public AudioListAction HauntingCuesAudioListAction => hauntingCuesAudioListAction;
        public AudioAction BreatheAudioAction => breatheAudioAction;
        public AudioListAction SkipAndWalkAudioListAction => skipAndWalkAudioListAction;
        public AudioAction HeartBeatAudioAction => heartBeatAudioAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.GHOST_GIRL_ID;

        public override Skinner CreateSkinner()
        {
            return new GhostGirlSkinner(this);
        }
    }
}