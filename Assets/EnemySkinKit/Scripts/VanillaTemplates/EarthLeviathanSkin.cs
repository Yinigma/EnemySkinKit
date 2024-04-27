using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "EarthLeviathanSkin", menuName = "EnemySkinKit/Skins/EarthLeviathan", order = 5)]
    public class EarthLeviathanSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioListAction groundRumbleAudioListAction;
        [SerializeField]
        private AudioListAction ambientRumbleAudioListAction;
        [SerializeField]
        private AudioListAction roarAudioListAction;
        [SerializeField]
        private AudioAction hitGroundAudioAction;
        [SerializeField]
        private AudioAction emergeAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;


        public override string EnemyId => EnemySkinRegistry.EARTH_LEVIATHAN_ID;

        public override Skinner CreateSkinner()
        {
            return new EarthLeviathanSkinner(attachments, bodyMeshAction, bodyMaterialAction, groundRumbleAudioListAction, ambientRumbleAudioListAction, roarAudioListAction, emergeAudioAction, hitGroundAudioAction);
        }
    }
}