using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "NutcrackerSkin", menuName = "EnemySkinKit/Skins/NutcrackerSkin", order = 1)]
    public class NutcrackerSkin : BaseSkin
    {
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private AudioListAction torsoTurningAudioListAction;
        [SerializeField]
        private AudioAction aimAudioAction;
        [SerializeField]
        private AudioAction kickAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.NUTCRACKER_ID;

        public override Skinner CreateSkinner()
        {
            return new NutcrackerSkinner(muteEffects, muteVoice, attachments, bodyMaterialAction, bodyMeshAction, torsoTurningAudioListAction, aimAudioAction, kickAudioAction);
        }
    }
}