using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "TulipSnakeSkin", menuName = "EnemySkinKit/Skins/TulipSnakeSkin", order = 21)]
    public class TulipSnakeSkin : BaseSkin
    {
        [Header("Materials")]
        //Materials
        [SerializeField]
        protected MaterialAction bodyMaterialAction;
        [Space(10)]

        [Header("Meshes")]
        //Skinned Meshes
        [SerializeField]
        protected SkinnedMeshAction bodyMeshAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioListAction flapAudioListAction;
        [SerializeField]
        protected AudioListAction chuckleAudioListAction;
        [SerializeField]
        protected AudioListAction leapAudioListAction;
        [SerializeField]
        protected AudioAction scurryAduioAction;
        [Space(10)]

        [Header("Armature Attachments")]
        [SerializeField]
        protected ArmatureAttachment[] attachments;

        public MaterialAction BodyMaterialAction => bodyMaterialAction;
        public SkinnedMeshAction BodyMeshAction => bodyMeshAction;
        public AudioListAction FlapAudioListAction => flapAudioListAction;
        public AudioListAction ChuckleAudioListAction => chuckleAudioListAction;
        public AudioListAction LeapAudioListAction => leapAudioListAction;
        public AudioAction ScurryAudioAction => scurryAduioAction;
        public ArmatureAttachment[] Attachments => attachments;

        public override string EnemyId => EnemySkinRegistry.TULIP_SNAKE_ID;

        public override Skinner CreateSkinner()
        {
            return new TulipSnakeSkinner(this);
        }
    }
}