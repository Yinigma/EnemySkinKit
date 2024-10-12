using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.SkinRegistry;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Patches
{
    static class AudioSourcePatch
    {
        private static Dictionary<AudioSource, AudioReflector> sourceToReflectorMap = new Dictionary<AudioSource, AudioReflector>();
        private static Dictionary<AudioReflector, AudioSource> reflectorToSourceMap = new Dictionary<AudioReflector, AudioSource>();

        public static void AddReflector(AudioSource source, AudioReflector reflector)
        {
            sourceToReflectorMap[source] = reflector;
            reflectorToSourceMap[reflector] = source;
        }

        public static void RemoveReflector(AudioReflector reflector)
        {
            if(reflectorToSourceMap.ContainsKey(reflector))
            {
                AudioSource source = null;
                reflectorToSourceMap.Remove(reflector, out source);
                if(source!=null && sourceToReflectorMap.ContainsKey(source))
                {
                    sourceToReflectorMap.Remove(source);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new Type[] { typeof(ulong) })]
        static void OnPlay(AudioSource __instance, ulong delay)
        {
            if(sourceToReflectorMap.ContainsKey(__instance))
            {
                sourceToReflectorMap[__instance].SkinKitPlay(delay);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new Type[] {})]
        static void OnPlay(AudioSource __instance)
        {
            if (sourceToReflectorMap.ContainsKey(__instance))
            {
                sourceToReflectorMap[__instance].SkinKitPlay(0);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.PlayOneShot), new Type[] { typeof(AudioClip), typeof(float) })]
        static void OnPlayOneShot(AudioSource __instance, AudioClip clip, float volumeScale)
        {
            if(sourceToReflectorMap.ContainsKey(__instance))
            {
                sourceToReflectorMap[__instance].SkinKitPlayOneShot(new OneShotArgs(clip, volumeScale));
            }
            
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Stop), new Type[] { typeof(bool) })]
        static void OnStop(AudioSource __instance)
        {
            if (sourceToReflectorMap.ContainsKey(__instance))
            {
                sourceToReflectorMap[__instance].SkinKitStop();
            }
        }
    }

    public readonly struct OneShotArgs
    {
        public float Volume { get; }
        public AudioClip Clip { get; }

        internal OneShotArgs(AudioClip clip, float volume)
        {
            Clip = clip;
            Volume = volume;
        }
    }
}