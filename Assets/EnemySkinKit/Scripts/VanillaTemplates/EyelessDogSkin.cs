using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "EyelessDogSkin", menuName = "EnemySkinKit/Skins/EyelessDog", order = 6)]
    public class EyelessDogSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        private MaterialAction bodyMaterialAction;
        [SerializeField]
        private MaterialAction topTeethMaterialAction;
        [SerializeField]
        private MaterialAction bottomTeethMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        private SkinnedMeshAction bodyMeshAction;
        //Static Meshes
        [SerializeField]
        private StaticMeshAction topTeethMeshAction;
        [SerializeField]
        private StaticMeshAction bottomTeethMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        private AudioAction screamAudioAction;
        [SerializeField]
        private AudioAction killPlayerAudioAction;
        [SerializeField]
        private AudioAction breathingAudioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        private ArmatureAttachment[] attachments;

        public override string EnemyId => EnemySkinRegistry.EYELESS_DOG_ID;

        public override Skinner CreateSkinner()
        {
            return new EyelessDogSkinner
            (
                muteEffects, 
                muteVoice,
                attachments,
                bodyMeshAction,
                topTeethMeshAction,
                bottomTeethMeshAction,
                bodyMaterialAction,
                topTeethMaterialAction,
                bottomTeethMaterialAction,
                screamAudioAction,
                killPlayerAudioAction,
                breathingAudioAction
            );
        }
    }
}