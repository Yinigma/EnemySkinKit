using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "EarthLeviathanSkin", menuName = "EnemySkinKit/Skins/EarthLeviathan", order = 1)]
    public class EarthLeviathanSkin : BaseSkin
    {
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;

        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;

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
        [SerializeField]
        private ArmatureAttachment[] attachments;


        public override string EnemyId => EnemySkinRegistry.EARTH_LEVIATHAN_ID;

        public override Skinner CreateSkinner()
        {
            return new EarthLeviathanSkinner(muteEffects, muteVoice, attachments, bodyMeshAction, bodyMaterialAction, groundRumbleAudioListAction, ambientRumbleAudioListAction, roarAudioListAction, emergeAudioAction, hitGroundAudioAction);
        }
    }
}