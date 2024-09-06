using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "EyelessDogSkin", menuName = "EnemySkinKit/Skins/EyelessDog", order = 6)]
    public class EyelessDogSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [SerializeField]
        protected MaterialAction topTeethMaterialAction;
        [SerializeField]
        protected MaterialAction bottomTeethMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        protected StaticMeshAction topTeethMeshAction;
        [SerializeField]
        protected StaticMeshAction bottomTeethMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction screamAudioAction;
        [SerializeField]
        protected AudioAction killPlayerAudioAction;
        [SerializeField]
        protected AudioAction breathingAudioAction;
        [SerializeField]
        protected AudioAction stunAudioAction;
        [SerializeField]
        protected AudioAction growlAudioAction;
        [SerializeField]
        protected AudioAction chasingAudioAction;
        [SerializeField]
        protected AudioAction lungeAudioAction;
        [SerializeField]
        protected AudioListAction footstepsAudioListAction;
        [Space(10)]

        [Header("Particles")]
        [SerializeField]
        protected MaterialAction spawnDustMaterialAction;
        [SerializeField]
        protected ParticleSystemAction spawnDustParticleAction;
        [SerializeField]
        protected MaterialAction runDustMaterialAction;
        [SerializeField]
        protected ParticleSystemAction runDustParticleAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public MaterialAction TopTeethMaterialAction => topTeethMaterialAction;
        public MaterialAction BottomTeethMaterialAction => bottomTeethMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public StaticMeshAction TopTeethMeshAction => topTeethMeshAction;
        public StaticMeshAction BottomTeethMeshAction => bottomTeethMeshAction;
        public AudioAction ScreamAudioAction => screamAudioAction;
        public AudioAction KillPlayerAudioAction => killPlayerAudioAction;
        public AudioAction BreathingAudioAction => breathingAudioAction;
        public AudioAction StunAudioAction => stunAudioAction;
        public AudioAction GrowlAudioAction => growlAudioAction;
        public AudioAction ChasingAudioAction => chasingAudioAction;
        public AudioAction LungeAudioAction => lungeAudioAction;
        public AudioListAction FootstepsAudioListAction => footstepsAudioListAction;
        public MaterialAction SpawnDustMaterialAction => spawnDustMaterialAction;
        public ParticleSystemAction SpawnDustParticleAction => spawnDustParticleAction;
        public MaterialAction RunDustMaterialAction => runDustMaterialAction;
        public ParticleSystemAction RunDustParticleAction => runDustParticleAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.EYELESS_DOG_ID;

        public override Skinner CreateSkinner()
        {
            return new EyelessDogSkinner(this);
        }
    }
}