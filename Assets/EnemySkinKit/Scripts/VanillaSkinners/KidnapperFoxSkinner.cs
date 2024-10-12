using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class KidnapperFoxSkinner : BaseSkinner, KidnapperFoxEventHandler
    {
        protected const string LOD0_PATH = "Mesh/BodyLOD0";
        protected const string LOD1_PATH = "Mesh/BodyLOD1";
        protected const string LOD2_PATH = "Mesh/BodyLOD2";
        protected const string ANCHOR_PATH = "AnimContainer/metarigContainer";
        protected const string AUDIO_ANIM_PATH = "AnimContainer";
        protected const string DIFFUSE_ID = "_Diffuse";
        protected const string FUR_MASK_ID = "_Fur_Mask";

        protected VanillaMaterial vanillaFurMaterial;
        protected Texture vanillaDiffuseTexture;
        protected Texture vanillaFurMask;
        protected VanillaMaterial vanillaTeethMaterial;

        protected GameObject skinnedMeshReplacement;

        protected List<GameObject> activeAttachments;
        private VanillaMaterial vanillaBloodMaterial;
        private VanillaMaterial vanillaDroolMaterial;

        protected AudioReflector modCreatureVoice;


        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        private ParticleSystem vanillaBloodParticle;
        private ParticleSystem vanillaDroolParticle;
        private AudioReflector modCreatureEffects;
        private AudioReflector modGrowlAudio;
        private AudioReflector modCallClose;
        private AudioReflector modCallFar;
        private AudioReflector modTongueAudio;

        protected KidnapperFoxSkin SkinData { get; }

        public KidnapperFoxSkinner(KidnapperFoxSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            BushWolfEnemy fox = enemy.GetComponent<BushWolfEnemy>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(AUDIO_ANIM_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if(audioAnimEvents!=null)
            {
                SkinData.FootstepsAudioListAction.ApplyToMap(audioAnimEvents.randomClips, clipMap);
            }

            SkinData.HitAudioAction.ApplyToMap(fox.hitBushWolfSFX, clipMap);
            SkinData.DieAudioAction.ApplyToMap(fox.dieSFX, clipMap);
            SkinData.DragSnarlAudioAction.ApplyToMap(fox.snarlSFX, clipMap);
            SkinData.GrowlAudioListAction.ApplyToMap(fox.growlSFX, clipMap);
            SkinData.ShootTongueAudioAction.ApplyToMap(fox.shootTongueSFX, clipMap);
            SkinData.TongueShootingAudioAction.ApplyToMap(fox.tongueShootSFX, clipMap);
            SkinData.KillPlayerAudioAction.ApplyToMap(fox.killSFX, clipMap);
            SkinData.NearCallAudioListAction.ApplyToMap(fox.callsClose, clipMap);
            SkinData.FarCallAudioListAction.ApplyToMap(fox.callsFar, clipMap);

            modCreatureEffects = CreateAudioReflector(fox.creatureSFX, clipMap, fox.NetworkObjectId); 
            fox.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(fox.creatureVoice, clipMap, fox.NetworkObjectId); 
            fox.creatureVoice.mute = true;
            modGrowlAudio = CreateAudioReflector(fox.growlAudio, clipMap, fox.NetworkObjectId); 
            fox.growlAudio.mute = true;
            modCallClose = CreateAudioReflector(fox.callClose, clipMap, fox.NetworkObjectId); 
            fox.callClose.mute = true;
            modCallFar = CreateAudioReflector(fox.callFar, clipMap, fox.NetworkObjectId); 
            fox.callFar.mute = true;
            modTongueAudio = CreateAudioReflector(fox.tongueAudio, clipMap, fox.NetworkObjectId); 
            fox.tongueAudio.mute = true;

            //important this step goes before material action application
            vanillaDiffuseTexture = SkinData.DiffuseTextureAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID);
            SkinData.DiffuseTextureAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID);
            SkinData.DiffuseTextureAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID);

            vanillaFurMask = SkinData.FurMaskTextureAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID);
            SkinData.FurMaskTextureAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID);
            SkinData.FurMaskTextureAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID);

            vanillaFurMaterial = SkinData.FurMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.FurMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.FurMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaTeethMaterial = SkinData.TeethMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            SkinData.TeethMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 1);
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            vanillaBloodMaterial = SkinData.BloodSpurtMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaDroolMaterial = SkinData.DroolMaterialAction.Apply(fox.spitParticle.GetComponent<ParticleSystemRenderer>(), 0);
            foreach (ParticleSystemRenderer rend in fox.spitParticle.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                SkinData.DroolMaterialAction.Apply(rend, 0);
            }

            vanillaBloodParticle = SkinData.BloodSpurtParticleAction.ApplyRef(ref audioAnimEvents.particle);
            vanillaDroolParticle = SkinData.DroolParticleAction.ApplyRef(ref fox.spitParticle);
            if (SkinData.DroolParticleAction.actionType == ParticleSystemActionType.HIDE)
            {
                foreach (ParticleSystemRenderer rend in fox.spitParticle.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    rend.enabled = false;
                }
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
                new Dictionary<string, Transform>
                {
                    { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig")}
                }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(fox, this);
        }

        public override void Remove(GameObject enemy)
        {
            BushWolfEnemy fox = enemy.GetComponent<BushWolfEnemy>();

            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(AUDIO_ANIM_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();

            SkinData.FurMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaFurMaterial);
            SkinData.FurMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaFurMaterial);
            SkinData.FurMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaFurMaterial);
            SkinData.TeethMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaTeethMaterial);
            SkinData.TeethMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 1, vanillaTeethMaterial);

            //important that this step comes after the material action handling
            SkinData.FurMaskTextureAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID, vanillaFurMask);
            SkinData.FurMaskTextureAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID, vanillaFurMask);
            SkinData.FurMaskTextureAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>().material, FUR_MASK_ID, vanillaFurMask);

            SkinData.DiffuseTextureAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID, vanillaDiffuseTexture);
            SkinData.DiffuseTextureAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID, vanillaDiffuseTexture);
            SkinData.DiffuseTextureAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>().material, DIFFUSE_ID, vanillaDiffuseTexture);

            SkinData.BloodSpurtMaterialAction.Apply(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0);
            SkinData.DroolMaterialAction.Apply(fox.spitParticle.GetComponent<ParticleSystemRenderer>(), 0);
            foreach (ParticleSystemRenderer rend in fox.spitParticle.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                SkinData.DroolMaterialAction.Apply(rend, 0);
            }

            SkinData.BloodSpurtParticleAction.RemoveRef(ref audioAnimEvents.particle, vanillaBloodParticle);
            SkinData.DroolParticleAction.RemoveRef(ref fox.spitParticle, vanillaDroolParticle);
            if (SkinData.DroolParticleAction.actionType == ParticleSystemActionType.HIDE)
            {
                foreach (ParticleSystemRenderer rend in fox.spitParticle.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    rend.enabled = true;
                }
            }

            SkinData.BloodSpurtMaterialAction.Remove(audioAnimEvents.particle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBloodMaterial);
            SkinData.DroolMaterialAction.Remove(fox.spitParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaDroolMaterial);
            foreach (ParticleSystemRenderer rend in fox.spitParticle.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                SkinData.DroolMaterialAction.Remove(rend, 0, vanillaDroolMaterial);
            }

            ArmatureAttachment.RemoveAttachments(activeAttachments);

            DestroyAudioReflector(modCreatureEffects);
            fox.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            fox.creatureVoice.mute = false;
            DestroyAudioReflector(modGrowlAudio);
            fox.growlAudio.mute = false;
            DestroyAudioReflector(modCallClose);
            fox.callClose.mute = false;
            DestroyAudioReflector(modCallFar);
            fox.callFar.mute = false;
            DestroyAudioReflector(modTongueAudio);
            fox.tongueAudio.mute = false;

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
            EnemySkinRegistry.RemoveEnemyEventHandler(fox, this);
        }

    }
}
