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

        protected AudioClip vanillaHitAudio;
        protected AudioClip vanillaDieAudio;
        protected AudioClip vanillaSnarlAudio;
        protected AudioClip[] vanillaGrowlAudioList;
        protected AudioClip vanillaShootTongueAudio;
        protected AudioClip vanillaTongueShootingAudio;
        protected AudioClip vanillaKillAudio;
        protected AudioClip[] vanillaNearCallAudioList;
        protected AudioClip[] vanillaFarCallAudioList;
        protected AudioClip[] vanillaFootstepsAudioList;
        protected GameObject skinnedMeshReplacement;

        protected List<GameObject> activeAttachments;
        private VanillaMaterial vanillaBloodMaterial;
        private VanillaMaterial vanillaDroolMaterial;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureVoice;
        private ParticleSystem vanillaBloodParticle;
        private ParticleSystem vanillaDroolParticle;

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
                vanillaFootstepsAudioList = SkinData.FootstepsAudioListAction.Apply(ref audioAnimEvents.randomClips);
            }
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(fox.creatureVoice, "modVoice");
                fox.creatureVoice.mute = true;
            }

            vanillaHitAudio = SkinData.HitAudioAction.Apply(ref fox.hitBushWolfSFX);
            vanillaDieAudio = SkinData.DieAudioAction.Apply(ref fox.dieSFX);
            vanillaSnarlAudio = SkinData.DragSnarlAudioAction.Apply(ref fox.snarlSFX);
            vanillaGrowlAudioList = SkinData.GrowlAudioListAction.Apply(ref fox.growlSFX);
            vanillaShootTongueAudio = SkinData.ShootTongueAudioAction.Apply(ref fox.shootTongueSFX);
            vanillaTongueShootingAudio = SkinData.TongueShootingAudioAction.Apply(ref fox.tongueShootSFX);
            vanillaKillAudio = SkinData.KillPlayerAudioAction.Apply(ref fox.killSFX);
            vanillaNearCallAudioList = SkinData.NearCallAudioListAction.Apply(ref fox.callsClose);
            vanillaFarCallAudioList = SkinData.FarCallAudioListAction.Apply(ref fox.callsFar);

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
            if (audioAnimEvents != null)
            {
                SkinData.FootstepsAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaFootstepsAudioList);
            }

            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                fox.creatureVoice.mute = false;
            }

            SkinData.HitAudioAction.Remove(ref fox.hitBushWolfSFX, vanillaHitAudio);
            SkinData.DieAudioAction.Remove(ref fox.dieSFX, vanillaDieAudio);
            SkinData.DragSnarlAudioAction.Remove(ref fox.snarlSFX, vanillaSnarlAudio);
            SkinData.GrowlAudioListAction.Remove(ref fox.growlSFX, vanillaGrowlAudioList);
            SkinData.ShootTongueAudioAction.Remove(ref fox.shootTongueSFX, vanillaShootTongueAudio);
            SkinData.TongueShootingAudioAction.Remove(ref fox.tongueShootSFX, vanillaTongueShootingAudio);
            SkinData.KillPlayerAudioAction.Remove(ref fox.killSFX, vanillaKillAudio);
            SkinData.NearCallAudioListAction.Remove(ref fox.callsClose, vanillaNearCallAudioList);
            SkinData.FarCallAudioListAction.Remove(ref fox.callsFar, vanillaFarCallAudioList);

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

        public void OnCancelReelingPlayer(BushWolfEnemy fox, bool wasDragging)
        {
            if(VoiceSilenced)
            {
                if (wasDragging) 
                { 
                    modCreatureVoice?.Stop(); 
                }
            }
        }

        public void OnLandedTongueShot(BushWolfEnemy fox, PlayerControllerB draggingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice?.PlayOneShot(SkinData.DragSnarlAudioAction.WorkingClip(vanillaSnarlAudio));
            }
        }

        public void OnTongueHit(BushWolfEnemy fox)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice?.PlayOneShot(SkinData.HitAudioAction.WorkingClip(vanillaHitAudio));
            }
        }

        public void OnTongueShot(BushWolfEnemy fox)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice?.PlayOneShot(SkinData.ShootTongueAudioAction.WorkingClip(vanillaShootTongueAudio));
            }
        }

        public void OnHit(EnemyAI enemy, PlayerControllerB attackingPlayer, bool playHitSoundEffect)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice?.PlayOneShot(SkinData.HitAudioAction.WorkingClip(vanillaHitAudio));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if (VoiceSilenced && modCreatureVoice != null)
            {
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, SkinData.DieAudioAction.WorkingClip(vanillaDieAudio));
            }
        }

        public void OnStun(EnemyAI enemy, PlayerControllerB attackingPlayer)
        {
            if (VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

    }
}
