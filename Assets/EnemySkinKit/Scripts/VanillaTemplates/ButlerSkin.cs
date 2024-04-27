using UnityEngine;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "ButlerSkin", menuName = "EnemySkinKit/Skins/ButlerSkin", order = 16)]
    public class ButlerSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction broomMaterialAction;
        [SerializeField]
        private MaterialAction teethMaterialAction;
        [SerializeField]
        private MaterialAction hairMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [SerializeField]
        private StaticMeshAction broomMeshAction;
        [SerializeField]
        private StaticMeshAction teethMeshAction;
        [SerializeField]
        private StaticMeshAction hairMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction footstepsAudioAction;
        [SerializeField]
        private AudioListAction sweepsAudioAction;
        [SerializeField]
        private AudioAction popReverbAudioAction;
        [SerializeField]
        private AudioAction murderMusicAudioAction;
        [SerializeField]
        private AudioAction defaultAmbienceAudioAction;
        [SerializeField]
        private AudioAction buzzingAmbienceAudioAction;
        [SerializeField]
        private AudioAction stabPlayerAudioAction;
        [SerializeField]
        private AudioAction coatRustleAudioAction;
        [SerializeField]
        private AudioAction brandishKnifeAudioAction;
        [SerializeField]
        private AudioAction popAudioAction;
        [SerializeField]
        private AudioAction hitBodyAudioAction;
        [SerializeField]
        private AudioAction inflateAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.BUTLER_ID;

        public override Skinner CreateSkinner()
        {
            return new ButlerSkinner
            (
                attachments,
                bodyMaterialAction,
                broomMaterialAction,
                teethMaterialAction,
                hairMaterialAction,
                bodyMeshAction,
                broomMeshAction,
                teethMeshAction,
                hairMeshAction,
                footstepsAudioAction,
                sweepsAudioAction,
                popReverbAudioAction,
                murderMusicAudioAction,
                defaultAmbienceAudioAction,
                buzzingAmbienceAudioAction,
                stabPlayerAudioAction,
                coatRustleAudioAction,
                brandishKnifeAudioAction,
                popAudioAction,
                hitBodyAudioAction,
                inflateAudioAction
            );
        }
    }
}
