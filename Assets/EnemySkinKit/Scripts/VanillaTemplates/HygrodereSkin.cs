using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "HygrodereSkin", menuName = "EnemySkinKit/Skins/Hygrodere", order = 1)]
    public class HygrodereSkin : BaseSkin
    {
        [SerializeField]
        private MaterialAction slimeMaterialAction;

        [SerializeField]
        private SkinnedMeshAction slimeMeshAction;

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