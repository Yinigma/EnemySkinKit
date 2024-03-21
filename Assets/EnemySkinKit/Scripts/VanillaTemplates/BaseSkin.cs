using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public abstract class BaseSkin : ScriptableObject, Skin
    {
        [SerializeField]
        protected string label;
        public string Label => label;
        [SerializeField]
        protected string id;
        public string Id => id;
        [SerializeField]
        protected Texture2D icon;
        [SerializeField]
        protected bool muteEffects;
        [SerializeField]
        protected bool muteVoice;
        public Texture2D Icon => icon;
        public abstract string EnemyId { get; }
        public abstract Skinner CreateSkinner();
    }
}
