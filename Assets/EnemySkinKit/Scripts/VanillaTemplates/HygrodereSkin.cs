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
        private MaterialAction slimeMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction slimeMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioAction agitatedAudioAction;
        [SerializeField]
        private AudioAction jiggleAudioAction;
        [SerializeField]
        private AudioAction hitAudioAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private AudioAction idleAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.HYGRODERE_ID;

        public override Skinner CreateSkinner()
        {
            return new HygrodereSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                slimeMaterialAction, 
                slimeMeshAction,
                agitatedAudioAction,
                jiggleAudioAction,
                hitAudioAction,
                killPlayerAudioAction,
                idleAudioAction
            );
        }
    }
}