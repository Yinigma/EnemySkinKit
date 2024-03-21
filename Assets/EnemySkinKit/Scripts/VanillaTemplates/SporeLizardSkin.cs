using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "SporeLizardSkin", menuName = "EnemySkinKit/Skins/SporeLizardSkin", order = 1)]
    public class SporeLizardSkin : BaseSkin
    {
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private AudioListAction frightenedAudioListAction;
        [SerializeField]
        private AudioAction stompAudioAction;
        [SerializeField]
        private AudioAction angryAudioAction;
        [SerializeField]
        private AudioAction nervousMumbleAudioAction;
        [SerializeField]
        private AudioAction puffAudioAction;
        [SerializeField]
        private AudioAction rattleTailAudioAction;
        [SerializeField]
        private AudioAction biteAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SPORE_LIZARD_ID;

        public override Skinner CreateSkinner()
        {
            return new SporeLizardSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMaterialAction, 
                bodyMeshAction,
                frightenedAudioListAction,
                stompAudioAction,
                angryAudioAction,
                puffAudioAction,
                nervousMumbleAudioAction,
                rattleTailAudioAction,
                biteAudioAction
            );
        }
    }
}