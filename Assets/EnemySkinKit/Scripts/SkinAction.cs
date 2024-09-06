using AntlerShed.EnemySkinKit.ArmatureReflection;
using AntlerShed.SkinRegistry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;


namespace AntlerShed.EnemySkinKit.SkinAction
{
    public enum SkinnedMeshActionType
    {
        RETAIN,
        HIDE,
        REPLACE,
        REPLACE_MESH
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
                    case SkinnedMeshActionType.REPLACE_MESH:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = false;
                        }
                        moddedSkin = UnityEngine.Object.Instantiate(replacementObject, anchor);
                        SkinnedMeshRenderer[] customRenderers = moddedSkin.GetComponentsInChildren<SkinnedMeshRenderer>();

                        foreach (SkinnedMeshRenderer customRendererInstance in customRenderers)
                        {
                            customRendererInstance.gameObject.layer = LayerMask.NameToLayer("Enemies");
                            BoneMapper.Apply(vanilla[0], customRendererInstance);
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
        public void Remove(SkinnedMeshRenderer[] vanilla, GameObject moddedSkin)
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
                    case SkinnedMeshActionType.REPLACE_MESH:
                        foreach (SkinnedMeshRenderer renderer in vanilla)
                        {
                            renderer.enabled = true;
                        }
                        if (moddedSkin != null)
                        {
                            GameObject.Destroy(moddedSkin);
                        }
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

        public Mesh ApplyToVisualEffect(VisualEffect vanillaVisualEffect, string meshId)
        {
            Mesh vanillaMesh = null;
            if (vanillaVisualEffect != null)
            {
                switch (actionType)
                {
                    case StaticMeshActionType.HIDE:
                        vanillaVisualEffect.enabled = false;
                        break;
                    case StaticMeshActionType.REPLACE:
                        vanillaMesh = vanillaVisualEffect.GetMesh(meshId);
                        vanillaVisualEffect.SetMesh(meshId, replacementMesh);
                        break;
                    case StaticMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Visual Effect was null. Skipping Apply."); }
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

        public void RemoveFromVisualEffect(VisualEffect vanillaVisualEffect, Mesh vanillaMesh, string meshId)
        {
            if (vanillaVisualEffect != null)
            {
                switch (actionType)
                {
                    case StaticMeshActionType.HIDE:
                        vanillaVisualEffect.enabled = true;
                        break;
                    case StaticMeshActionType.REPLACE:
                        vanillaVisualEffect.SetMesh(meshId, vanillaMesh);
                        break;
                    case StaticMeshActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Visual Effect was null. Skipping Remove."); }
            }
        }
    }

    public enum ColorActionType
    {
        RETAIN,
        REPLACE
    }

    [Serializable]
    public struct ColorAction
    {
        public ColorActionType actionType;
        public Color replacementColor;
        
        public Color Apply(Material vanillaMat, string property)
        {
            Color vanillaColor = Color.black;
            if (vanillaMat != null)
            {
                try
                {
                    vanillaColor = vanillaMat.GetColor(property);
                    switch (actionType)
                    {
                        case ColorActionType.REPLACE:
                            vanillaMat.SetColor(property, replacementColor);
                            break;
                        case ColorActionType.RETAIN:
                            break;
                    }
                }
                catch(Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla material \"{vanillaMat.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Material was null for Color Action. Skipping Apply."); }
            }
            return vanillaColor;
        }

        public void Remove(Material vanillaMat, string property, Color vanillaColor)
        {
            if (vanillaMat != null)
            {
                try
                {
                    switch (actionType)
                    {
                        case ColorActionType.REPLACE:
                            vanillaMat.SetColor(property, vanillaColor);
                            break;
                        case ColorActionType.RETAIN:
                            break;
                    }
                }
                catch(Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla material \"{vanillaMat.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Material was null for Color Action. Skipping remove."); }
            }
        }

        public Color Apply(Light vanillaLight)
        {
            Color vanillaColor = Color.black;
            if (vanillaLight != null)
            {

                vanillaColor = vanillaLight.color;
                switch (actionType)
                {
                    case ColorActionType.REPLACE:
                        vanillaLight.color = replacementColor;
                        break;
                    case ColorActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Light was null for Color Action. Skipping Apply."); }
            }
            return vanillaColor;
        }

        public void Remove(Light vanillaLight, Color vanillaColor)
        {
            if (vanillaLight != null)
            {
                switch (actionType)
                {
                    case ColorActionType.REPLACE:
                        vanillaLight.color = vanillaColor;
                        break;
                    case ColorActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Light was null for Color Action. Skipping remove."); }
            }
        }
    }

    public enum TextureActionType
    {
        RETAIN,
        REPLACE
    }

    [Serializable]
    public struct TextureAction
    {
        public TextureActionType actionType;
        public Texture replacementTexture;

        public Texture Apply(Material vanillaMat, string property)
        {
            Texture vanillaTexture = null;
            if (vanillaMat != null)
            {
                try
                { 
                    vanillaTexture = vanillaMat.GetTexture(property);
                    switch (actionType)
                    {
                        case TextureActionType.REPLACE:
                            vanillaMat.SetTexture(property, replacementTexture);
                            break;
                        case TextureActionType.RETAIN:
                            break;
                    }
                }
                catch (Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla material \"{vanillaMat.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Material was null for Texture Action. Skipping Apply."); }
            }
            return vanillaTexture;
        }

        public void Remove(Material vanillaMat, string property, Texture vanillaTexture)
        {
            if (vanillaMat != null)
            {
                try 
                { 
                    switch (actionType)
                    {
                        case TextureActionType.REPLACE:
                            vanillaMat.SetTexture(property, vanillaTexture);
                            break;
                        case TextureActionType.RETAIN:
                            break;
                    }
                }
                catch (Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla material \"{vanillaMat.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Material was null for Texture Action. Skipping remove."); }
            }
        }

        public Texture ApplyToVisualEffect(VisualEffect vanillaVisualEffect, string property)
        {
            Texture vanillaTexture = null;
            if (vanillaVisualEffect != null)
            {
                try
                {
                    vanillaTexture = vanillaVisualEffect.GetTexture(property);
                    switch (actionType)
                    {
                        case TextureActionType.REPLACE:
                            vanillaVisualEffect.SetTexture(property, replacementTexture);
                            break;
                        case TextureActionType.RETAIN:
                            break;
                    }
                }
                catch (Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla visual effect \"{vanillaVisualEffect.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla visual effect was null for Texture Action. Skipping Apply."); }
            }
            return vanillaTexture;
        }

        public void RemoveFromVisualEffect(VisualEffect vanillaVisualEffect, string property, Texture vanillaTexture)
        {
            if (vanillaVisualEffect != null)
            {
                try
                {
                    switch (actionType)
                    {
                        case TextureActionType.REPLACE:
                            vanillaVisualEffect.SetTexture(property, vanillaTexture);
                            break;
                        case TextureActionType.RETAIN:
                            break;
                    }
                }
                catch (Exception)
                {
                    if (EnemySkinKit.LogLevelSetting >= LogLevel.ERROR) { EnemySkinKit.SkinKitLogger.LogError($"No such property \"{property}\" exists for vanilla visual effect \"{vanillaVisualEffect.name}.\" Skipping."); }
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla visual effect was null for Texture Action. Skipping remove."); }
            }
        }
    }

    public enum MaterialActionType
    {
        RETAIN,
        REPLACE,
        REPLACE_TEXTURE
    }

    public struct VanillaMaterial
    {
        public Material material;
        public Texture texture;
    }

    [Serializable]
    public struct MaterialAction
    {
        public MaterialActionType actionType;
        public Material replacementMaterial;
        public Texture2D replacementTexture;

        public VanillaMaterial Apply(Renderer vanillaRenderer, int materialIndex)
        {
            VanillaMaterial vanillaMaterial = new VanillaMaterial();
            if (vanillaRenderer != null)
            {
                switch (actionType)
                {
                    case MaterialActionType.REPLACE:
                        vanillaMaterial.material = vanillaRenderer.materials[materialIndex];
                        Material[] mats = vanillaRenderer.materials;
                        mats[materialIndex] = replacementMaterial;
                        vanillaRenderer.materials = mats;
                        break;
                    case MaterialActionType.REPLACE_TEXTURE:
                        vanillaMaterial.texture = vanillaRenderer.materials[materialIndex].mainTexture;
                        vanillaRenderer.materials[materialIndex].mainTexture = replacementTexture;
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

        public VanillaMaterial ApplyRef(ref Material vanillaRef)
        {
            
            VanillaMaterial vanillaMaterial = new VanillaMaterial();
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    vanillaMaterial.material = vanillaRef;
                    vanillaRef = replacementMaterial;
                    break;
                case MaterialActionType.REPLACE_TEXTURE:
                    vanillaMaterial.texture = vanillaRef.mainTexture;
                    vanillaRef.mainTexture = replacementTexture;
                    break;
                case MaterialActionType.RETAIN:
                    break;
            }
            return vanillaMaterial;
        }

        public void Remove(Renderer vanillaRenderer, int materialIndex, VanillaMaterial vanillaMaterial)
        {
            if (vanillaRenderer != null)
            {
                switch (actionType)
                {
                    case MaterialActionType.REPLACE:
                        Material[] mats = vanillaRenderer.materials;
                        mats[materialIndex] = vanillaMaterial.material;
                        vanillaRenderer.materials = mats;
                        break;
                    case MaterialActionType.REPLACE_TEXTURE:
                        vanillaRenderer.materials[materialIndex].mainTexture = vanillaMaterial.texture;
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

        public void RemoveRef(ref Material dest, VanillaMaterial vanillaMaterial)
        {
            switch (actionType)
            {
                case MaterialActionType.REPLACE:
                    dest = vanillaMaterial.material;
                    break;
                case MaterialActionType.REPLACE_TEXTURE:
                    dest.mainTexture = vanillaMaterial.texture;
                    break;
                case MaterialActionType.RETAIN:
                    break;
            }
        }
    }

    public enum ParticleSystemActionType
    {
        RETAIN,
        HIDE,
        REPLACE
    }

    [Serializable]
    public struct ParticleSystemAction
    {
        public ParticleSystemActionType actionType;
        public ParticleSystem replacementParticle;

        public ParticleSystem Apply(ParticleSystem vanillaParticle)
        {
            ParticleSystem retSystem = null;
            if (vanillaParticle?.gameObject?.GetComponent<ParticleSystemRenderer>() != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.RETAIN:
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                        break;
                    case ParticleSystemActionType.REPLACE:
                        ParticleSystem instance = GameObject.Instantiate(replacementParticle.gameObject, vanillaParticle.transform).GetComponent<ParticleSystem>();
                        vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                        instance.transform.localScale = Vector3.one;
                        instance.transform.localRotation = Quaternion.identity;
                        instance.transform.localPosition = Vector3.zero;
                        if(vanillaParticle.isPlaying != instance.isPlaying)
                        {
                            if(vanillaParticle.isPlaying)
                            {
                                instance.Play();
                            }
                            else
                            {
                                instance.Stop();
                            }
                        }
                        retSystem = instance;
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Renderer was null for Particle Action. Skipping Apply."); }
            }
            return retSystem;
        }

        public ParticleSystem ApplySubEmitter(ParticleSystem vanillaParticleRef, int subEmitterIndex)
        {
            ParticleSystem vanillaSubEmitter = vanillaParticleRef?.subEmitters.GetSubEmitterSystem(subEmitterIndex);
            if(vanillaSubEmitter != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.RETAIN:
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaParticleRef.subEmitters.GetSubEmitterSystem(subEmitterIndex).gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                        break;
                    case ParticleSystemActionType.REPLACE:
                        if (replacementParticle != null)
                        {
                            ParticleSystem instance = GameObject.Instantiate(replacementParticle.gameObject, vanillaSubEmitter.transform).GetComponent<ParticleSystem>();
                            instance.transform.localScale = Vector3.one;
                            instance.transform.localRotation = Quaternion.identity;
                            instance.transform.localPosition = Vector3.zero;
                            vanillaParticleRef.subEmitters.SetSubEmitterSystem(subEmitterIndex, instance);
                            if (instance.isPlaying != vanillaSubEmitter.isPlaying)
                            {
                                if (vanillaSubEmitter.isPlaying)
                                {
                                    instance.Play();
                                }
                                else
                                {
                                    instance.Stop();
                                }
                            }
                            if (vanillaSubEmitter.isPlaying)
                            {
                                vanillaSubEmitter.Stop();
                            }
                        }
                        else
                        {
                            vanillaParticleRef.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                            if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Replacement particle was null. Vanilla particle will be hidden."); }
                        }
                        break;
                }
                return vanillaSubEmitter;
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Could not find particle sub-emitter at index specified. Skipping."); }
                return null;
            }
        }

        public ParticleSystem ApplyRef(ref ParticleSystem vanillaParticleRef)
        {
            ParticleSystem vanillaParticle = vanillaParticleRef;
            if (vanillaParticleRef?.gameObject?.GetComponent<ParticleSystemRenderer>() != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.RETAIN:
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaParticleRef.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                        break;
                    case ParticleSystemActionType.REPLACE:
                        if(replacementParticle!=null)
                        {
                            ParticleSystem instance = GameObject.Instantiate(replacementParticle.gameObject, vanillaParticle.transform).GetComponent<ParticleSystem>();
                            instance.transform.localPosition = Vector3.zero;
                            vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                            instance.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                            if (vanillaParticle.isPlaying != instance.isPlaying)
                            {
                                if (vanillaParticle.isPlaying)
                                {
                                    instance.Play();
                                }
                                else
                                {
                                    instance.Stop();
                                }
                            }
                            vanillaParticleRef = instance;
                        }
                        else
                        {
                            vanillaParticleRef.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                            if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Replacement particle was null. Vanilla particle will be hidden."); }
                        }
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Renderer was null for Particle Action. Skipping Apply."); }
            }
            return vanillaParticle;
        }

        public ParticleSystem RemoveSubEmitter(ParticleSystem vanillaParticleRef, int subEmitterIndex, ParticleSystem vanillaSubEmitter)
        {
            ParticleSystem replacementSubEmitter = vanillaParticleRef?.subEmitters.GetSubEmitterSystem(subEmitterIndex);
            if (vanillaSubEmitter != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.RETAIN:
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaSubEmitter.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                        break;
                    case ParticleSystemActionType.REPLACE:
                        vanillaParticleRef.subEmitters.SetSubEmitterSystem(subEmitterIndex, vanillaSubEmitter);
                        if (replacementSubEmitter != null)
                        {
                            if (replacementSubEmitter.isPlaying != vanillaSubEmitter.isPlaying)
                            {
                                if(replacementSubEmitter.isPlaying)
                                {
                                    vanillaSubEmitter.Play();
                                }
                                else
                                {
                                    vanillaSubEmitter.Stop();
                                }
                            }
                            GameObject.Destroy(replacementSubEmitter.gameObject);
                        }
                        break;
                }
                return vanillaSubEmitter;
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Could not find particle sub-emitter at index specified. Skipping."); }
                return null;
            }
        }

        public void Remove(ParticleSystem vanillaParticle, ParticleSystem replacementSystem)
        {
            if (vanillaParticle?.gameObject?.GetComponent<ParticleSystemRenderer>() != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.RETAIN:
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                        break;
                    case ParticleSystemActionType.REPLACE:
                        if(replacementSystem!=null)
                        {
                            if(replacementSystem.isPlaying != vanillaParticle.isPlaying)
                            {
                                if (replacementSystem.isPlaying)
                                {
                                    vanillaParticle.Play();
                                }
                                else
                                {
                                    vanillaParticle.Stop();
                                }
                            }
                            GameObject.Destroy(replacementSystem);
                        }
                        vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Renderer was null for Particle Action. Skipping Remove."); }
            }
        }

        public void RemoveRef(ref ParticleSystem vanillaParticleRef, ParticleSystem vanillaParticle)
        {
            if (vanillaParticleRef?.gameObject?.GetComponent<ParticleSystemRenderer>() != null)
            {
                switch (actionType)
                {
                    case ParticleSystemActionType.REPLACE:
                        ParticleSystem replacementSystem = vanillaParticleRef;
                        if (replacementSystem != null && replacementSystem != vanillaParticle)
                        {
                            replacementSystem.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
                            vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                            if (replacementSystem.isPlaying && !vanillaParticle.isPlaying)
                            {
                                vanillaParticle.Play();
                            }
                            vanillaParticleRef = vanillaParticle;
                            GameObject.Destroy(replacementSystem.gameObject);
                        }
                        else
                        {
                            vanillaParticleRef.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                            if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Replacement particle was null. The only action being taken is showing the vanilla particle."); }
                        }
                        break;
                    case ParticleSystemActionType.HIDE:
                        vanillaParticle.gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
                        break;
                    case ParticleSystemActionType.RETAIN:
                        break;
                }
            }
            else
            {
                if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) { EnemySkinKit.SkinKitLogger.LogWarning("Vanilla Renderer was null for Particle Action. Skipping remove."); }
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
            AudioClip vanillaClip = null;
            if (vanillaSource != null)
            {
                vanillaClip = vanillaSource?.clip;
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
                        if(replacementClips == null)
                        {
                            if (EnemySkinKit.LogLevelSetting >= LogLevel.WARN) EnemySkinKit.SkinKitLogger.LogWarning("Audio list was null on a replace action. This probably means either you're running dev build or you've uninstalled fixplugintypesserialization for some reason.");
                        }
                        else
                        {
                            vanillaSource = replacementClips;
                        }
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