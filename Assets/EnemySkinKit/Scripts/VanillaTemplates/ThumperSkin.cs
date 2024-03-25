using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ThumperSkin", menuName = "EnemySkinKit/Skins/ThumperSkin", order = 15)]
    public class ThumperSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
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
        [Space(10)]

        [Header("Armature Attachments")]
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