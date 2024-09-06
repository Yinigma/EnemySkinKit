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
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction leafMaterialAction;
        [SerializeField]
        protected MaterialAction leftEyeMaterialAction;
        [SerializeField]
        protected MaterialAction rightEyeMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        protected StaticMeshAction leftEyeMeshAction;
        [SerializeField]
        protected StaticMeshAction rightEyeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction angerAudioAction;
        [SerializeField]
        protected AudioAction neckSnapAudioAction;
        [SerializeField]
        protected AudioAction foundAudioAction;
        [SerializeField]
        protected AudioAction hitBodyAudioAction;
        [SerializeField]
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioListAction leafRustleAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction deathSporeMaterialAction;
        [SerializeField]
        protected ParticleSystemAction deathSporeParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction LeafMaterialAction => leafMaterialAction;
        public MaterialAction LeftEyeMaterialAction => leftEyeMaterialAction;
        public MaterialAction RightEyeMaterialAction => rightEyeMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction LeftEyeMeshAction => leftEyeMeshAction;
        public StaticMeshAction RightEyeMeshAction => rightEyeMeshAction;
        public AudioAction AngerAudioAction => angerAudioAction;
        public AudioAction NeckSnapAudioAction => neckSnapAudioAction;
        public AudioAction FoundAudioAction => foundAudioAction;
        public AudioAction HitBodyAudioAction => hitBodyAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioListAction LeafRustleAudioListAction => leafRustleAudioListAction;
        public MaterialAction DeathSporeMaterialAction => deathSporeMaterialAction;
        public ParticleSystemAction DeathSporeParticleAction => deathSporeParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.BRACKEN_ID;

        public override Skinner CreateSkinner()
        {
            return new BrackenSkinner(this);
        }
    }
}
