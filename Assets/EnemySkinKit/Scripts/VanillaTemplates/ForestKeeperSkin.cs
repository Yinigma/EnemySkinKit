using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ForestKeeperSkin", menuName = "EnemySkinKit/Skins/ForestKeeper", order = 7)]
    public class ForestKeeperSkin : BaseSkin
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
        private AudioAction stunAudioAction;
        [SerializeField]
        private AudioAction roarAudioAction;
        [SerializeField]
        private AudioAction eatPlayerAudioAction;
        [SerializeField]
        private AudioAction fallAudioAction;
        [SerializeField]
        private AudioAction deathCryAudioAction;
        [SerializeField]
        private AudioListAction stompAudioListAction;
        [SerializeField]
        private AudioListAction rumbleAudioListAction;
        [SerializeField]
        private AudioAction burnAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.FOREST_KEEPER_ID;

        public override Skinner CreateSkinner()
        {
            return new ForestKeeperSkinner
            (
                attachments, 
                bodyMaterialAction, 
                bodyMeshAction,
                stunAudioAction,
                roarAudioAction,
                eatPlayerAudioAction,
                fallAudioAction,
                deathCryAudioAction,
                stompAudioListAction,
                rumbleAudioListAction,
                burnAudioAction
            );
        }
    }

}
