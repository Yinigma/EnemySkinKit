using UnityEngine;

namespace AntlerShed.EnemySkinKit.AudioReflection
{
    public readonly struct AudioReplacement
    {
        public AudioClip ReplacementClip { get; }
        public AudioClip[] ReplacementClips { get; }

        public AudioReplacement(AudioClip replacementClip)
        {
            ReplacementClip = replacementClip;
            ReplacementClips = null;
        }

        public AudioReplacement(AudioClip[] replacementClips)
        {
            ReplacementClip = null;
            ReplacementClips = replacementClips;
        }
    }

}