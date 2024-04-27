using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "BunkerSpiderSkin", menuName = "EnemySkinKit/Skins/BunkerSpider", order = 3)]
    public class BunkerSpiderSkin : BaseSkin
    {
        [Header("Materials")]
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction leftFangMaterialAction;
        [SerializeField]
        private MaterialAction rightFangMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        private StaticMeshAction leftFangMeshAction;
        [SerializeField]
        private StaticMeshAction rightFangMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction footstepsAction;
        [SerializeField]
        private AudioAction attackAudioAction;
        [SerializeField]
        private AudioAction spoolPlayerAudioAction;
        [SerializeField]
        private AudioAction hangPlayerAudioAction;
        [SerializeField]
        private AudioAction hitHissAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction stunAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.SPIDER_ID;

        public override Skinner CreateSkinner()
        {
            return new BunkerSpiderSkinner
            (
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
                hitHissAudioAction,
                hitBodyAudioAction,
                stunAudioAction
            );
        }
    }
}