using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "CircuitBeesSkin", menuName = "EnemySkinKit/Skins/CircuitBeesSkin", order = 22)]
    public class CircuitBeesSkin : BaseSkin
    {
        [Header("Mesh")]
        [SerializeField]
        protected StaticMeshAction beeMeshAction;
        [Space(10)]

        [Header("Texture")]
        [SerializeField]
        protected TextureAction beeTextureAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction idleAudioAction;
        [SerializeField]
        protected AudioAction defensiveAudioAction;
        [SerializeField]
        protected AudioAction angryAudioAction;
        [SerializeField]
        protected AudioAction leaveAudioAction;
        [SerializeField]
        protected AudioAction zapConstantAudioAction;
        [SerializeField]
        protected AudioListAction zapAudioListAction;

        public StaticMeshAction BeeMeshAction => beeMeshAction;
        public TextureAction BeeTextureAction => beeTextureAction;
        public AudioAction IdleAudioAction => idleAudioAction;
        public AudioAction DefensiveAudioAction => defensiveAudioAction;
        public AudioAction AngryAudioAction => angryAudioAction;
        public AudioAction LeaveAudioAction => leaveAudioAction;
        public AudioAction ZapConstantAudioAction => zapConstantAudioAction;
        public AudioListAction ZapAudioListAction => zapAudioListAction;


        public override string EnemyId => EnemySkinRegistry.CIRCUIT_BEES_ID;

        public override Skinner CreateSkinner()
        {
            return new CircuitBeesSkinner(this);
        }
    }
}