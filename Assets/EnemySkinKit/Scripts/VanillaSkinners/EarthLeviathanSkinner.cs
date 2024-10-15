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
        protected AudioClip[] vanillaGroundRumbleAudio;
        protected AudioClip[] vanillaAmbientRumbleAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaEmergeAudio;
        protected AudioClip[] vanillaRoarAudio;
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

        public EarthLeviathanSkinner( EarthLeviathanSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            SandWormAI worm = enemy.GetComponent<SandWormAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaAmbientRumbleAudio = SkinData.AmbientRumbleAudioListAction.Apply(ref enemy.GetComponent<SandWormAI>().ambientRumbleSFX);
            vanillaGroundRumbleAudio = SkinData.GroundRumbleAudioListAction.Apply(ref enemy.GetComponent<SandWormAI>().groundRumbleSFX);
            vanillaRoarAudio = SkinData.RoarAudioListAction.Apply(ref enemy.GetComponent<SandWormAI>().roarSFX);
            vanillaEmergeAudio = SkinData.EmergeAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().emergeFromGroundSFX);
            vanillaHitGroundAudio = SkinData.HitGroundAudioAction.Apply(ref enemy.GetComponent<SandWormAI>().hitGroundSFX);

            ParticleSystem vanillaEmergeShockwave = worm.transform.Find(EMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaSubmergeShockwave = worm.transform.Find(SUBMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();

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
            SkinData.AmbientRumbleAudioListAction.Remove(ref enemy.GetComponent<SandWormAI>().ambientRumbleSFX, vanillaAmbientRumbleAudio);
            SkinData.GroundRumbleAudioListAction.Remove(ref enemy.GetComponent<SandWormAI>().groundRumbleSFX, vanillaGroundRumbleAudio);
            SkinData.RoarAudioListAction.Remove(ref enemy.GetComponent<SandWormAI>().roarSFX, vanillaRoarAudio);
            SkinData.EmergeAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().emergeFromGroundSFX, vanillaEmergeAudio);
            SkinData.HitGroundAudioAction.Remove(ref enemy.GetComponent<SandWormAI>().hitGroundSFX, vanillaHitGroundAudio);

            ParticleSystem vanillaEmergeShockwave = worm.transform.Find(EMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaSubmergeShockwave = worm.transform.Find(SUBMERGE_SHOCKWAVE_PATH)?.GetComponent<ParticleSystem>();

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