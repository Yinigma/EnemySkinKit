using AntlerShed.EnemySkinKit.AudioReflection;
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
        //source
        //protected AudioClip vanillaThrustFarAudio;
        //source
        //protected AudioClip vanillaThrustCloseAudio;
        //source
        //protected AudioClip vanillaThrustStartAudio;
        //source
        //protected AudioClip vanillaActivateSpotlightAudio;
        //source
        //protected AudioClip vanillaEngineAudio;
        //source
        //protected AudioClip vanillaChargeAudio;

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

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();

        protected AudioReflector modLRAD2;
        protected AudioReflector modEffects;
        protected AudioReflector modCreatureVoice;
        protected AudioReflector modFlyingDistant;
        protected AudioReflector modSpotlightOn;
        protected AudioReflector modExplosion;
        protected AudioReflector modEngineEffects;
        protected AudioReflector modCreatureEffects;
        protected AudioReflector modChargeForward;
        protected AudioReflector modBlowtorch;
        protected AudioReflector modLRAD;
        protected AudioReflector modThrusterClose;
        protected AudioReflector modThrusterStart;

        protected OldBirdSkin SkinData { get; }
        
        public OldBirdSkinner(OldBirdSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            RadMechAI mech = enemy.GetComponent<RadMechAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            vanillaSpotlightMaterial = SkinData.LightMaterialAction.Apply(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshRenderer>(), 0);
            vanillaSearchlightMesh = SkinData.SearchLightMeshAction.Apply(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshFilter>());

            vanillaFlameMaterialLeft = SkinData.ThrusterFireMaterialAction.Apply(mech.transform.Find(THRUSTER_FLAME_LEFT_PATH)?.GetComponent<SpriteRenderer>(), 0);
            vanillaFlameMaterialRight = SkinData.ThrusterFireMaterialAction.Apply(mech.transform.Find(THRUSTER_FLAME_RIGHT_PATH)?.GetComponent<SpriteRenderer>(), 0);

            vanillaSpotlightColor = SkinData.LightColorAction.Apply(mech.transform.Find(SPOT_LIGHT_PATH)?.GetComponent<Light>());
            vanillaPointlightColor = SkinData.LightColorAction.Apply(mech.transform.Find(POINT_LIGHT_PATH)?.GetComponent<Light>());

            vanillaBotSpotlightMaterial = SkinData.SpotlightActiveMaterialAction.ApplyRef(ref mech.spotlightMat);
            vanillaDefaultMaterial = SkinData.DefaultMaterialAction.ApplyRef(ref mech.defaultMat);

            SkinData.ShootGunAudioListAction.ApplyToMap(mech.shootGunSFX, clipMap);
            SkinData.ExplosionAudioListAction.ApplyToMap(mech.largeExplosionSFX, clipMap);

            SkinData.AlarmAudioAction.ApplyToMap(mech.LocalLRADAudio.clip, clipMap);
            SkinData.SpotlightActivateAudioAction.ApplyToMap(mech.spotlightOnAudio.clip, clipMap);

            SkinData.ThrusterFarAudioAction.ApplyToMap(mech.flyingDistantAudio.clip, clipMap);
            SkinData.BlowtorchAudioAction.ApplyToMap(mech.blowtorchAudio.clip, clipMap);
            SkinData.ChargeAudioAction.ApplyToMap(mech.chargeForwardAudio.clip, clipMap);
            SkinData.EngineHumAudioAction.ApplyToMap(mech.engineSFX.clip, clipMap);
            SkinData.WakeAudioAction.ApplyToMap(mech.creatureSFX.clip, clipMap);

            AudioClip[] vanillaBrainwashAudio = new AudioClip[mech.enemyType.audioClips.Length - 4];
            Array.Copy(mech.enemyType.audioClips, 4, vanillaBrainwashAudio, 0, mech.enemyType.audioClips.Length - 4);
            AudioClip[] vanillaStompAudio = new AudioClip[4];
            Array.Copy(mech.enemyType.audioClips, 0, vanillaStompAudio, 0, 4);

            SkinData.BrainwashingAudioListAction.ApplyToMap(vanillaBrainwashAudio, clipMap);
            SkinData.StompAudioListAction.ApplyToMap(vanillaStompAudio, clipMap);

            SkinData.SpotlightDectivateAudioAction.ApplyToMap(mech.spotlightOff, clipMap);
            SkinData.SpotlightFlickerAudioAction.ApplyToMap(mech.spotlightFlicker, clipMap);

            modLRAD = CreateAudioReflector(mech.LocalLRADAudio, clipMap, mech.NetworkObjectId);
            mech.LocalLRADAudio.mute = true;
            modLRAD2 = CreateAudioReflector(mech.LocalLRADAudio2, clipMap, mech.NetworkObjectId); 
            mech.LocalLRADAudio2.mute = true;
            modBlowtorch = CreateAudioReflector(mech.blowtorchAudio, clipMap, mech.NetworkObjectId); 
            mech.blowtorchAudio.mute = true;
            modChargeForward = CreateAudioReflector(mech.chargeForwardAudio, clipMap, mech.NetworkObjectId); 
            mech.chargeForwardAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(mech.creatureSFX, clipMap, mech.NetworkObjectId); 
            mech.creatureSFX.mute = true;
            modEngineEffects = CreateAudioReflector(mech.engineSFX, clipMap, mech.NetworkObjectId); 
            mech.engineSFX.mute = true;
            modExplosion = CreateAudioReflector(mech.explosionAudio, clipMap, mech.NetworkObjectId); 
            mech.explosionAudio.mute = true;
            modSpotlightOn = CreateAudioReflector(mech.spotlightOnAudio, clipMap, mech.NetworkObjectId); 
            mech.spotlightOnAudio.mute = true;
            modFlyingDistant = CreateAudioReflector(mech.flyingDistantAudio, clipMap, mech.NetworkObjectId); 
            mech.flyingDistantAudio.mute = true;
            modCreatureVoice = CreateAudioReflector(mech.creatureVoice, clipMap, mech.NetworkObjectId); 
            mech.creatureVoice.mute = true;

            AudioSource thrusterStartSource = enemy.transform.Find(THRUSTER_BLAST_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterStartSource != null)
            {
                SkinData.ThrusterStartAudioAction.ApplyToMap(thrusterStartSource.clip, clipMap);
                modThrusterStart = CreateAudioReflector(thrusterStartSource, clipMap, mech.NetworkObjectId);
                thrusterStartSource.mute = true;
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                SkinData.ThrusterCloseAudioAction.ApplyToMap(thrusterCloseSource.clip, clipMap);
                modThrusterClose = CreateAudioReflector(thrusterCloseSource, clipMap, mech.NetworkObjectId);
                thrusterCloseSource.mute = true;
            }

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
            DestroyAudioReflector(modLRAD);
            mech.LocalLRADAudio.mute = false;
            DestroyAudioReflector(modLRAD2);
            mech.LocalLRADAudio2.mute = false;
            DestroyAudioReflector(modBlowtorch);
            mech.blowtorchAudio.mute = false;
            DestroyAudioReflector(modChargeForward);
            mech.chargeForwardAudio.mute = false;
            DestroyAudioReflector(modCreatureEffects);
            mech.creatureSFX.mute = false;
            DestroyAudioReflector(modEngineEffects);
            mech.engineSFX.mute = false;
            DestroyAudioReflector(modExplosion);
            mech.explosionAudio.mute = false;
            DestroyAudioReflector(modSpotlightOn);
            mech.spotlightOnAudio.mute = false;
            DestroyAudioReflector(modFlyingDistant);
            mech.flyingDistantAudio.mute = false;
            DestroyAudioReflector(modCreatureVoice);
            mech.creatureVoice.mute = false;

            AudioSource thrusterStartSource = enemy.transform.Find(THRUSTER_BLAST_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterStartSource != null)
            {
                DestroyAudioReflector(modThrusterStart);
                thrusterStartSource.mute = false;
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                DestroyAudioReflector(modThrusterClose);
                thrusterCloseSource.mute = false;
            }

            SkinData.LightMaterialAction.Remove(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshRenderer>(), 0, vanillaSpotlightMaterial);
            SkinData.SearchLightMeshAction.Remove(mech.transform.Find(SEARCH_LIGHT_MESH_PATH)?.GetComponent<MeshFilter>(), vanillaSearchlightMesh);

            SkinData.ThrusterFireMaterialAction.Remove(mech.transform.Find(THRUSTER_FLAME_LEFT_PATH)?.GetComponent<SpriteRenderer>(), 0, vanillaFlameMaterialLeft);
            SkinData.ThrusterFireMaterialAction.Remove(mech.transform.Find(THRUSTER_FLAME_RIGHT_PATH)?.GetComponent<SpriteRenderer>(), 0, vanillaFlameMaterialRight);

            SkinData.LightColorAction.Remove(mech.transform.Find(SPOT_LIGHT_PATH)?.GetComponent<Light>(), vanillaSpotlightColor);
            SkinData.LightColorAction.Remove(mech.transform.Find(POINT_LIGHT_PATH)?.GetComponent<Light>(), vanillaPointlightColor);

            SkinData.SpotlightActiveMaterialAction.RemoveRef(ref mech.spotlightMat, vanillaBotSpotlightMaterial);
            SkinData.DefaultMaterialAction.RemoveRef(ref mech.defaultMat, vanillaDefaultMaterial);

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

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }
    }
}