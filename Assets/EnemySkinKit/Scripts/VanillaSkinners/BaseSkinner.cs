using AntlerShed.SkinRegistry;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public abstract class BaseSkinner : Skinner
    {
        /*protected bool origEffectsMute;
        protected bool origVoiceMute;
        protected bool MuteEffects { get; }
        protected bool MuteVoice { get; }

        public BaseSkinner()
        {
            MuteEffects = muteSoundEffects;
            MuteVoice = muteVoice;
        }*/

        public abstract void Apply(GameObject enemy);
        // {
        /*EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        origVoiceMute = enemyAI.creatureVoice.mute;
        origEffectsMute = enemyAI.creatureSFX.mute;
        enemyAI.creatureSFX.mute = MuteEffects;
        enemyAI.creatureVoice.mute = MuteVoice;*/
        //}

        public abstract void Remove(GameObject enemy);
        //{
            /*EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.creatureSFX.mute = origEffectsMute;
            enemyAI.creatureVoice.mute = origVoiceMute;*/
        //}

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
            destination.spatialize = source.spatialize;
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

        protected static AudioSource CreateModdedAudioSource
        (
            AudioSource vanilla,
            string label = "modAudioSource",
            AudioClip initClip = null
        )
        {
            GameObject voiceGO = new GameObject(label);
            voiceGO.transform.parent = vanilla.gameObject.transform;
            voiceGO.transform.localPosition = Vector3.zero;
            voiceGO.transform.localScale = Vector3.one;
            voiceGO.transform.localRotation = Quaternion.identity;
            AudioSource moddedSource = voiceGO.AddComponent<AudioSource>();
            CopyAudioSource(vanilla, moddedSource);
            moddedSource.clip = initClip;
            OccludeAudio vanillaOcclude = vanilla.gameObject.GetComponent<OccludeAudio>();
            AudioLowPassFilter vanillaLPF = vanilla.gameObject.GetComponent<AudioLowPassFilter>();
            AudioReverbFilter vanillaReverb = vanilla.gameObject.GetComponent<AudioReverbFilter>();
            if (vanillaOcclude != null)
            {
                if (vanillaLPF != null)
                {
                    AudioLowPassFilter moddedLPF = voiceGO.AddComponent<AudioLowPassFilter>();
                    CopyLowPass(vanillaLPF, moddedLPF);
                }
                if (vanillaReverb != null)
                {
                    AudioReverbFilter moddedReverb = voiceGO.AddComponent<AudioReverbFilter>();
                    CopyReverb(vanillaReverb, moddedReverb);
                }
                OccludeAudio moddedOcclude = voiceGO.AddComponent<OccludeAudio>();
                CopyOccludeAudio(vanillaOcclude, moddedOcclude);
            }
            if (vanilla.playOnAwake)
            {
                moddedSource.Play();
            }
            return moddedSource;
        }

        protected static void DestroyModdedAudioSource(AudioSource mod)
        {
            GameObject.Destroy(mod.gameObject);
        }
    }

}