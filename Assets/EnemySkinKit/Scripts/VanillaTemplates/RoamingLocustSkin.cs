using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    [CreateAssetMenu(fileName = "RoamingLocustSkin", menuName = "EnemySkinKit/Skins/RoamingLocustSkin", order = 23)]
    public class RoamingLocustSkin : BaseSkin
    {
        [Header("Mesh")]
        [SerializeField]
        protected StaticMeshAction locustMeshAction;
        [Space(10)]

        [Header("Texture")]
        [SerializeField]
        protected TextureAction locustTextureAction;
        [Space(10)]

        [Header("Audio")]
        [SerializeField]
        protected AudioAction chirpAudioAction;
        [SerializeField]
        protected AudioAction disperseAudioAction;

        public StaticMeshAction LocustMeshAction => locustMeshAction;
        public TextureAction LocustTextureAction => locustTextureAction;
        public AudioAction ChirpAudioAction => chirpAudioAction;
        public AudioAction DisperseAudioAction => disperseAudioAction;

        public override string EnemyId => EnemySkinRegistry.ROAMING_LOCUST_ID;

        public override Skinner CreateSkinner()
        {
            return new RoamingLocustSkinner(this);
        }
    }
}