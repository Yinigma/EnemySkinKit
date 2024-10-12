using AntlerShed.EnemySkinKit.AudioReflection;
using AntlerShed.EnemySkinKit.SkinAction;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class EarthLeviathanSkinner : BaseSkinner
    {
        protected const string BODY_PATH = "MeshContainer/Renderer";
        protected const string ANCHOR_PATH = "MeshContainer/Armature";
        protected const string SUBMERGE_SHOCKWAVE_PATH = "MeshContainer/Armature/EmergeParticle/EnterGround/Shockwave";
        protected const string EMERGE_SHOCKWAVE_PATH = "MeshContainer/Armature/EmergeParticle/AppearFromGround2/Shockwave";

        protected VanillaMaterial vanillaBodyMaterial;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;
        protected ParticleSystem vanillaPreEmergeParticle;
        protected ParticleSystem vanillaEmergeParticle;
        protected ParticleSystem replacementShock;
        protected ParticleSystem vanillaSubmergeParticle;
        protected ParticleSystem replacementSubShock;
        protected VanillaMaterial vanillaPreEmergeParticleMaterial;
        protected VanillaMaterial vanillaEmergeParticleMaterial;
        protected VanillaMaterial vanillaShockMaterial;
        protected VanillaMaterial vanillaSubmergeParticleMaterial;
        protected VanillaMaterial vanillaSubShockMaterial;

        protected EarthLeviathanSkin SkinData { get; }

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        protected AudioReflector modGround;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modCreatureEffects;

        public EarthLeviathanSkinner( EarthLeviathanSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            SandWormAI worm = enemy.GetComponent<SandWormAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            
            SkinData.AmbientRumbleAudioListAction.ApplyToMap(worm.ambientRumbleSFX, clipMap);
            SkinData.GroundRumbleAudioListAction.ApplyToMap(worm.groundRumbleSFX, clipMap);
            SkinData.RoarAudioListAction.ApplyToMap(worm.roarSFX, clipMap);
            SkinData.EmergeAudioAction.ApplyToMap(worm.emergeFromGroundSFX, clipMap);
            SkinData.HitGroundAudioAction.ApplyToMap(worm.hitGroundSFX, clipMap);

            ParticleSystem vanillaEmergeShockwave = worm.transform.Find(EMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaSubmergeShockwave = worm.transform.Find(SUBMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();

            modGround = CreateAudioReflector(worm.groundAudio, clipMap, worm.NetworkObjectId);
            worm.groundAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(worm.creatureSFX, clipMap, worm.NetworkObjectId);
            worm.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(worm.creatureVoice, clipMap, worm.NetworkObjectId);
            worm.creatureVoice.mute = true;

            vanillaPreEmergeParticleMaterial = SkinData.PreEmergeParticleMaterialAction.Apply(worm.emergeFromGroundParticle1.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaEmergeParticleMaterial = SkinData.EmergeParticleMaterialAction.Apply(worm.emergeFromGroundParticle2.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaShockMaterial = SkinData.EmergeShockwaveParticleMaterialAction.Apply(vanillaEmergeShockwave?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSubmergeParticleMaterial = SkinData.SubmergeParticleMaterialAction.Apply(worm.hitGroundParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaSubShockMaterial = SkinData.SubmergeShockwaveParticleMaterialAction.Apply(vanillaSubmergeShockwave?.GetComponent<ParticleSystemRenderer>(), 0);

            vanillaPreEmergeParticle = SkinData.PreEmergeParticleAction.ApplyRef(ref worm.emergeFromGroundParticle1);
            vanillaEmergeParticle = SkinData.EmergeParticleAction.ApplyRef(ref worm.emergeFromGroundParticle2);
            replacementShock = SkinData.EmergeShockwaveParticleAction.Apply(vanillaEmergeShockwave);
            vanillaSubmergeParticle = SkinData.SubmergeParticleAction.ApplyRef(ref worm.hitGroundParticle);
            replacementSubShock = SkinData.SubmergeShockwaveParticleAction.Apply(vanillaSubmergeShockwave);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[] 
                { 
                    enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() 
                }, 
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public override void Remove(GameObject enemy)
        {
            SandWormAI worm = enemy.GetComponent<SandWormAI>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            ParticleSystem vanillaEmergeShockwave = worm.transform.Find(EMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaSubmergeShockwave = worm.transform.Find(SUBMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();

            DestroyAudioReflector(modGround);
            worm.groundAudio.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            worm.creatureSFX.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            worm.creatureVoice.mute = false;

            SkinData.PreEmergeParticleAction.RemoveRef(ref worm.emergeFromGroundParticle1, vanillaPreEmergeParticle);
            SkinData.EmergeParticleAction.RemoveRef(ref worm.emergeFromGroundParticle2, vanillaEmergeParticle);
            SkinData.EmergeShockwaveParticleAction.Remove(vanillaEmergeShockwave, replacementShock);
            SkinData.SubmergeParticleAction.RemoveRef(ref worm.hitGroundParticle, vanillaSubmergeParticle);
            SkinData.SubmergeShockwaveParticleAction.Remove(vanillaSubmergeShockwave, replacementSubShock);

            SkinData.PreEmergeParticleMaterialAction.Remove(worm.emergeFromGroundParticle1.GetComponent<ParticleSystemRenderer>(), 0, vanillaPreEmergeParticleMaterial);
            SkinData.EmergeParticleMaterialAction.Remove(worm.emergeFromGroundParticle2.GetComponent<ParticleSystemRenderer>(), 0, vanillaEmergeParticleMaterial);
            SkinData.EmergeShockwaveParticleMaterialAction.Remove(vanillaEmergeShockwave?.GetComponent<ParticleSystemRenderer>(), 0, vanillaShockMaterial);
            SkinData.SubmergeParticleMaterialAction.Remove(worm.hitGroundParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaSubmergeParticleMaterial);
            SkinData.SubmergeShockwaveParticleMaterialAction.Remove(vanillaSubmergeShockwave?.GetComponent<ParticleSystemRenderer>(), 0, vanillaSubShockMaterial);

            SkinData.BodyMeshAction.Remove(new SkinnedMeshRenderer[] { enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>() }, skinnedMeshReplacement);
        }
    }
}