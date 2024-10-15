using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class OldBirdSkinner : BaseSkinner, OldBirdEventHandler
    {
        protected const string MECH_PATH = "MeshContainer/Body";
        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer";
        protected const string THRUSTER_CLOSE_AUDIO_PATH = "MeshContainer/AnimContainer/metarig/LegLeft/LegLeft2/LeftLegFireContainer/ThrusterCloseAudio (2)";
        protected const string THRUSTER_BLAST_AUDIO_PATH = "ChargeForwardEffect/LargeExplosionEffect/InitialBlast";
        protected const string SEARCH_LIGHT_MESH_PATH = "MeshContainer/AnimContainer/metarig/TorsoContainer/Torso/LightContainer/Sphere";
        protected const string SPOT_LIGHT_PATH = "MeshContainer/AnimContainer/metarig/TorsoContainer/Torso/LightContainer/Spotlight";
        protected const string POINT_LIGHT_PATH = "MeshContainer/AnimContainer/metarig/TorsoContainer/Torso/LightContainer/Pointlight";
        protected const string THRUSTER_FLAME_LEFT_PATH = "MeshContainer/AnimContainer/metarig/LegLeft/LegLeft2/LeftLegFireContainer/ThrusterFlame";
        protected const string THRUSTER_FLAME_RIGHT_PATH = "MeshContainer/AnimContainer/metarig/LegRight/LegRight2/RightLegFireContainer/ThrusterFlame";

        //particles
        protected const string BLOWTORCH_RED_FLAMES_PATH = "MeshContainer/AnimContainer/metarig/TorsoContainer/Torso/LeftUpperArm/GunArmContainer/LeftLowerArm/FlamethrowerParticle/MainBlast (1)";
        protected const string CHARGE_FLASH_PATH = "ChargeForwardParticle/BrightFlash";
        protected const string CHARGE_SMOKE_PATH = "ChargeForwardParticle/Smoke (1)";
        protected const string CHARGE_FLAMES_PATH = "ChargeForwardParticle/LingeringFire";
        protected const string CHARGE_BLAST_PATH = "ChargeForwardParticle/MainBlast (1)";

        protected VanillaMaterial vanillaDefaultMaterial;
        protected VanillaMaterial vanillaBotSpotlightMaterial;
        protected List<GameObject> activeAttachments;
        protected AudioClip[] vanillaShootGunAudio;
        protected AudioClip[] vanillaExplosionAudio;
        protected AudioClip vanillaBlowtorchAudio;
        protected AudioClip vanillaAlarmAudio;
        //source
        protected AudioClip vanillaThrustFarAudio;
        //source
        protected AudioClip vanillaThrustCloseAudio;
        //source
        protected AudioClip vanillaThrustStartAudio;
        //source
        protected AudioClip vanillaActivateSpotlightAudio;
        protected AudioClip vanillaDeactivateSpotlightAudio;
        protected AudioClip vanillaSpotlightFlickerAudio;
        protected AudioClip vanillaWakeAudio;
        //source
        protected AudioClip vanillaEngineAudio;
        //source
        protected AudioClip vanillaChargeAudio;
        protected AudioClip[] vanillaStompAudio;
        protected AudioClip[] vanillaBrainwashAudio;
        protected GameObject skinnedMeshReplacement;

        private VanillaMaterial vanillaSpotlightMaterial;
        private Mesh vanillaSearchlightMesh;
        private VanillaMaterial vanillaFlameMaterialLeft;
        private VanillaMaterial vanillaFlameMaterialRight;
        private Color vanillaSpotlightColor;
        private Color vanillaPointlightColor;
        private VanillaMaterial vanillaChargeFlashMaterial;
        private VanillaMaterial vanillaChargeBlastMaterial;
        private VanillaMaterial vanillaChargeSmokeMaterial;
        private VanillaMaterial vanillaChargeFlameMaterial;
        private VanillaMaterial vanillaTorchFlamesMaterial;
        private VanillaMaterial vanillaBlueTorchFlamesMaterial;
        private VanillaMaterial vanillaRightStompMaterial;
        private VanillaMaterial vanillaLeftStompMaterial;
        private VanillaMaterial vanillaRightSmokeMaterial;
        private VanillaMaterial vanillaLeftSmokeMaterial;
        private VanillaMaterial vanillaChargeMaterial;
        private VanillaMaterial vanillaLandShockwaveMaterial;
        private VanillaMaterial vanillaMuzzleMaterial;
        private ParticleSystem replacementChargeFlashParticle;
        private ParticleSystem replacementChargeBlastParticle;
        private ParticleSystem replacementChargeSmokeParticle;
        private ParticleSystem replacementChargeFlameParticle;
        private ParticleSystem replacementTorchFlamesParticle;
        private ParticleSystem vanillaBlueTorchFlamesParticle;
        private ParticleSystem vanillaRightStompParticle;
        private ParticleSystem vanillaLeftStompParticle;
        private ParticleSystem vanillaRightSmokeParticle;
        private ParticleSystem vanillaLeftSmokeParticle;
        private ParticleSystem vanillaChargeParticle;
        private ParticleSystem vanillaLandShockwaveParticle;
        private ParticleSystem vanillaMuzzleParticle;

        protected bool EffectsSilenced => SkinData.BrainwashingAudioListAction.actionType != AudioListActionType.RETAIN || SkinData.StompAudioListAction.actionType != AudioListActionType.RETAIN;

        protected AudioSource modLRAD2;
        protected AudioSource modEffects;

        protected OldBirdSkin SkinData { get; }
        
        public OldBirdSkinner(OldBirdSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            RadMechAI mech = enemy.GetComponent<RadMechAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            AudioSource thrusterStartSource = enemy.transform.Find(THRUSTER_BLAST_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterStartSource != null)
            {
                vanillaThrustStartAudio = SkinData.ThrusterStartAudioAction.ApplyToSource(thrusterStartSource);
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                vanillaThrustCloseAudio = SkinData.ThrusterCloseAudioAction.ApplyToSource(thrusterCloseSource);
            }

            vanillaSpotlightMaterial = SkinData.LightMaterialAction.Apply(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshRenderer>(), 0);
            vanillaSearchlightMesh = SkinData.SearchLightMeshAction.Apply(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshFilter>());

            vanillaFlameMaterialLeft = SkinData.ThrusterFireMaterialAction.Apply(mech.transform.Find(THRUSTER_FLAME_LEFT_PATH)?.GetComponent<SpriteRenderer>(), 0);
            vanillaFlameMaterialRight = SkinData.ThrusterFireMaterialAction.Apply(mech.transform.Find(THRUSTER_FLAME_RIGHT_PATH)?.GetComponent<SpriteRenderer>(), 0);

            vanillaSpotlightColor = SkinData.LightColorAction.Apply(mech.transform.Find(SPOT_LIGHT_PATH)?.GetComponent<Light>());
            vanillaPointlightColor = SkinData.LightColorAction.Apply(mech.transform.Find(POINT_LIGHT_PATH)?.GetComponent<Light>());

            vanillaBotSpotlightMaterial = SkinData.SpotlightActiveMaterialAction.ApplyRef(ref mech.spotlightMat);
            vanillaDefaultMaterial = SkinData.DefaultMaterialAction.ApplyRef(ref mech.defaultMat);

            vanillaShootGunAudio = SkinData.ShootGunAudioListAction.Apply(ref mech.shootGunSFX);
            vanillaExplosionAudio = SkinData.ExplosionAudioListAction.Apply(ref mech.largeExplosionSFX);

            vanillaAlarmAudio = SkinData.AlarmAudioAction.ApplyToSource(mech.LocalLRADAudio);
            vanillaActivateSpotlightAudio = SkinData.SpotlightActivateAudioAction.ApplyToSource(mech.spotlightOnAudio);

            vanillaThrustFarAudio = SkinData.ThrusterFarAudioAction.ApplyToSource(mech.flyingDistantAudio);
            vanillaBlowtorchAudio = SkinData.BlowtorchAudioAction.ApplyToSource(mech.blowtorchAudio);
            vanillaChargeAudio = SkinData.ChargeAudioAction.ApplyToSource(mech.chargeForwardAudio);
            vanillaEngineAudio = SkinData.EngineHumAudioAction.ApplyToSource(mech.engineSFX);
            vanillaWakeAudio = SkinData.WakeAudioAction.ApplyToSource(mech.creatureSFX);

            vanillaDeactivateSpotlightAudio = SkinData.SpotlightDectivateAudioAction.Apply(ref mech.spotlightOff);
            vanillaSpotlightFlickerAudio = SkinData.SpotlightFlickerAudioAction.Apply(ref mech.spotlightFlicker);

            ParticleSystem vanillaChargeFlash = mech.transform.Find(CHARGE_FLASH_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeBlast = mech.transform.Find(CHARGE_BLAST_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeSmoke = mech.transform.Find(CHARGE_SMOKE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeFlames = mech.transform.Find(CHARGE_FLAMES_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaTorchRedFlames = mech.transform.Find(BLOWTORCH_RED_FLAMES_PATH)?.GetComponent<ParticleSystem>();

            vanillaChargeFlashMaterial = SkinData.ChargeFlashMaterialAction.Apply(vanillaChargeFlash?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaChargeBlastMaterial = SkinData.ChargeBlastMaterialAction.Apply(vanillaChargeBlast?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaChargeSmokeMaterial = SkinData.ChargeSmokeMaterialAction.Apply(vanillaChargeSmoke?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaChargeFlameMaterial = SkinData.ChargeFireMaterialAction.Apply(vanillaChargeFlames?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaTorchFlamesMaterial = SkinData.RedFlameMaterialAction.Apply(vanillaTorchRedFlames?.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaBlueTorchFlamesMaterial = SkinData.BlueFlameMaterialAction.Apply(mech.blowtorchParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaRightStompMaterial = SkinData.StompShockwaveMaterialAction.Apply(mech.rightFootParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaLeftStompMaterial = SkinData.StompShockwaveMaterialAction.Apply(mech.leftFootParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaRightSmokeMaterial = SkinData.SmokeTrailMaterialAction.Apply(mech.smokeRightLeg.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaLeftSmokeMaterial = SkinData.SmokeTrailMaterialAction.Apply(mech.smokeLeftLeg.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaChargeMaterial = SkinData.ChargeMaterialAction.Apply(mech.chargeParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaLandShockwaveMaterial = SkinData.LandShockwaveMaterialAction.Apply(mech.bothFeetParticle.GetComponent<ParticleSystemRenderer>(), 0);
            vanillaMuzzleMaterial = SkinData.MuzzleFlashMaterialAction.Apply(mech.gunArmParticle.GetComponent<ParticleSystemRenderer>(), 0);

            replacementChargeFlashParticle = SkinData.ChargeFlashParticleAction.Apply(vanillaChargeFlash);
            replacementChargeBlastParticle = SkinData.ChargeBlastParticleAction.Apply(vanillaChargeBlast);
            replacementChargeSmokeParticle = SkinData.ChargeSmokeParticleAction.Apply(vanillaChargeSmoke);
            replacementChargeFlameParticle = SkinData.ChargeFireParticleAction.Apply(vanillaChargeFlames);
            replacementTorchFlamesParticle = SkinData.RedFlameParticleAction.Apply(vanillaTorchRedFlames);
            vanillaBlueTorchFlamesParticle = SkinData.BlueFlameParticleAction.ApplyRef(ref mech.blowtorchParticle);
            vanillaRightStompParticle = SkinData.StompShockwaveParticleAction.ApplyRef(ref mech.rightFootParticle);
            vanillaLeftStompParticle = SkinData.StompShockwaveParticleAction.ApplyRef(ref mech.leftFootParticle);
            vanillaRightSmokeParticle = SkinData.SmokeTrailParticleAction.ApplyRef(ref mech.smokeRightLeg);
            vanillaLeftSmokeParticle = SkinData.SmokeTrailParticleAction.ApplyRef(ref mech.smokeLeftLeg);
            vanillaChargeParticle = SkinData.ChargeParticleAction.ApplyRef(ref mech.chargeParticle);
            vanillaLandShockwaveParticle = SkinData.LandShockwaveParticleAction.ApplyRef(ref mech.bothFeetParticle);
            vanillaMuzzleParticle = SkinData.MuzzleFlashParticleAction.ApplyRef(ref mech.gunArmParticle);

            vanillaBrainwashAudio = new AudioClip[mech.enemyType.audioClips.Length-4];
            Array.Copy(mech.enemyType.audioClips, 4, vanillaBrainwashAudio, 0, mech.enemyType.audioClips.Length - 4);
            vanillaStompAudio = new AudioClip[4];
            Array.Copy(mech.enemyType.audioClips, 0, vanillaStompAudio, 0, 4);

            if (EffectsSilenced)
            {
                modLRAD2 = CreateModdedAudioSource(mech.LocalLRADAudio2, "moddedLRAD");
                mech.LocalLRADAudio2.mute = true;
                modEffects = CreateModdedAudioSource(mech.creatureSFX, "moddedEffects");
                mech.creatureSFX.mute = true;
            }

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH),
                //Vile rigs. Vile, I say.
                new Dictionary<string, Transform>
                {
                    { "TorsoContainer", enemy.transform.Find($"{ANCHOR_PATH}/metarig/TorsoContainer") },
                    { "GunArmContainer", enemy.transform.Find($"{ANCHOR_PATH}/metarig/TorsoContainer/Torso/LeftUpperArm/GunArmContainer") },
                }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(mech, this);
        }

        public override void Remove(GameObject enemy)
        {
            RadMechAI mech = enemy.GetComponent<RadMechAI>();
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            EnemySkinRegistry.RemoveEnemyEventHandler(mech, this);
            AudioSource thrusterStartSource = enemy.transform.Find(THRUSTER_BLAST_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterStartSource != null)
            {
                SkinData.ThrusterStartAudioAction.RemoveFromSource(thrusterStartSource, vanillaThrustStartAudio);
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                SkinData.ThrusterCloseAudioAction.RemoveFromSource(thrusterCloseSource, vanillaThrustCloseAudio);
            }

            SkinData.LightMaterialAction.Remove(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshRenderer>(), 0, vanillaSpotlightMaterial);
            SkinData.SearchLightMeshAction.Remove(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshFilter>(), vanillaSearchlightMesh);

            SkinData.ThrusterFireMaterialAction.Remove(mech.transform.Find(THRUSTER_FLAME_LEFT_PATH)?.GetComponent<SpriteRenderer>(), 0, vanillaFlameMaterialLeft);
            SkinData.ThrusterFireMaterialAction.Remove(mech.transform.Find(THRUSTER_FLAME_RIGHT_PATH)?.GetComponent<SpriteRenderer>(), 0, vanillaFlameMaterialRight);

            SkinData.LightColorAction.Remove(mech.transform.Find(SPOT_LIGHT_PATH)?.GetComponent<Light>(), vanillaSpotlightColor);
            SkinData.LightColorAction.Remove(mech.transform.Find(POINT_LIGHT_PATH)?.GetComponent<Light>(), vanillaPointlightColor);

            SkinData.SpotlightActiveMaterialAction.RemoveRef(ref mech.spotlightMat, vanillaBotSpotlightMaterial);
            SkinData.DefaultMaterialAction.RemoveRef(ref mech.defaultMat, vanillaDefaultMaterial);

            SkinData.ShootGunAudioListAction.Remove(ref mech.shootGunSFX, vanillaShootGunAudio);
            SkinData.ExplosionAudioListAction.Remove(ref mech.largeExplosionSFX, vanillaExplosionAudio);

            SkinData.AlarmAudioAction.RemoveFromSource(mech.LocalLRADAudio, vanillaAlarmAudio);
            SkinData.SpotlightActivateAudioAction.RemoveFromSource(mech.spotlightOnAudio, vanillaActivateSpotlightAudio);
            SkinData.ThrusterFarAudioAction.RemoveFromSource(mech.flyingDistantAudio, vanillaThrustFarAudio);
            SkinData.BlowtorchAudioAction.RemoveFromSource(mech.blowtorchAudio, vanillaBlowtorchAudio);
            SkinData.ChargeAudioAction.RemoveFromSource(mech.chargeForwardAudio, vanillaChargeAudio);
            SkinData.EngineHumAudioAction.RemoveFromSource(mech.engineSFX, vanillaEngineAudio);
            SkinData.WakeAudioAction.RemoveFromSource(mech.creatureSFX, vanillaWakeAudio);

            SkinData.SpotlightDectivateAudioAction.Remove(ref mech.spotlightOff, vanillaDeactivateSpotlightAudio);
            SkinData.SpotlightFlickerAudioAction.Remove(ref mech.spotlightFlicker, vanillaSpotlightFlickerAudio);

            ParticleSystem vanillaChargeFlash = mech.transform.Find(CHARGE_FLASH_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeBlast = mech.transform.Find(CHARGE_BLAST_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeSmoke = mech.transform.Find(CHARGE_SMOKE_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaChargeFlames = mech.transform.Find(CHARGE_FLAMES_PATH)?.GetComponent<ParticleSystem>();
            ParticleSystem vanillaTorchRedFlames = mech.transform.Find(BLOWTORCH_RED_FLAMES_PATH)?.GetComponent<ParticleSystem>();

            SkinData.ChargeFlashParticleAction.Remove(vanillaChargeFlash, replacementChargeFlashParticle);
            SkinData.ChargeBlastParticleAction.Remove(vanillaChargeBlast, replacementChargeBlastParticle);
            SkinData.ChargeSmokeParticleAction.Remove(vanillaChargeSmoke, replacementChargeSmokeParticle);
            SkinData.ChargeFireParticleAction.Remove(vanillaChargeFlames, replacementChargeFlameParticle);
            SkinData.RedFlameParticleAction.Remove(vanillaTorchRedFlames, replacementTorchFlamesParticle);
            SkinData.BlueFlameParticleAction.RemoveRef(ref mech.blowtorchParticle, vanillaBlueTorchFlamesParticle);
            SkinData.StompShockwaveParticleAction.RemoveRef(ref mech.rightFootParticle, vanillaRightStompParticle);
            SkinData.StompShockwaveParticleAction.RemoveRef(ref mech.leftFootParticle, vanillaLeftStompParticle);
            SkinData.SmokeTrailParticleAction.RemoveRef(ref mech.smokeRightLeg, vanillaRightSmokeParticle);
            SkinData.SmokeTrailParticleAction.RemoveRef(ref mech.smokeLeftLeg, vanillaLeftSmokeParticle);
            SkinData.ChargeParticleAction.RemoveRef(ref mech.chargeParticle, vanillaChargeParticle);
            SkinData.LandShockwaveParticleAction.RemoveRef(ref mech.bothFeetParticle, vanillaLandShockwaveParticle);
            SkinData.MuzzleFlashParticleAction.RemoveRef(ref mech.gunArmParticle, vanillaMuzzleParticle);

            SkinData.ChargeFlashMaterialAction.Remove(vanillaChargeFlash.GetComponent<ParticleSystemRenderer>(), 0, vanillaChargeFlashMaterial);
            SkinData.ChargeBlastMaterialAction.Remove(vanillaChargeBlast.GetComponent<ParticleSystemRenderer>(), 0, vanillaChargeBlastMaterial);
            SkinData.ChargeSmokeMaterialAction.Remove(vanillaChargeSmoke.GetComponent<ParticleSystemRenderer>(), 0, vanillaChargeSmokeMaterial);
            SkinData.ChargeFireMaterialAction.Remove(vanillaChargeFlames.GetComponent<ParticleSystemRenderer>(), 0, vanillaChargeFlameMaterial);
            SkinData.RedFlameMaterialAction.Remove(vanillaTorchRedFlames.GetComponent<ParticleSystemRenderer>(), 0, vanillaTorchFlamesMaterial);
            SkinData.BlueFlameMaterialAction.Remove(mech.blowtorchParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaBlueTorchFlamesMaterial);
            SkinData.StompShockwaveMaterialAction.Remove(mech.rightFootParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaRightStompMaterial);
            SkinData.StompShockwaveMaterialAction.Remove(mech.leftFootParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaLeftStompMaterial);
            SkinData.SmokeTrailMaterialAction.Remove(mech.smokeRightLeg.GetComponent<ParticleSystemRenderer>(), 0, vanillaRightSmokeMaterial);
            SkinData.SmokeTrailMaterialAction.Remove(mech.smokeLeftLeg.GetComponent<ParticleSystemRenderer>(), 0, vanillaLeftSmokeMaterial);
            SkinData.ChargeMaterialAction.Remove(mech.chargeParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaChargeMaterial);
            SkinData.LandShockwaveMaterialAction.Remove(mech.bothFeetParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaLandShockwaveMaterial);
            SkinData.MuzzleFlashMaterialAction.Remove(mech.gunArmParticle.GetComponent<ParticleSystemRenderer>(), 0, vanillaMuzzleMaterial);

            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modLRAD2);
                DestroyModdedAudioSource(modEffects);
                mech.creatureSFX.mute = false;
                mech.LocalLRADAudio2.mute = false;
            }

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }

        public void OnStomp(RadMechAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] stompClips = SkinData.StompAudioListAction.WorkingClips(vanillaStompAudio);
                AudioClip stompClip = stompClips[UnityEngine.Random.Range(0, stompClips.Length)];
                modEffects.PlayOneShot(stompClip, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modEffects, stompClip, 0.85f);
            }
        }

        public void OnBlastBrainwashing(RadMechAI oldBird, int clipIndex)
        {
            if(EffectsSilenced)
            {
                if (SkinData.BrainwashingAudioListAction.actionType == AudioListActionType.RETAIN)
                {
                    modLRAD2.clip = oldBird.enemyType.audioClips[clipIndex + 4];
                    modLRAD2.Play();
                }
                else if (SkinData.BrainwashingAudioListAction.actionType == AudioListActionType.REPLACE)
                {
                    AudioClip[] clips = SkinData.BrainwashingAudioListAction.WorkingClips(vanillaBrainwashAudio);
                    modLRAD2.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    modLRAD2.Play();
                }
            }
        }

        public void OnShootGun(RadMechAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] shootClips = SkinData.ShootGunAudioListAction.WorkingClips(vanillaShootGunAudio);
                AudioClip shootClip = shootClips[UnityEngine.Random.Range(0, shootClips.Length)];
                modEffects.PlayOneShot(shootClip, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modEffects, shootClip, 0.85f);
            }
        }
    }
}