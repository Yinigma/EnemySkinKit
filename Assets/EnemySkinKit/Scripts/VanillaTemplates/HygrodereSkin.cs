using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "HygrodereSkin", menuName = "EnemySkinKit/Skins/Hygrodere", order = 10)]
    public class HygrodereSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        protected MaterialAction slimeMaterialAction;
        [SerializeField]
        protected ColorAction slimeGradiantColorAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        protected SkinnedMeshAction slimeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction agitatedAudioAction;
        [SerializeField]
        protected AudioAction jiggleAudioAction;
        [SerializeField]
        protected AudioAction hitAudioAction;
        [SerializeField]
        protected AudioAction killPlayerAudioAction;
        [SerializeField]
        protected AudioAction idleAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction SlimeMaterialAction => slimeMaterialAction;
        public ColorAction SlimeGradiantColorAction => slimeGradiantColorAction;
        public SkinnedMeshAction SlimeMeshAction => slimeMeshAction;
        public AudioAction AgitatedAudioAction => agitatedAudioAction;
        public AudioAction JiggleAudioAction => jiggleAudioAction;
        public AudioAction HitAudioAction => hitAudioAction;
        public AudioAction KillPlayerAudioAction => killPlayerAudioAction;
        public AudioAction IdleAudioAction => idleAudioAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.HYGRODERE_ID;

        public override Skinner CreateSkinner()
        {
            return new HygrodereSkinner(this);
        }
    }
}