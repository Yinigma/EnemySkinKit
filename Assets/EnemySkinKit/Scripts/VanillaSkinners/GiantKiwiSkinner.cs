using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class GiantKiwiSkinner : BaseSkinner, GiantKiwiEventHandler
    {
        protected const string LOD0_PATH = "GiantKiwiModel/AnimContainer/bodyMesh";
        protected const string ANCHOR_PATH = "BaboonBirdModel/AnimContainer";

        protected GameObject birdNestMeshReplacement;
        // Egg
        protected GameObject eggBodySkinnedMeshAction;
        protected VanillaMaterial vanillaEggBodyMaterial;
        protected AudioClip vanillaEggScreamAudio;
        protected AudioClip vanillaEggScream3Audio;
        protected AudioClip vanillaEggPeepAudio;
        protected AudioClip vanillaEggBreakAudio;

        // Parent
        protected GameObject skinnedMeshReplacement;
        protected VanillaMaterial vanillaBodyMaterial;
        protected AudioClip[] vanillaFootstepsAudio;
        protected AudioClip[] vanillaFootstepsBassAudio;
        protected AudioClip[] vanillaAttackAudio;
        protected AudioClip[] vanillaScreamAudio;
        protected AudioClip vanillaDeathAudio;
        protected AudioClip vanillaHitBodyAudio;
        protected AudioClip vanillaSquawkAudio;
        protected AudioClip vanillaPeckTreeAudio;
        protected AudioClip vanillaBreakAndEnterShipAudio;
        protected AudioClip vanillaShipAlarmAudio;
        protected AudioClip vanillaBirdNestAudio; // might be an audiosource thing?
        protected ParticleSystem vanillaWoodChip;
        protected ParticleSystem vanillaRocks;
        protected ParticleSystem vanillaRunning;
        protected ParticleSystem vanillaFeathers;
        protected List<GameObject> activeAttachments;

        protected GiantKiwiSkin SkinData { get; }

        public GiantKiwiSkinner(GiantKiwiSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            GiantKiwiAI giantKiwi = enemy.GetComponent<GiantKiwiAI>();

            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.ParentAttachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );

            vanillaFootstepsAudio = SkinData.FootstepsAudioListAction.Apply(ref giantKiwi.footstepSFX);
            vanillaFootstepsBassAudio = SkinData.FootstepsBassAudioListAction.Apply(ref giantKiwi.footstepBassSFX);
            vanillaScreamAudio = SkinData.ScreamAudioListAction.Apply(ref giantKiwi.screamSFX);
            vanillaAttackAudio = SkinData.AttackAudioListAction.Apply(ref giantKiwi.attackSFX);
            vanillaDeathAudio = SkinData.DeathAudioAction.Apply(ref giantKiwi.dieSFX);
            giantKiwi.enemyType.hitBodySFX = SkinData.HitBodyAudioAction.Apply(ref vanillaHitBodyAudio);
            vanillaSquawkAudio = SkinData.SquawkAudioAction.Apply(ref giantKiwi.squawkSFX);
            vanillaPeckTreeAudio = SkinData.PeckTreeAudioAction.Apply(ref giantKiwi.peckTreeSFX);
            vanillaBreakAndEnterShipAudio = SkinData.BreakAndEnterShipAudioAction.Apply(ref giantKiwi.breakAndEnter);
            vanillaShipAlarmAudio = SkinData.ShipAlarmAudioAction.Apply(ref giantKiwi.shipAlarm);
            // vanillaBirdNestAudio = SkinData.BirdNestAudioAction.Apply(ref giantKiwi.nest);

            vanillaWoodChip = SkinData.WoodChipParticleAction.ApplyRef(ref giantKiwi.woodChipParticle);
            vanillaRocks = SkinData.RocksParticleAction.ApplyRef(ref giantKiwi.rocksParticle);
            vanillaRunning = SkinData.RunningParticleAction.ApplyRef(ref giantKiwi.runningParticle);
            // vanillaFeathers = SkinData.FeathersParticleAction.ApplyRef(ref giantKiwi.feathersParticle);
            EnemySkinRegistry.RegisterEnemyEventHandler(giantKiwi, this);
        }

        public override void Remove(GameObject enemy)
        {
            GiantKiwiAI giantKiwi = enemy.GetComponent<GiantKiwiAI>();

            EnemySkinRegistry.RemoveEnemyEventHandler(giantKiwi, this);

            ArmatureAttachment.RemoveAttachments(activeAttachments);

            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );

            SkinData.FootstepsAudioListAction.Remove(ref giantKiwi.footstepSFX, vanillaFootstepsAudio);
            SkinData.FootstepsBassAudioListAction.Remove(ref giantKiwi.footstepBassSFX, vanillaFootstepsBassAudio);
            SkinData.ScreamAudioListAction.Remove(ref giantKiwi.screamSFX, vanillaScreamAudio);
            SkinData.AttackAudioListAction.Remove(ref giantKiwi.attackSFX, vanillaAttackAudio);
            SkinData.DeathAudioAction.Remove(ref giantKiwi.dieSFX, vanillaDeathAudio);
            giantKiwi.enemyType.hitBodySFX = vanillaHitBodyAudio;
            SkinData.SquawkAudioAction.Remove(ref giantKiwi.squawkSFX, vanillaSquawkAudio);
            SkinData.PeckTreeAudioAction.Remove(ref giantKiwi.peckTreeSFX, vanillaPeckTreeAudio);
            SkinData.BreakAndEnterShipAudioAction.Remove(ref giantKiwi.breakAndEnter, vanillaBreakAndEnterShipAudio);
            SkinData.ShipAlarmAudioAction.Remove(ref giantKiwi.shipAlarm, vanillaShipAlarmAudio);
            // SkinData.BirdNestAudioAction.Remove(ref giantKiwi.nest, vanillaBirdNestAudio);

            SkinData.WoodChipParticleAction.RemoveRef(ref giantKiwi.woodChipParticle, vanillaWoodChip);
            SkinData.RocksParticleAction.RemoveRef(ref giantKiwi.rocksParticle, vanillaRocks);
            SkinData.RunningParticleAction.RemoveRef(ref giantKiwi.runningParticle, vanillaRunning);
        }
    }
}