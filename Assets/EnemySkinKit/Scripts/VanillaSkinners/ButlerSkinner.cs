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
        protected AudioClip vanillaAmbience;
        protected AudioClip vanillaBuzzingAmbience;
        //protected AudioSource sweepingAudio;
        protected AudioClip[] vanillaFootsteps;
        protected AudioClip[] vanillaBroomSweeps;
        protected AudioClip vanillaPopReverb;
        protected AudioClip vanillaMurderMusic;
        protected AudioClip vanillaPopAudio;
        protected GameObject skinnedMeshReplacement;

        //Hornet crap
        protected ButlerBeesEnemyAI hornets;
        protected GameObject hornetReplacementInstance;
        protected Mesh vanillaHornetMesh;
        protected Texture vanillaHornetTexture;
        protected AudioClip vanillaHornetBuzzAudio;

        protected bool PopSilenced => SkinData.InflateAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN ||
            SkinData.BrandishKnifeAudioAction.actionType != AudioActionType.RETAIN ||
            SkinData.CoatRustleAudioAction.actionType != AudioActionType.RETAIN ||
            SkinData.StabPlayerAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modPopNear;
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


            vanillaAmbience = SkinData.DefaultAmbienceAudioAction.ApplyToSource(butler.ambience1);
            vanillaBuzzingAmbience = SkinData.BuzzingAmbienceAudioAction.ApplyToSource(butler.buzzingAmbience);
            vanillaFootsteps = SkinData.FootstepsAudioAction.Apply(ref butler.footsteps);
            vanillaBroomSweeps = SkinData.SweepsAudioAction.Apply(ref butler.broomSweepSFX);
            vanillaPopReverb = SkinData.PopReverbAudioAction.ApplyToSource(butler.popAudioFar);
            vanillaMurderMusic = SkinData.MurderMusicAudioAction.ApplyToSource(butler.ambience2);
            vanillaPopAudio = SkinData.PopAudioAction.ApplyToSource(butler.popAudio);

            if (PopSilenced)
            {
                modPopNear = CreateModdedAudioSource(butler.popAudio, "modPopAudio");
                butler.popAudio.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(butler.creatureSFX, "modEffects");
                butler.creatureSFX.mute = true;
            }

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
            if (PopSilenced)
            {
                DestroyModdedAudioSource(butler.popAudio);
                butler.popAudio.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(butler.creatureSFX);
                butler.creatureSFX.mute = false;
            }

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

            SkinData.DefaultAmbienceAudioAction.RemoveFromSource(butler.ambience1, vanillaAmbience);
            SkinData.BuzzingAmbienceAudioAction.RemoveFromSource(butler.buzzingAmbience, vanillaBuzzingAmbience);
            SkinData.FootstepsAudioAction.Remove(ref butler.footsteps, vanillaFootsteps);
            SkinData.SweepsAudioAction.Remove(ref butler.broomSweepSFX, vanillaBroomSweeps);
            SkinData.PopAudioAction.RemoveFromSource(butler.popAudioFar, vanillaPopReverb);
            SkinData.MurderMusicAudioAction.RemoveFromSource(butler.ambience2, vanillaMurderMusic);

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

        public void OnStep(ButlerEnemyAI instance)
        {
            if(EffectsSilenced)
            {
                AudioClip[] stepClips = SkinData.FootstepsAudioAction.WorkingClips(vanillaFootsteps);
                AudioClip stepClip = stepClips[UnityEngine.Random.Range(0, stepClips.Length)];
                modCreatureEffects.PlayOneShot(stepClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, stepClip);
            }
            
        }

        public void OnSweep(ButlerEnemyAI instance)
        {
            if(EffectsSilenced)
            {
                AudioClip[] sweepClips = SkinData.SweepsAudioAction.WorkingClips(vanillaBroomSweeps);
                AudioClip sweepClip = sweepClips[UnityEngine.Random.Range(0, sweepClips.Length)];
                modCreatureEffects.PlayOneShot(sweepClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, sweepClip);
            }
            
        }

        public void OnEnterSweepeingState(ButlerEnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
            }
        }

        public void OnEnterPremeditatingState(EnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
            }
        }

        public void OnEnterMurderingState(ButlerEnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.BrandishKnifeAudioAction.WorkingClip(instance.enemyType.audioClips[2]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.BrandishKnifeAudioAction.WorkingClip(instance.enemyType.audioClips[2]));
            }
        }

        public void OnPop(ButlerEnemyAI instance)
        {
            if(PopSilenced)
            {
                modPopNear.Play();
            }
        }

        public void OnSpawnHornets(ButlerEnemyAI instance, ButlerBeesEnemyAI hornets)
        {
            this.hornets = hornets;
            ApplySkinToHornets();
        }

        public void OnInflate(ButlerEnemyAI instance)
        {
            if(PopSilenced)
            {
                modPopNear.PlayOneShot(SkinData.InflateAudioAction.WorkingClip(instance.enemyType.audioClips[3]));
            }
        }

        public void OnStabPlayer(ButlerEnemyAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.StabPlayerAudioAction.WorkingClip(instance.enemyType.audioClips[0]));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
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
                vanillaHornetBuzzAudio = SkinData.HornetBuzzAudioAction.ApplyToSource(hornets.buzzing);
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
                SkinData.HornetBuzzAudioAction.RemoveFromSource(hornets.buzzing, vanillaHornetBuzzAudio);
            }
        }
    }
}