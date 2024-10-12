using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ManeaterSkinner : BaseSkinner, ManeaterEventHandler
    {
        protected const string BABY_ANCHOR_PATH = "BabyMeshContainer";
        protected const string BABY_WALK_AUDIO = "BabyWalkSFX";
        protected const string BABY_MESH_PATH = "BabyMeshContainer/BabyManeaterMesh";
        protected const string BABY_ANIM_EVENTS_PATH = "BabyMeshContainer/BabyAnimContainer";
        protected const string FLESH_PULL_PATH = "MeshContainer/AnimContainer/RagdollPoint/FleshPull (1)";
        protected const string ADULT_ANCHOR_PATH = "MeshContainer";
        protected const string ADULT_MESH_PATH = "MeshContainer/Mesh";
        protected const string ADULT_ANIM_EVENTS_PATH = "MeshContainer/AnimContainer";
        protected const string TRANSFORM_PARTICLE_PATH = "MeshContainer/AnimContainer/Spine/BloodSpurtParticle1 (1)";
        protected const string CRY_PARTICLE_2 = "CryParticle2";
        protected const string BLOOD_SECONDARY_PARTICLE = "BloodParticle";
        protected const string BLOOD_SPURT_PARTICLE = "BloodSpurtParticle";

        protected VanillaMaterial vanillaBabyMaterial;
        protected GameObject replacementBabyBodyMesh;
        protected VanillaMaterial vanillaTearsMaterial;
        protected ParticleSystem vanillaTearsParticle;
        protected VanillaMaterial vanillaPukeMaterial;
        protected ParticleSystem vanillaPukeParticle;
        
        protected VanillaMaterial vanillaAdultMaterial;
        protected VanillaMaterial vanillaDeadPlayerFleshMaterial;
        protected GameObject replacementAdultBodyMesh;
        protected Mesh vanillaDeadPlayerFleshMesh;
        protected VanillaMaterial vanillaBloodSpurtMaterial;
        protected ParticleSystem vanillaBloodSpurtParticle;
        protected VanillaMaterial vanillaBloodSpurt2Material;
        protected ParticleSystem vanillaBloodSpurt2Particle;
        protected VanillaMaterial vanillaTransformationGooMaterial;
        protected ParticleSystem vanillaTransformationGooParticle;

        protected List<GameObject> babyAttachments;
        protected List<GameObject> adultAttachments;

        protected ManeaterSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        public ManeaterSkinner(ManeaterSkin skinData)
        {
            SkinData = skinData;
        }

        protected SkinnedMeshRenderer vanillaBabyMesh;
        protected SkinnedMeshRenderer vanillaAdultMesh;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modBabyVoice;
        protected AudioReflector modbabyWalkAudio;
        protected AudioReflector modCreatureEffects;
        protected AudioReflector modClickingAudio1;
        protected AudioReflector modClickingAudio2;
        protected AudioReflector modWalkingAudio;
        protected AudioReflector modBabyCryingAudio;
        protected AudioReflector modScreamAudio;
        protected AudioReflector modScreamAudioNonDiagetic;

        public override void Apply(GameObject enemy)
        {
            CaveDwellerAI maneater = enemy.GetComponent<CaveDwellerAI>();

            PlayAudioAnimationEvent babyAudioAnimEvents = enemy.transform.Find(BABY_ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if(babyAudioAnimEvents != null)
            {
                SkinData.BabyFootstepsAudioListAction.ApplyToMap(babyAudioAnimEvents.randomClips, clipMap);
            }
            PlayAudioAnimationEvent adultAudioAnimEvents = enemy.transform.Find(ADULT_ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if(adultAudioAnimEvents != null)
            {
                vanillaTransformationGooMaterial = SkinData.TransformationGooMaterialAction.Apply(adultAudioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
                SkinData.TransformationGooMaterialAction.Apply(adultAudioAnimEvents.particle.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
                SkinData.TransformationGooMaterialAction.Apply(adultAudioAnimEvents.particle.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
                vanillaTransformationGooParticle = SkinData.TransformationGooParticleAction.ApplyRef(ref adultAudioAnimEvents.particle);
            }

            SkinData.ClickingMandiblesAudioAction.ApplyToMap(maneater.clickingAudio1.clip, clipMap);
            SkinData.BuzzingAudioAction.ApplyToMap(maneater.clickingAudio2.clip, clipMap);
            SkinData.LeapScreamAudioAction.ApplyToMap(maneater.screamAudio.clip, clipMap);
            SkinData.LeapScreamAudioAction.ApplyToMap(maneater.screamAudioNonDiagetic.clip, clipMap);
            SkinData.AdultWalkingAudioAction.ApplyToMap(maneater.walkingAudio.clip, clipMap);
            SkinData.CryingAudioAction.ApplyToMap(maneater.babyCryingAudio.clip, clipMap);

            SkinData.GrowlAudioAction.ApplyToMap(maneater.growlSFX, clipMap);
            SkinData.FakeCryAudioListAction.ApplyToMap(maneater.fakeCrySFX, clipMap);
            SkinData.CooldownAudioAction.ApplyToMap(maneater.cooldownSFX, clipMap);
            SkinData.SquirmAudioAction.ApplyToMap(maneater.squirmingSFX, clipMap);
            SkinData.TransformAudioAction.ApplyToMap(maneater.transformationSFX, clipMap);
            SkinData.ScaredNoiseAudioListAction.ApplyToMap(maneater.scaredBabyVoiceSFX, clipMap);
            SkinData.BiteAudioAction.ApplyToMap(maneater.biteSFX, clipMap);

            modCreatureVoice = CreateAudioReflector(maneater.creatureVoice, clipMap, maneater.NetworkObjectId);
            maneater.creatureVoice.mute = true;
            modBabyVoice = CreateAudioReflector(maneater.babyVoice, clipMap, maneater.NetworkObjectId);
            maneater.babyVoice.mute = true;
            AudioSource babyWalkAudio = maneater.transform.Find(BABY_WALK_AUDIO).GetComponent<AudioSource>();
            if (babyWalkAudio != null)
            {
                modbabyWalkAudio = CreateAudioReflector(babyWalkAudio, clipMap, maneater.NetworkObjectId);
                babyWalkAudio.mute = true;
            }
            modCreatureEffects = CreateAudioReflector(maneater.creatureSFX, clipMap, maneater.NetworkObjectId); 
            maneater.creatureSFX.mute = true;
            modClickingAudio1 = CreateAudioReflector(maneater.clickingAudio1, clipMap, maneater.NetworkObjectId); 
            maneater.clickingAudio1.mute = true;
            modClickingAudio2 = CreateAudioReflector(maneater.clickingAudio2, clipMap, maneater.NetworkObjectId); 
            maneater.clickingAudio2.mute = true;
            modWalkingAudio = CreateAudioReflector(maneater.walkingAudio, clipMap, maneater.NetworkObjectId); 
            maneater.walkingAudio.mute = true;
            modBabyCryingAudio = CreateAudioReflector(maneater.babyCryingAudio, clipMap, maneater.NetworkObjectId); 
            maneater.babyCryingAudio.mute = true;
            modScreamAudio = CreateAudioReflector(maneater.screamAudio, clipMap, maneater.NetworkObjectId); 
            maneater.screamAudio.mute = true;
            modScreamAudioNonDiagetic = CreateAudioReflector(maneater.screamAudioNonDiagetic, clipMap, maneater.NetworkObjectId); 
            maneater.screamAudioNonDiagetic.mute = true;
            

            vanillaTearsMaterial = SkinData.TearsMaterialAction.Apply(maneater.babyTearsParticle.GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.TearsMaterialAction.Apply(maneater.babyTearsParticle.transform.Find(CRY_PARTICLE_2).GetComponent<ParticleSystemRenderer>(), 0);
            vanillaTearsParticle = SkinData.TearsParticleAction.ApplyRef(ref maneater.babyTearsParticle);

            vanillaPukeMaterial = SkinData.PukeMaterialAction.Apply(maneater.babyFoamAtMouthParticle.GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.PukeMaterialAction.Apply(maneater.babyFoamAtMouthParticle.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
            vanillaPukeParticle = SkinData.PukeParticleAction.ApplyRef(ref maneater.babyFoamAtMouthParticle);
            vanillaBloodSpurtMaterial = SkinData.BloodSpurtMaterialAction.Apply(maneater.killPlayerParticle1.GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.BloodSpurtMaterialAction.Apply(maneater.killPlayerParticle1.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.BloodSpurtMaterialAction.Apply(maneater.killPlayerParticle1.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBloodSpurtParticle = SkinData.BloodSpurtParticleAction.ApplyRef(ref maneater.killPlayerParticle1);
            vanillaBloodSpurt2Material = SkinData.BloodSpurt2MaterialAction.Apply(maneater.killPlayerParticle2.GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.BloodSpurt2MaterialAction.Apply(maneater.killPlayerParticle2.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.BloodSpurt2MaterialAction.Apply(maneater.killPlayerParticle2.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBloodSpurt2Particle = SkinData.BloodSpurt2ParticleAction.ApplyRef(ref maneater.killPlayerParticle2);

            babyAttachments = ArmatureAttachment.ApplyAttachments(SkinData.BabyAttachments, maneater.transform.Find(BABY_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            adultAttachments = ArmatureAttachment.ApplyAttachments(SkinData.AdultAttachments, enemy.transform.Find(ADULT_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            vanillaDeadPlayerFleshMaterial = SkinData.DeadPlayerFleshMaterialAction.Apply(maneater.bodyRagdollPoint.transform.Find(FLESH_PULL_PATH)?.GetComponent<MeshRenderer>(), 0);
            vanillaDeadPlayerFleshMesh = SkinData.DeadPlayerFleshMeshAction.Apply(maneater.bodyRagdollPoint.transform.Find(FLESH_PULL_PATH)?.GetComponent<MeshFilter>());

            vanillaBabyMesh = enemy.transform.Find(BABY_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>();
            vanillaAdultMesh = enemy.transform.Find(ADULT_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>();

            vanillaBabyMaterial = SkinData.BabyMaterialAction.Apply(vanillaBabyMesh, 0);
            vanillaAdultMaterial = SkinData.AdultMaterialAction.Apply(vanillaAdultMesh, 0);
            replacementBabyBodyMesh = SkinData.BabyBodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[] { vanillaBabyMesh }, 
                enemy.transform.Find(BABY_ANCHOR_PATH)
            );
            if (!SkinData.BabyBodyMeshAction.actionType.Equals(SkinnedMeshActionType.RETAIN))
            {
                //LC enables the mesh somehow when the little bugger gets picked up
                vanillaBabyMesh.forceRenderingOff = true;
            }
            replacementAdultBodyMesh = SkinData.AdultBodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[] { vanillaAdultMesh },
                enemy.transform.Find(ADULT_ANCHOR_PATH)
            );
            if (!SkinData.AdultBodyMeshAction.actionType.Equals(SkinnedMeshActionType.RETAIN))
            {
                //LC enables this mesh when its transforming... again I have no clue why since the whole "MeshContainer" is also disabled until the maneater transforms
                //Zeekerss gonna Zeek. If I ever go to PAX or something and he's there I'm going to throw him into a volcano (in minecraft)
                vanillaAdultMesh.forceRenderingOff = true;
            }
            EnemySkinRegistry.RegisterEnemyEventHandler(maneater, this);
        }

        public override void Remove(GameObject enemy)
        {
            CaveDwellerAI maneater = enemy.GetComponent<CaveDwellerAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(maneater, this);

            PlayAudioAnimationEvent adultAudioAnimEvents = enemy.transform.Find(ADULT_ANIM_EVENTS_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (adultAudioAnimEvents != null)
            {
                SkinData.TransformationGooMaterialAction.Remove(adultAudioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaTransformationGooMaterial);
                SkinData.TransformationGooMaterialAction.Remove(adultAudioAnimEvents.particle.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaTransformationGooMaterial);
                SkinData.TransformationGooMaterialAction.Remove(adultAudioAnimEvents.particle.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaTransformationGooMaterial);
                SkinData.TransformationGooParticleAction.RemoveRef(ref adultAudioAnimEvents.particle, vanillaTransformationGooParticle);
            }

            DestroyAudioReflector(modCreatureVoice);
            maneater.creatureVoice.mute = false;
            DestroyAudioReflector(modBabyVoice);
            maneater.babyVoice.mute = false;
            AudioSource babyWalkAudio = maneater.transform.Find(BABY_WALK_AUDIO)?.GetComponent<AudioSource>();
            if (babyWalkAudio != null)
            {
                DestroyAudioReflector(modbabyWalkAudio);
                babyWalkAudio.mute = false;
            }
            DestroyAudioReflector(modCreatureEffects);
            maneater.creatureSFX.mute = false;
            DestroyAudioReflector(modClickingAudio1);
            maneater.clickingAudio1.mute = false;
            DestroyAudioReflector(modClickingAudio2);
            maneater.clickingAudio2.mute = false;
            DestroyAudioReflector(modWalkingAudio);
            maneater.walkingAudio.mute = false;
            DestroyAudioReflector(modBabyCryingAudio);
            maneater.babyCryingAudio.mute = false;
            DestroyAudioReflector(modScreamAudio);
            maneater.screamAudio.mute = false;
            DestroyAudioReflector(modScreamAudioNonDiagetic);
            maneater.screamAudioNonDiagetic.mute = false;

            SkinData.TearsMaterialAction.Remove(maneater.babyTearsParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaTearsMaterial);
            SkinData.TearsMaterialAction.Remove(maneater.babyTearsParticle.transform.Find(CRY_PARTICLE_2).GetComponent<ParticleSystemRenderer>(), 0, vanillaTearsMaterial);
            SkinData.TearsParticleAction.RemoveRef(ref maneater.babyTearsParticle, vanillaTearsParticle);

            SkinData.PukeMaterialAction.Remove(maneater.babyFoamAtMouthParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaPukeMaterial);
            SkinData.PukeMaterialAction.Remove(maneater.babyFoamAtMouthParticle.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaPukeMaterial);
            SkinData.PukeParticleAction.RemoveRef(ref maneater.babyFoamAtMouthParticle, vanillaPukeParticle);
            SkinData.BloodSpurtMaterialAction.Remove(maneater.killPlayerParticle1.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurtMaterial);
            SkinData.BloodSpurtMaterialAction.Remove(maneater.killPlayerParticle1.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurtMaterial);
            SkinData.BloodSpurtMaterialAction.Remove(maneater.killPlayerParticle1.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurtMaterial);
            SkinData.BloodSpurtParticleAction.RemoveRef(ref maneater.killPlayerParticle1, vanillaBloodSpurtParticle);
            SkinData.BloodSpurt2MaterialAction.Remove(maneater.killPlayerParticle2.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurt2Material);
            SkinData.BloodSpurt2MaterialAction.Remove(maneater.killPlayerParticle2.transform.Find(BLOOD_SECONDARY_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurt2Material);
            SkinData.BloodSpurt2MaterialAction.Remove(maneater.killPlayerParticle2.transform.Find(BLOOD_SPURT_PARTICLE).GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodSpurt2Material);
            SkinData.BloodSpurt2ParticleAction.RemoveRef(ref maneater.killPlayerParticle2, vanillaBloodSpurt2Particle);

            ArmatureAttachment.RemoveAttachments(babyAttachments);
            ArmatureAttachment.RemoveAttachments(adultAttachments);

            SkinData.DeadPlayerFleshMaterialAction.Remove(maneater.bodyRagdollPoint.transform.Find(FLESH_PULL_PATH)?.GetComponent<MeshRenderer>(), 0, vanillaDeadPlayerFleshMaterial);
            SkinData.DeadPlayerFleshMeshAction.Remove(maneater.bodyRagdollPoint.transform.Find(FLESH_PULL_PATH)?.GetComponent<MeshFilter>(), vanillaDeadPlayerFleshMesh);

            SkinData.BabyMaterialAction.Remove(vanillaBabyMesh, 0, vanillaBabyMaterial);
            SkinData.AdultMaterialAction.Remove(vanillaAdultMesh, 0, vanillaAdultMaterial);
            SkinData.BabyBodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[] { enemy.transform.Find(BABY_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() },
                replacementBabyBodyMesh
            );
            if (!SkinData.BabyBodyMeshAction.actionType.Equals(SkinnedMeshActionType.RETAIN))
            {
                vanillaBabyMesh.forceRenderingOff = false;
            }
            SkinData.AdultBodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[] { enemy.transform.Find(ADULT_MESH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() },
                replacementAdultBodyMesh
            );
            if (!SkinData.AdultBodyMeshAction.actionType.Equals(SkinnedMeshActionType.RETAIN))
            {
                vanillaAdultMesh.forceRenderingOff = false;
            }

        }
    }
}