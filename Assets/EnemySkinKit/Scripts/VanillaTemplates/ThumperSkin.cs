using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ThumperSkin", menuName = "EnemySkinKit/Skins/ThumperSkin", order = 1)]
    public class ThumperSkin : BaseSkin
    {
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private MaterialAction bodyMaterialAction;

        [SerializeField]
        private AudioListAction longRoarsAudioListAction;
        [SerializeField]
        private AudioListAction hitsAudioAction;
        [SerializeField]
        private AudioListAction wallHitsAudioListAction;
        [SerializeField]
        private AudioAction shortRoarAudioAction;
        [SerializeField]
        private AudioAction eatPlayerAudioAction;
        [SerializeField]
        private AudioAction biteAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;


        public override string EnemyId => EnemySkinRegistry.THUMPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ThumperSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMaterialAction, 
                bodyMeshAction,
                shortRoarAudioAction,
                wallHitsAudioListAction,
                biteAudioAction,
                eatPlayerAudioAction,
                hitsAudioAction,
                longRoarsAudioListAction
            );
        }
    }
}