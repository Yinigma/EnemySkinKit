using AntlerShed.EnemySkinKit.ArmatureReflection;
using AntlerShed.SkinRegistry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace AntlerShed.EnemySkinKit.SkinAction
{
    public enum SkinnedMeshActionType
    {
        RETAIN,
        HIDE,
        REPLACE
    }

    [Serializable]
    public struct SkinnedMeshAction
    {
        public SkinnedMeshActionType actionType;

        public GameObject replacementObject;

        public ArmatureMapGenerator armatureMap;

        /// <summary>
        /// Applies this skinned mesh action. 
        /// HIDE will disable all skinned mesh renderers in the given array. 
        /// REPLACE will do the same and then instantiate a clone of the "replacementObject" field as a child of "anchor."
        /// RETAIN is a no op.
        /// </summary>
        /// <param name="vanilla">the array of skinned mesh renderers to operate on. This is an array so multiple LOD meshes can be dealt with in the same way.</param>
        /// <param name="anchor">the transform that will be the parent of the replacement prefab if applicable. It's up to the author of any custom enemies to decide what is an appopriate anchor.</param>
        /// <return></return>
        public GameObject Apply(SkinnedMeshRenderer[] vanilla, Transform anchor, Dictionary<string, Transform> additionalTransforms = null)
        {
            GameObject moddedSkin = null;
            if (vanilla != null)
            {
                switch (actionType)
                {
                    case SkinnedMeshActionType.HIDE:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = false;
                        }
                        break;
                    case SkinnedMeshActionType.REPLACE:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = false;
                        }
                        moddedSkin = UnityEngine.Object.Instantiate(replacementObject, anchor);
                        SkinnedMeshRenderer customRenderer = moddedSkin.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (customRenderer != null)
                        {
                            customRenderer.gameObject.layer = LayerMask.NameToLayer("Enemies");
                            ArmatureReflector armatureReflector = moddedSkin.AddComponent<ArmatureReflector>();
                            armatureReflector.Init(customRenderer, vanilla[0], armatureMap, additionalTransforms);
                        }
                        else
                        {
                            GameObject.Destroy(customRenderer);
                            if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) EnemySkinKit.SkinKitLogger.LogError("The given prefab for a skinned mesh action did not contain a skinned mesh renderer in its hierarchy.");
                        }
                        break;
                    case SkinnedMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Skinned Mesh Renderer was null. Skipping Apply."); }
            }
            return moddedSkin;
        }

        /// <summary>
        /// Undoes the action performed by Apply provided the same arguments are given
        /// </summary>
        /// <param name="vanilla">the array of skinned mesh renderers that a previous call to "Apply" operated on</param>
        /// <param name="anchor">the parent transform of the replacement prefab instance if applicable</param>
        public void Remove(SkinnedMeshRenderer[] vanilla, Transform anchor)
        {
            if (vanilla != null)
            {
                switch (actionType)
                {
                    case SkinnedMeshActionType.HIDE:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = true;
                        }
                        break;
                    case SkinnedMeshActionType.REPLACE:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = true;
                        }
                        GameObject.Destroy(anchor.GetComponentInChildren<ArmatureReflector>(true).gameObject);
                        break;
                    case SkinnedMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Skinned Mesh Renderer was null. Skipping remove."); }
            }
        }
    }

    public enum StaticMeshActionType
    {
        RETAIN,
        HIDE,
        REPLACE
    }

    [Serializable]
    public struct StaticMeshAction
    {
        public StaticMeshActionType actionType;

        public Mesh replacementMesh;

        public Mesh Apply(MeshFilter vanilla)
        {
            Mesh vanillaMesh = null;
            if (vanilla != null)
            {
                switch (actionType)
                {
                    case StaticMeshActionType.HIDE:
                        vanilla.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        break;
                    case StaticMeshActionType.REPLACE:
                        vanillaMesh = vanilla.mesh;
                        vanilla.mesh = replacementMesh;
                        break;
                    case StaticMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Mesh Filter was null. Skipping Apply."); }
            }
            return vanillaMesh;
        }

        public void Remove(MeshFilter vanilla, Mesh vanillaMesh)
        {
            if (vanilla != null)
            {
                switch (actionType)
                {
                    case StaticMeshActionType.HIDE:
                        vanilla.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        break;
                    case StaticMeshActionType.REPLACE:
                        vanilla.mesh = vanillaMesh;
                        break;
                    case StaticMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Mesh Filter was null. Skipping remove."); }
            }
        }
    }

    public enum MaterialActionType
    {
        RETAIN,
        REPLACE
    }

    [Serializable]
    public struct MaterialAction
    {
        public MaterialActionType actionType;
        public Material replacementMaterial;

        public Material Apply(Renderer vanillaRenderer, int materialIndex)
        {
            Material vanillaMaterial = null;
            if (vanillaRenderer != null)
            {
                switch (actionType)
                {
                    case MaterialActionType.REPLACE:
                        vanillaMaterial = vanillaRenderer.materials[materialIndex];
                        Material[] mats = vanillaRenderer.materials;
                        mats[materialIndex] = replacementMaterial;
                        vanillaRenderer.materials = mats;
                        break;
                    case MaterialActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning( "Vanilla Renderer was null for Material Action. Skipping Apply." ); }
            }
            return vanillaMaterial;
        }

        public Material ApplyRef(ref Material vanillaRef)
        {
            Material vanillaMaterial = vanillaRef;
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    vanillaRef = replacementMaterial;
                    break;
                case MaterialActionType.RETAIN:
                    break;
            }
            return vanillaMaterial;
        }

        public void Remove(Renderer vanillaRenderer, int materialIndex, Material vanillaMaterial)
        {
            if (vanillaRenderer != null)
            {
                switch (actionType)
                {
                    case MaterialActionType.REPLACE:
                        Material[] mats = vanillaRenderer.materials;
                        mats[materialIndex] = vanillaMaterial;
                        vanillaRenderer.materials = mats;
                        break;
                    case MaterialActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Renderer was null for Material Action. Skipping remove."); }
            }
        }

        public void RemoveRef(ref Material dest, Material vanillaMaterial)
        {
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    dest = vanillaMaterial;
                    break;
                case MaterialActionType.RETAIN:
                    break;
            }
        }
    }

    public enum AudioActionType
    {
        RETAIN,
        MUTE,
        REPLACE
    }

    [Serializable]
    public struct AudioAction
    {
        public AudioActionType actionType;

        public AudioClip replacementClip;

        public AudioClip Silence => AudioClip.Create("empty", 1, 1, 1000, false);

        public AudioClip Apply(ref AudioClip vanillaRef)
        {
            
            AudioClip vanillaClip = vanillaRef;
            if (vanillaRef != null)
            {
                switch (actionType)
                {
                    case AudioActionType.MUTE:
                        vanillaRef = Silence;
                        break;
                    case AudioActionType.REPLACE:
                        vanillaRef = replacementClip;
                        break;
                    case AudioActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Audio Clip was null for Audio Action. Skipping Apply."); }
            }
            return vanillaClip;
        }

        public AudioClip WorkingClip(AudioClip vanillaClip)
        {
            switch (actionType)
            {
                case AudioActionType.REPLACE:
                    return replacementClip;
                case AudioActionType.MUTE:
                    return Silence;
                case AudioActionType.RETAIN:
                    goto default;
                default:
                    return vanillaClip;
            }
        }

        /// <summary>
        /// Used in cases where an audio source only has a single, static audio clip as its source.
        /// Rather than changing the pointer to point to a different audio clip, this will replace 
        /// the reference in the Audio Source component playing it.
        /// One example of a sound that needs to be replaced in this way is the bracken's anger sound.
        /// </summary>
        /// <param name="vanillaSource">The audio source component with an effectively-static audioClip</param>
        /// <returns>The vanilla audio clip in the case that the actionType is set to REPLACE. Null otherwise. Whatever is returned must be stored and re-used in the corresponding Remove call</returns>
        public AudioClip ApplyToSource(AudioSource vanillaSource)
        {
            AudioClip vanillaClip = vanillaSource.clip;
            if (vanillaSource != null)
            {
                switch (actionType)
                {
                    case AudioActionType.MUTE:
                        vanillaSource.Stop();
                        vanillaSource.clip = Silence;
                        break;
                    case AudioActionType.REPLACE:
                        vanillaSource.clip = replacementClip;
                        if (vanillaSource.isPlaying) 
                        {
                            vanillaSource.Stop();
                            vanillaSource.Play();
                        }
                        break;
                    case AudioActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla AudioSource was null for Audio Action. Skipping apply."); }
            }
            return vanillaClip;
        }

        public void Remove(ref AudioClip vanillaRef, AudioClip vanillaClip)
        {
            if (vanillaRef != null)
            {
                switch (actionType)
                {
                    case AudioActionType.MUTE:
                        vanillaRef = vanillaClip;
                        break;
                    case AudioActionType.REPLACE:
                        vanillaRef = vanillaClip;
                        break;
                    case AudioActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Audio Clip was null for Audio Action. Skipping remove."); }
            }
        }

        public void RemoveFromSource(AudioSource vanillaSource, AudioClip vanillaClip)
        {
            if (vanillaSource != null)
            {
                switch (actionType)
                {
                    case AudioActionType.MUTE:
                        vanillaSource.mute = false;
                        break;
                    case AudioActionType.REPLACE:
                        vanillaSource.clip = vanillaClip;
                        if (vanillaSource.isPlaying)
                        {
                            vanillaSource.Stop();
                            vanillaSource.Play();
                        }
                        break;
                    case AudioActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla AudioSource was null for Audio Action. Skipping remove."); }
            }
        }
    }

    public enum AudioListActionType
    {
        RETAIN,
        MUTE,
        REPLACE,
        
    }

    [Serializable]
    public struct AudioListAction
    {
        public AudioListActionType actionType;

        public AudioClip[] replacementClips;

        private AudioClip Silence => AudioClip.Create("empty", 1, 1, 1000, false);

        public AudioClip[] Apply(ref AudioClip[] vanillaSource)
        {
            AudioClip[] vanillaClips = new AudioClip[vanillaSource.Length];
            Array.Copy(vanillaSource, vanillaClips, vanillaSource.Length);
            if (vanillaSource != null)
            {
                switch (actionType)
                {
                    case AudioListActionType.MUTE:
                        for (int i = 0; i < vanillaSource.Length; i++)
                        {
                            vanillaSource[i] = Silence;
                        }
                        break;
                    case AudioListActionType.REPLACE:
                        vanillaSource = replacementClips;
                        break;
                    case AudioListActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Audio Clip Array was null for Audio List Action. Skipping Apply."); }
            }
            return vanillaClips;
        }

        public AudioClip[] WorkingClips(AudioClip[] vanillaClips)
        {
            switch (actionType)
            {
                case AudioListActionType.MUTE:
                    AudioClip[] silenceClips = new AudioClip[vanillaClips.Length];
                    for (int i = 0; i < vanillaClips.Length; i++)
                    {
                        silenceClips[i] = Silence;
                    }
                    return silenceClips;
                case AudioListActionType.REPLACE:
                    return replacementClips;
                case AudioListActionType.RETAIN:
                    goto default;
                default:
                    return vanillaClips;
            }
        }

        public void Remove(ref AudioClip[] vanillaSource, AudioClip[] vanillaClips)
        {
            if (vanillaSource != null)
            {
                switch (actionType)
                {
                    case AudioListActionType.MUTE:
                        vanillaSource = vanillaClips;
                        break;
                    case AudioListActionType.REPLACE:
                        vanillaSource = vanillaClips;
                        break;
                    case AudioListActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Audio Clip Array was null for Audio List Action. Skipping remove."); }
            }
        }
    }
}