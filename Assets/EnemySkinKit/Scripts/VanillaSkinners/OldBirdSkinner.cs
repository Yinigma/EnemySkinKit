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

        protected Material vanillaDefaultMaterial;
        protected Material vanillaSpotlightMaterial;
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

        protected MaterialAction DefaultMaterialAction { get; }
        protected MaterialAction SpotlightActiveMaterialAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        protected AudioListAction BrainwashingAudioListAction { get; }
        protected AudioListAction StompAudioListAction { get; }
        protected AudioListAction ShootGunAudioListAction { get; }
        protected AudioListAction ExplosionAudioListAction { get; }
        protected AudioAction SpotlightActivateAudioAction { get; }
        protected AudioAction SpotlightDectivateAudioAction { get; }
        protected AudioAction SpotlightFlickerAudioAction { get; }
        protected AudioAction BlowtorchAudioAction { get; }
        protected AudioAction AlarmAudioAction { get; }
        protected AudioAction ThrusterFarAudioAction { get; }
        protected AudioAction ThrusterCloseAudioAction { get; }
        protected AudioAction ThrusterStartAudioAction { get; }
        protected AudioAction EngineHumAudioAction { get; }
        protected AudioAction ChargeAudioAction { get; }
        protected AudioAction WakeAudioAction { get; }


        protected bool EffectsSilenced => BrainwashingAudioListAction.actionType != AudioListActionType.RETAIN || StompAudioListAction.actionType != AudioListActionType.RETAIN;

        protected AudioSource modLRAD2;
        protected AudioSource modEffects;

        public OldBirdSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction defaultMaterialAction,
            MaterialAction spotlightActiveMaterialAction,
            SkinnedMeshAction bodyMeshAction,
            AudioListAction brainwashingAudioListAction,
            AudioListAction stompAudioListAction,
            AudioListAction shootGunAudioListAction,
            AudioListAction explosionAudioListAction,
            AudioAction spotlightActivateAudioAction,
            AudioAction spotlightDectivateAudioAction,
            AudioAction spotlightFlickerAudioAction,
            AudioAction blowtorchAudioAction,
            AudioAction alarmAudioAction,
            AudioAction thrusterStartAudioAction,
            AudioAction thrusterCloseAudioAction,
            AudioAction thrusterFarAudioAction,
            AudioAction engineHumAudioAction,
            AudioAction chargeAudioAction,
            AudioAction wakeAudioAction
        )
        {
            DefaultMaterialAction = defaultMaterialAction;
            SpotlightActiveMaterialAction = spotlightActiveMaterialAction;
            BodyMeshAction = bodyMeshAction;
            BrainwashingAudioListAction = brainwashingAudioListAction;
            StompAudioListAction = stompAudioListAction;
            ShootGunAudioListAction = shootGunAudioListAction;
            ExplosionAudioListAction = explosionAudioListAction;
            SpotlightActivateAudioAction = spotlightActivateAudioAction;
            SpotlightDectivateAudioAction = spotlightDectivateAudioAction;
            SpotlightFlickerAudioAction = spotlightFlickerAudioAction;
            BlowtorchAudioAction = blowtorchAudioAction;
            AlarmAudioAction = alarmAudioAction;
            ThrusterStartAudioAction = thrusterStartAudioAction;
            ThrusterCloseAudioAction = thrusterCloseAudioAction;
            ThrusterFarAudioAction = thrusterFarAudioAction;
            EngineHumAudioAction = engineHumAudioAction;
            ChargeAudioAction = chargeAudioAction;
            Attachments = attachments;
            WakeAudioAction = wakeAudioAction;
        }

        public override void Apply(GameObject enemy)
        {
            RadMechAI mech = enemy.GetComponent<RadMechAI>();
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            AudioSource thrusterStartSource = enemy.transform.Find(THRUSTER_BLAST_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterStartSource != null)
            {
                vanillaThrustStartAudio = ThrusterStartAudioAction.ApplyToSource(thrusterStartSource);
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                vanillaThrustCloseAudio = ThrusterCloseAudioAction.ApplyToSource(thrusterCloseSource);
            }
            vanillaSpotlightMaterial = SpotlightActiveMaterialAction.ApplyRef(ref mech.spotlightMat);
            vanillaDefaultMaterial = DefaultMaterialAction.ApplyRef(ref mech.defaultMat);

            vanillaShootGunAudio = ShootGunAudioListAction.Apply(ref mech.shootGunSFX);
            vanillaExplosionAudio = ShootGunAudioListAction.Apply(ref mech.largeExplosionSFX);

            vanillaAlarmAudio = AlarmAudioAction.ApplyToSource(mech.LocalLRADAudio);
            vanillaActivateSpotlightAudio = SpotlightActivateAudioAction.ApplyToSource(mech.spotlightOnAudio);

            vanillaThrustFarAudio = ThrusterStartAudioAction.ApplyToSource(mech.flyingDistantAudio);
            vanillaBlowtorchAudio = BlowtorchAudioAction.ApplyToSource(mech.blowtorchAudio);
            vanillaChargeAudio = ChargeAudioAction.ApplyToSource(mech.chargeForwardAudio);
            vanillaEngineAudio = EngineHumAudioAction.ApplyToSource(mech.engineSFX);
            vanillaWakeAudio = WakeAudioAction.ApplyToSource(mech.creatureSFX);

            vanillaDeactivateSpotlightAudio = SpotlightDectivateAudioAction.Apply(ref mech.spotlightOff);
            vanillaSpotlightFlickerAudio = SpotlightFlickerAudioAction.Apply(ref mech.spotlightFlicker);

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

            BodyMeshAction.Apply
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
                ThrusterStartAudioAction.RemoveFromSource(thrusterStartSource, vanillaThrustStartAudio);
            }
            AudioSource thrusterCloseSource = enemy.transform.Find(THRUSTER_CLOSE_AUDIO_PATH)?.gameObject?.GetComponent<AudioSource>();
            if (thrusterCloseSource != null)
            {
                ThrusterCloseAudioAction.RemoveFromSource(thrusterCloseSource, vanillaThrustCloseAudio);
            }
            SpotlightActiveMaterialAction.RemoveRef(ref mech.spotlightMat, vanillaSpotlightMaterial);
            DefaultMaterialAction.RemoveRef(ref mech.defaultMat, vanillaDefaultMaterial);

            ShootGunAudioListAction.Remove(ref mech.shootGunSFX, vanillaShootGunAudio);
            ShootGunAudioListAction.Remove(ref mech.largeExplosionSFX, vanillaExplosionAudio);

            AlarmAudioAction.RemoveFromSource(mech.LocalLRADAudio, vanillaAlarmAudio);
            SpotlightActivateAudioAction.RemoveFromSource(mech.spotlightOnAudio, vanillaActivateSpotlightAudio);
            ThrusterStartAudioAction.RemoveFromSource(mech.flyingDistantAudio, vanillaThrustFarAudio);
            BlowtorchAudioAction.RemoveFromSource(mech.blowtorchAudio, vanillaBlowtorchAudio);
            ChargeAudioAction.RemoveFromSource(mech.chargeForwardAudio, vanillaChargeAudio);
            EngineHumAudioAction.RemoveFromSource(mech.engineSFX, vanillaEngineAudio);
            WakeAudioAction.RemoveFromSource(mech.creatureSFX, vanillaWakeAudio);

            SpotlightDectivateAudioAction.Remove(ref mech.spotlightOff, vanillaDeactivateSpotlightAudio);
            SpotlightFlickerAudioAction.Remove(ref mech.spotlightFlicker, vanillaSpotlightFlickerAudio);

            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modLRAD2);
                DestroyModdedAudioSource(modEffects);
                mech.creatureSFX.mute = false;
                mech.LocalLRADAudio2.mute = false;
            }

            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(MECH_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnStomp(RadMechAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] stompClips = StompAudioListAction.WorkingClips(vanillaStompAudio);
                AudioClip stompClip = stompClips[UnityEngine.Random.Range(0, stompClips.Length)];
                modEffects.PlayOneShot(stompClip, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modEffects, stompClip, 0.85f);
            }
        }

        public void OnBlastBrainwashing(RadMechAI oldBird, int clipIndex)
        {
            if(EffectsSilenced)
            {
                if (BrainwashingAudioListAction.actionType == AudioListActionType.RETAIN)
                {
                    modLRAD2.clip = oldBird.enemyType.audioClips[clipIndex + 4];
                    modLRAD2.Play();
                }
                else if (BrainwashingAudioListAction.actionType == AudioListActionType.REPLACE)
                {
                    AudioClip[] clips = BrainwashingAudioListAction.WorkingClips(vanillaBrainwashAudio);
                    modLRAD2.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    modLRAD2.Play();
                }
            }
        }

        public void OnShootGun(RadMechAI instance)
        {
            if (EffectsSilenced)
            {
                AudioClip[] shootClips = ShootGunAudioListAction.WorkingClips(vanillaShootGunAudio);
                AudioClip shootClip = shootClips[UnityEngine.Random.Range(0, shootClips.Length)];
                modEffects.PlayOneShot(shootClip, UnityEngine.Random.Range(0.82f, 1f));
                WalkieTalkie.TransmitOneShotAudio(modEffects, shootClip, 0.85f);
            }
        }
    }
}