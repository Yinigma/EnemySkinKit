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
        public void Apply(SkinnedMeshRenderer[] vanilla, Transform anchor, Dictionary<string, Transform> additionalTransforms = null)
        {
            switch(actionType)
            {
                case SkinnedMeshActionType.HIDE:
                    foreach(SkinnedMeshRenderer renderer in vanilla)
                    {
                        renderer.enabled = false;
                    }
                    break;
                case SkinnedMeshActionType.REPLACE:
                    foreach (SkinnedMeshRenderer renderer in vanilla)
                    {
                        renderer.enabled = false;
                    }
                    GameObject moddedSkin = UnityEngine.Object.Instantiate(replacementObject, anchor);
                    ArmatureReflector armatureReflector = moddedSkin.AddComponent<ArmatureReflector>();
                    //todo - descriptive error here for if the replacement object doesn't have a mesh renderer
                    armatureReflector.Init(moddedSkin.GetComponentInChildren<SkinnedMeshRenderer>(), vanilla[0], armatureMap, additionalTransforms);
                    break;
                case SkinnedMeshActionType.RETAIN:
                    break;
            }
        }

        /// <summary>
        /// Undoes the action performed by Apply provided the same arguments are given
        /// </summary>
        /// <param name="vanilla">the array of skinned mesh renderers that a previous call to "Apply" operated on</param>
        /// <param name="anchor">the parent transform of the replacement prefab instance if applicable</param>
        public void Remove(SkinnedMeshRenderer[] vanilla, Transform anchor)
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
            return vanillaMesh;
        }

        public void Remove(MeshFilter vanilla, Mesh vanillaMesh)
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
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    vanillaMaterial = vanillaRenderer.materials[materialIndex];
                    vanillaRenderer.materials[materialIndex] = replacementMaterial;
                    break;
                case MaterialActionType.RETAIN:
                    break;
            }
            return vanillaMaterial;
        }

        public void Remove(Renderer vanillaRenderer, int materialIndex, Material vanillaMaterial)
        {
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    vanillaRenderer.materials[materialIndex] = vanillaMaterial;
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

        public AudioClip Apply(ref AudioClip vanillaRef)
        {
            AudioClip vanillaClip = null;
            switch (actionType)
            {
                case AudioActionType.MUTE:
                    vanillaClip = vanillaRef;
                    vanillaRef = AudioClip.Create("empty", 1, 1, 1000, false);
                    break;
                case AudioActionType.REPLACE:
                    vanillaClip = vanillaRef;
                    vanillaRef = replacementClip;
                    break;
                case AudioActionType.RETAIN:
                    break;
            }
            return vanillaClip;
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
            AudioClip vanillaClip = null;
            switch (actionType)
            {
                case AudioActionType.MUTE:
                    vanillaSource.mute = true;
                    break;
                case AudioActionType.REPLACE:
                    vanillaClip = vanillaSource.clip;
                    vanillaSource.clip = replacementClip;
                    break;
                case AudioActionType.RETAIN:
                    break;
            }
            return vanillaClip;
        }

        public void Remove(ref AudioClip vanillaRef, AudioClip vanillaClip)
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

        public void RemoveFromSource(AudioSource vanillaSource, AudioClip vanillaClip)
        {
            switch (actionType)
            {
                case AudioActionType.MUTE:
                    vanillaSource.mute = false;
                    break;
                case AudioActionType.REPLACE:
                    vanillaSource.clip = vanillaClip;
                    break;
                case AudioActionType.RETAIN:
                    break;
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

        public AudioClip[] Apply(ref AudioClip[] vanillaSource)
        {
            AudioClip[] vanillaClips = null;
            switch (actionType)
            {
                case AudioListActionType.MUTE:
                    vanillaClips = new AudioClip[vanillaSource.Length];
                    for(int i = 0; i <  vanillaSource.Length; i++)
                    {
                        vanillaClips[i] = vanillaSource[i];
                        vanillaSource[i] = AudioClip.Create("empty", 1, 1, 1000, false);
                    }
                    break;
                case AudioListActionType.REPLACE:
                    vanillaClips = new AudioClip[vanillaSource.Length];
                    Array.Copy(vanillaSource, vanillaClips, vanillaSource.Length);
                    vanillaSource = replacementClips;
                    break;
                /*case AudioListActionType.PARTIAL_REPLACE:
                    vanillaClips = new AudioClip[replacementClips.Length];
                    for (int i = 0; i < replacementClips.Length; i++)
                    {
                        vanillaClips[i] = vanillaSource[i];
                        vanillaSource[i] = replacementClips[i];
                    }
                    break;*/
                case AudioListActionType.RETAIN:
                    break;
            }
            return vanillaClips;
        }

        public void Remove(ref AudioClip[] vanillaSource, AudioClip[] vanillaClips)
        {
            switch (actionType)
            {
                case AudioListActionType.MUTE:
                    vanillaSource = vanillaClips;
                    break;
                case AudioListActionType.REPLACE:
                    vanillaSource = vanillaClips;
                    break;
                /*case AudioListActionType.PARTIAL_REPLACE:
                    Array.Copy(vanillaClips, vanillaSource, replacementClips.Length);
                    break;*/
                case AudioListActionType.RETAIN:
                    break;
            }
        }
    }
}