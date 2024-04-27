using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "NutcrackerSkin", menuName = "EnemySkinKit/Skins/NutcrackerSkin", order = 12)]
    public class NutcrackerSkin : BaseSkin
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
        private AudioAction torsoTurnAudioAction;
        [SerializeField]
        private AudioListAction torsoFinishTurningAudioListAction;
        [SerializeField]
        private AudioAction aimAudioAction;
        [SerializeField]
        private AudioAction kickAudioAction;
        [SerializeField]
        private AudioAction hitEyeAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction headPopUpAudioAction;
        [SerializeField]
        private AudioAction reloadAudioAction;
        [SerializeField]
        private AudioAction angryDrumsAudioAction;
        [SerializeField]
        private AudioListAction footstepsAudioListAction;
        [SerializeField]
        private AudioListAction jointSqueaksAudioListAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.NUTCRACKER_ID;

        public override Skinner CreateSkinner()
        {
            return new NutcrackerSkinner
            (
                attachments, 
                bodyMaterialAction, 
                bodyMeshAction, 
                torsoTurnAudioAction,
                torsoFinishTurningAudioListAction,
                aimAudioAction, 
                kickAudioAction,
                headPopUpAudioAction,
                hitBodyAudioAction,
                hitEyeAudioAction,
                reloadAudioAction,
                angryDrumsAudioAction,
                jointSqueaksAudioListAction,
                footstepsAudioListAction
            );
        }
    }
}