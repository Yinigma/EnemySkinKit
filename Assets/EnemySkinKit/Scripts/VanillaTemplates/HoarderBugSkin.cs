using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "HoarderBugSkin", menuName = "EnemySkinKit/Skins/HoarderBug", order = 1)]
    public class HoarderBugSkin : BaseSkin
    {
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leftWingMaterialAction;
        [SerializeField]
        private MaterialAction rightWingMaterialAction;

        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        [SerializeField]
        private StaticMeshAction leftWingMeshAction;
        [SerializeField]
        private StaticMeshAction rightWingMeshAction;

        [SerializeField]
        private AudioListAction chitterAudioListAction;
        [SerializeField]
        private AudioListAction angryChirpsAudioListAction;
        [SerializeField]
        private AudioAction beginAttackAudioAction;
        [SerializeField]
        private AudioAction flyAudioAction;
        [SerializeField]
        private AudioAction hitPlayerAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.HOARDER_BUG_ID;

        public override Skinner CreateSkinner()
        {
            return new HoarderBugSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMaterialAction,
                leftWingMaterialAction,
                rightWingMaterialAction,
                bodyMeshAction,
                leftWingMeshAction,
                rightWingMeshAction,
                chitterAudioListAction,
                angryChirpsAudioListAction,
                beginAttackAudioAction,
                flyAudioAction,
                hitPlayerAudioAction
            );
        }
    }
}