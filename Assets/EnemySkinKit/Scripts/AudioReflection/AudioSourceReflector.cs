using AntlerShed.EnemySkinKit.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.AudioReflection
{
    public class AudioReflector : MonoBehaviour
    {
        private AudioSource moddedSource;

        private AudioSource vanillaSource;

        private Dictionary<string, AudioReplacement> clipMap;

        private System.Random random;

        public void Awake()
        {
            vanillaSource = transform.parent.GetComponent<AudioSource>();
            moddedSource = gameObject.AddComponent<AudioSource>();
            OccludeAudio vanillaOcclude = transform.parent.GetComponent<OccludeAudio>();
            AudioLowPassFilter vanillaLPF = transform.parent.GetComponent<AudioLowPassFilter>();
            AudioReverbFilter vanillaReverb = transform.parent.GetComponent<AudioReverbFilter>();
            CopyAudioSource(vanillaSource, moddedSource);
            if (vanillaOcclude != null)
            {
                if (vanillaLPF != null)
                {
                    AudioLowPassFilter moddedLPF = gameObject.AddComponent<AudioLowPassFilter>();
                    CopyLowPass(vanillaLPF, moddedLPF);
                }
                if (vanillaReverb != null)
                {
                    AudioReverbFilter moddedReverb = gameObject.AddComponent<AudioReverbFilter>();
                    CopyReverb(vanillaReverb, moddedReverb);
                }
                OccludeAudio moddedOcclude = gameObject.AddComponent<OccludeAudio>();
                CopyOccludeAudio(vanillaOcclude, moddedOcclude);
            }
        }

        public void Init(Dictionary<string, AudioReplacement> clipMap, ulong seed)
        {
            this.clipMap = clipMap;
            random = new System.Random((int)seed);
            if(vanillaSource?.clip?.name != null && clipMap.ContainsKey(vanillaSource.clip.name))
            {
                if(vanillaSource.playOnAwake && clipMap[vanillaSource.clip.name].ReplacementClip != null)
                {
                    moddedSource.clip = clipMap[vanillaSource.clip.name].ReplacementClip;
                    moddedSource.Play();
                }
            }
        }

        public void SkinKitPlay(ulong delay)
        {
            EnemySkinKit.SkinKitLogger.LogInfo("Play Clip");
            moddedSource.clip = vanillaSource.clip;
            if (vanillaSource.clip != null)
            {
                EnemySkinKit.SkinKitLogger.LogInfo($"Vanilla: {vanillaSource.clip.name}");
                if (clipMap.ContainsKey(vanillaSource.clip.name))
                {
                    EnemySkinKit.SkinKitLogger.LogInfo($"Replacement: {GetClip(clipMap[vanillaSource.clip.name]).name}");
                    AudioClip replacementClip = GetClip(clipMap[vanillaSource.clip.name]);
                    if (replacementClip != null)
                    {
                        //REPLACE or MUTE
                        moddedSource.clip = replacementClip;
                    }
                }
            }
            moddedSource.Play(delay);
        }

        public void SkinKitPlayOneShot(OneShotArgs args)
        {
            if(args.Clip!=null)
            {
                if (clipMap.ContainsKey(args.Clip.name))
                {
                    AudioClip replacementClip = GetClip(clipMap[args.Clip.name]);
                    if (replacementClip != null)
                    {
                        //REPLACE or MUTE
                        moddedSource.PlayOneShot(replacementClip, args.Volume);
                    }
                }
                else
                {
                    //RETAIN
                    moddedSource.PlayOneShot(args.Clip, args.Volume);
                }
            }
        }

        private AudioClip GetClip(AudioReplacement replacement)
        {
            //Single clip
            if (replacement.ReplacementClip != null)
            {
                return replacement.ReplacementClip;
            }
            //Multiple clips
            else if (replacement.ReplacementClips != null)
            {
                return replacement.ReplacementClips[random.Next(replacement.ReplacementClips.Length)];
            }
            return null;
        }

        public void SkinKitStop()
        {
            moddedSource.Stop();
        }

        public void Update()
        {
            moddedSource.volume = vanillaSource.volume;
            moddedSource.pitch = vanillaSource.pitch;
        }

        private static void CopyAudioSource(AudioSource source, AudioSource destination)
        {
            destination.bypassEffects = source.bypassEffects;
            destination.bypassListenerEffects = source.bypassListenerEffects;
            destination.bypassReverbZones = source.bypassReverbZones;
            destination.dopplerLevel = source.dopplerLevel;
            destination.ignoreListenerPause = source.ignoreListenerPause;
            destination.ignoreListenerVolume = source.ignoreListenerVolume;
            destination.maxDistance = source.maxDistance;
            destination.minDistance = source.minDistance;
            destination.outputAudioMixerGroup = source.outputAudioMixerGroup;
            destination.panStereo = source.panStereo;
            destination.pitch = source.pitch;
            destination.priority = source.priority;
            destination.reverbZoneMix = source.reverbZoneMix;
            destination.rolloffMode = source.rolloffMode;
            destination.spatialBlend = source.spatialBlend;
            destination.spatialize = false;
            destination.spatializePostEffects = source.spatializePostEffects;
            destination.spread = source.spread;
            destination.tag = source.tag;
            destination.volume = source.volume;
            destination.clip = source.clip;
            destination.playOnAwake = source.playOnAwake;
        }

        private static void CopyLowPass(AudioLowPassFilter source, AudioLowPassFilter destination)
        {
            destination.customCutoffCurve = source.customCutoffCurve;
            destination.cutoffFrequency = source.cutoffFrequency;
            destination.lowpassResonanceQ = source.lowpassResonanceQ;
        }

        private static void CopyReverb(AudioReverbFilter source, AudioReverbFilter destination)
        {
            destination.decayHFRatio = source.decayHFRatio;
            destination.decayTime = source.decayTime;
            destination.density = source.density;
            destination.diffusion = source.diffusion;
            destination.dryLevel = source.dryLevel;
            destination.hfReference = source.hfReference;
            destination.lfReference = source.lfReference;
            destination.reflectionsDelay = source.reflectionsDelay;
            destination.reflectionsLevel = source.reflectionsLevel;
            destination.reverbDelay = source.reverbDelay;
            destination.reverbLevel = source.reverbLevel;
            destination.reverbPreset = source.reverbPreset;
            destination.room = source.room;
            destination.roomLF = source.roomLF;
            destination.roomHF = source.roomHF;
        }

        protected static void CopyOccludeAudio(OccludeAudio source, OccludeAudio destination)
        {
            destination.useReverb = source.useReverb;
            destination.overridingLowPass = source.overridingLowPass;
            destination.lowPassOverride = source.lowPassOverride;
        }
    }
}