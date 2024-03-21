using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BunkerSpiderSkin", menuName = "EnemySkinKit/Skins/BunkerSpider", order = 1)]
    public class BunkerSpiderSkin : BaseSkin
    {
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leftFangMaterialAction;
        [SerializeField]
        private MaterialAction rightFangMaterialAction;
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        private StaticMeshAction leftFangMeshAction;
        [SerializeField]
        private StaticMeshAction rightFangMeshAction;
        [SerializeField]
        private AudioListAction footstepsAction;
        [SerializeField]
        private AudioAction attackAudioAction;
        [SerializeField]
        private AudioAction spoolPlayerAudioAction;
        [SerializeField]
        private AudioAction hangPlayerAudioAction;
        [SerializeField]
        private AudioAction hitAudioAction;
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SPIDER_ID;

        public override Skinner CreateSkinner()
        {
            return new BunkerSpiderSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMaterialAction,
                leftFangMaterialAction,
                rightFangMaterialAction,
                bodyMeshAction,
                leftFangMeshAction,
                rightFangMeshAction,
                footstepsAction,
                attackAudioAction,
                spoolPlayerAudioAction,
                hangPlayerAudioAction,
                hitAudioAction
            );
        }
    }
}