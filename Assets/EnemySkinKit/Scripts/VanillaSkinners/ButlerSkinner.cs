using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ButlerSkinner : BaseSkinner, ButlerEventHandler
    {
        protected const string LOD0_PATH = "MeshContainer/BodyLOD0";
        protected const string LOD1_PATH = "MeshContainer/BodyLOD1";
        protected const string LOD2_PATH = "MeshContainer/BodyLOD2";
        protected const string ANCHOR_PATH = "MeshContainer";
        protected const string BROOM_PATH = "MeshContainer/metarig/spine/Broom";
        protected const string HAIR_PATH = "MeshContainer/metarig/spine/spine.001/NeckContainer/spine.004/face/Hair";
        protected const string TEETH_PATH = "MeshContainer/metarig/spine/spine.001/NeckContainer/spine.004/face/Teeth";
        protected const string MESH_PROPERTY = "BugMesh";
        protected const string TEXTURE_PROPERTY = "BugTexture";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaBroomMaterial;
        protected VanillaMaterial vanillaTeethMaterial;
        protected VanillaMaterial vanillaHairMaterial;
        protected List<GameObject> activeAttachments;
        protected Mesh vanillaBroomMesh;
        protected Mesh vanillaTeethMesh;
        protected Mesh vanillaHairMesh;
        protected VanillaMaterial vanillaBloodSpurtMat;
        protected VanillaMaterial vanillaBloodMat;
        protected VanillaMaterial vanillaPopMat;
        protected ParticleSystem vanillaBloodParticle;
        protected ParticleSystem vanillaPopParticle;

        protected GameObject skinnedMeshReplacement;

        //Hornet crap
        protected ButlerBeesEnemyAI hornets;
        protected GameObject hornetReplacementInstance;
        protected Mesh vanillaHornetMesh;
        protected Texture vanillaHornetTexture;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected AudioReflector modCreatureEffects;
        protected AudioReflector modPopNear;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modAmbience;
        protected AudioReflector modAmbience2;
        protected AudioReflector modBuzzing;
        protected AudioReflector modPopFar;
        protected AudioReflector modSweeping;
        protected AudioReflector modHornetBuzzing;
        protected ParticleSystem vanillaBloodSpurtParticle;

        protected ButlerSkin SkinData { get; }

        public ButlerSkinner(ButlerSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            ButlerEnemyAI butler = enemy.GetComponent<ButlerEnemyAI>();

            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBroomMaterial = SkinData.BroomMaterialAction.Apply(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaTeethMaterial = SkinData.TeethMaterialAction.Apply(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaHairMaterial = SkinData.HairMaterialAction.Apply(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaBroomMesh = SkinData.BroomMeshAction.Apply(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaTeethMesh = SkinData.TeethMeshAction.Apply(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaHairMesh = SkinData.HairMeshAction.Apply(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<MeshFilter>());

            vanillaBloodSpurtMat = SkinData.BloodSpurtMaterialAction.Apply(butler.stabBloodParticle.subEmitters.GetSubEmitterSystem(0).GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBloodMat = SkinData.StabBloodMaterialAction.Apply(butler.stabBloodParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaPopMat = SkinData.PopMaterialAction.Apply(butler.popParticle.GetComponent<ParticleSystemRenderer>(), 0);

            vanillaBloodSpurtParticle = SkinData.BloodSpurtParticleAction.ApplySubEmitter(butler.stabBloodParticle, 0);
            vanillaBloodParticle = SkinData.StabBloodParticleAction.ApplyRef(ref butler.stabBloodParticle);
            vanillaPopParticle = SkinData.PopParticleAction.ApplyRef(ref butler.popParticle);


            SkinData.DefaultAmbienceAudioAction.ApplyToMap(butler.ambience1.clip, clipMap);
            SkinData.BuzzingAmbienceAudioAction.ApplyToMap(butler.buzzingAmbience.clip, clipMap);
            SkinData.FootstepsAudioAction.ApplyToMap(butler.footsteps, clipMap);
            SkinData.SweepsAudioAction.ApplyToMap(butler.broomSweepSFX, clipMap);
            SkinData.PopReverbAudioAction.ApplyToMap(butler.popAudioFar.clip, clipMap);
            SkinData.MurderMusicAudioAction.ApplyToMap(butler.ambience2.clip, clipMap);
            SkinData.PopAudioAction.ApplyToMap(butler.popAudio.clip, clipMap);
            SkinData.StabPlayerAudioAction.ApplyToMap(butler.enemyType.audioClips[0], clipMap);
            SkinData.CoatRustleAudioAction.ApplyToMap(butler.enemyType.audioClips[1], clipMap);
            SkinData.BrandishKnifeAudioAction.ApplyToMap(butler.enemyType.audioClips[2], clipMap);
            SkinData.InflateAudioAction.ApplyToMap(butler.enemyType.audioClips[3], clipMap);
            SkinData.HitBodyAudioAction.ApplyToMap(butler.enemyType.hitBodySFX, clipMap);

            modCreatureVoice = CreateAudioReflector(butler.creatureVoice, clipMap, butler.NetworkObjectId);
            butler.creatureVoice.mute = true;
            modCreatureEffects = CreateAudioReflector(butler.creatureSFX, clipMap, butler.NetworkObjectId);
            butler.creatureSFX.mute = true;
            modAmbience = CreateAudioReflector(butler.ambience1, clipMap, butler.NetworkObjectId);
            butler.ambience1.mute = true;
            modAmbience2 = CreateAudioReflector(butler.ambience2, clipMap, butler.NetworkObjectId);
            butler.ambience2.mute = true;
            modBuzzing = CreateAudioReflector(butler.buzzingAmbience, clipMap, butler.NetworkObjectId);
            butler.buzzingAmbience.mute = true;
            modPopNear = CreateAudioReflector(butler.popAudio, clipMap, butler.NetworkObjectId);
            butler.popAudio.mute = true;
            modPopFar = CreateAudioReflector(butler.popAudioFar, clipMap, butler.NetworkObjectId);
            butler.popAudioFar.mute = true;
            modSweeping = CreateAudioReflector(butler.sweepingAudio, clipMap, butler.NetworkObjectId);
            butler.sweepingAudio.mute = true;

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>()
                {
                    { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig") },
                    { "NeckContainer", enemy.transform.Find($"{ANCHOR_PATH}/metarig/spine/spine.001/NeckContainer") } 
                }
            );
            ApplySkinToHornets();
            EnemySkinRegistry.RegisterEnemyEventHandler(butler, this);
        }

        public override void Remove(GameObject enemy)
        {
            ButlerEnemyAI butler = enemy.GetComponent<ButlerEnemyAI>();
            RemoveSkinFromHornets();
            EnemySkinRegistry.RemoveEnemyEventHandler(butler, this);
            DestroyAudioReflector(modCreatureVoice);
            butler.creatureVoice.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            butler.creatureSFX.mute = false;
            DestroyAudioReflector(modAmbience);
            butler.ambience1.mute = false;
            DestroyAudioReflector(modAmbience2);
            butler.ambience2.mute = false;
            DestroyAudioReflector(modBuzzing);
            butler.buzzingAmbience.mute = false;
            DestroyAudioReflector(modPopNear);
            butler.popAudio.mute = false;
            DestroyAudioReflector(modPopFar);
            butler.popAudioFar.mute = true;
            DestroyAudioReflector(modSweeping);
            butler.sweepingAudio.mute = true;

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BroomMaterialAction.Remove(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBroomMaterial);
            SkinData.TeethMaterialAction.Remove(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaTeethMaterial);
            SkinData.HairMaterialAction.Remove(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaHairMaterial);
            SkinData.BroomMeshAction.Remove(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaBroomMesh);
            SkinData.TeethMeshAction.Remove(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaTeethMesh);
            SkinData.HairMeshAction.Remove(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaHairMesh);

            SkinData.StabBloodParticleAction.RemoveRef(ref butler.stabBloodParticle, vanillaBloodParticle);
            SkinData.PopParticleAction.RemoveRef(ref butler.popParticle, vanillaPopParticle);
            SkinData.BloodSpurtParticleAction.RemoveSubEmitter(butler.stabBloodParticle, 0, vanillaBloodSpurtParticle);

            SkinData.StabBloodMaterialAction.Remove(butler.stabBloodParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodMat);
            SkinData.PopMaterialAction.Remove(butler.popParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaPopMat);
            SkinData.BloodSpurtMaterialAction.Remove(butler.stabBloodParticle.subEmitters.GetSubEmitterSystem(0).GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurtMat);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }

        public void OnSpawnHornets(ButlerEnemyAI instance, ButlerBeesEnemyAI hornets)
        {
            this.hornets = hornets;
            ApplySkinToHornets();
        }

        private void ApplySkinToHornets()
        {
            if(hornets != null)
            {
                VisualEffect hornetEffect = hornets.transform.Find("BugSwarmParticle").GetComponent<VisualEffect>();
                if (SkinData.HornetReplacementPrefab != null)
                {
                    hornetReplacementInstance = GameObject.Instantiate(SkinData.HornetReplacementPrefab, hornets.transform);
                    hornetReplacementInstance.transform.localPosition = Vector3.zero;
                    hornetEffect.enabled = false;
                }
                else
                {
                    vanillaHornetTexture = SkinData.HornetTextureAction.ApplyToVisualEffect(hornetEffect, TEXTURE_PROPERTY);
                    vanillaHornetMesh = SkinData.HornetMeshAction.ApplyToVisualEffect(hornetEffect, MESH_PROPERTY);
                }

                SkinData.HornetBuzzAudioAction.ApplyToMap(hornets.buzzing.clip, clipMap);
                modHornetBuzzing = CreateAudioReflector(hornets.buzzing, clipMap, hornets.NetworkObjectId);
                hornets.buzzing.mute = true;
            }
        }

        private void RemoveSkinFromHornets()
        {
            if (hornets != null)
            {
                VisualEffect hornetEffect = hornets.transform.Find("BugSwarmParticle").GetComponent<VisualEffect>();
                if (hornetReplacementInstance != null)
                {
                    GameObject.Destroy(hornetReplacementInstance);
                    hornetEffect.enabled = true;
                }
                else
                {
                    SkinData.HornetTextureAction.RemoveFromVisualEffect(hornetEffect, TEXTURE_PROPERTY, vanillaHornetTexture);
                    SkinData.HornetMeshAction.RemoveFromVisualEffect(hornetEffect, vanillaHornetMesh, MESH_PROPERTY);
                }
                DestroyAudioReflector(modHornetBuzzing);
                hornets.buzzing.mute = false;
            }
        }
    }
}