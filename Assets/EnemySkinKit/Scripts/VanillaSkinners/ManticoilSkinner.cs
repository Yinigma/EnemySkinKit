using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ManticoilSkinner : BaseSkinner, ManticoilEventHandler
    {
        protected const string LOD0_PATH = "DoublewingModel/BirdLOD0";
        protected const string LOD1_PATH = "DoublewingModel/BirdLOD1";
        protected const string ANCHOR_PATH = "DoublewingModel";


        protected AudioClip[] vanillaScreechAudio;
        protected AudioClip vanillaHitGroundAudio;
        protected AudioClip vanillaFlyAudio;
        protected VanillaMaterial vanillaBodyMaterial;
        protected List<GameObject> activeAttachments;

        protected GameObject skinnedMeshReplacement;

        protected bool VoiceSilenced => SkinData.StunAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => SkinData.FlapAudioListAction.actionType != AudioListActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modCreatureVoice;

        protected ManticoilSkin SkinData { get; }

        public ManticoilSkinner(ManticoilSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            DoublewingAI manticoil = enemy.GetComponent<DoublewingAI>();

            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(manticoil.creatureVoice, "modVoice");
                manticoil.creatureVoice.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(manticoil.creatureSFX, "modEffects");
                manticoil.creatureSFX.mute = true;
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaScreechAudio = SkinData.ScreechAudioListAction.Apply(ref manticoil.birdScreechSFX);
            vanillaHitGroundAudio = SkinData.HitGroundAudioAction.Apply(ref manticoil.birdHitGroundSFX);
            vanillaFlyAudio = SkinData.FlyingAudioAction.ApplyToSource(manticoil.flappingAudio);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(manticoil, this);
        }

        public override void Remove(GameObject enemy)
        {
            DoublewingAI manticoil = enemy.GetComponent<DoublewingAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(manticoil, this);

            if (VoiceSilenced)
            {
                CreateModdedAudioSource(modCreatureVoice);
                manticoil.creatureVoice.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                manticoil.creatureSFX.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            SkinData.ScreechAudioListAction.Remove(ref manticoil.birdScreechSFX, vanillaScreechAudio);
            SkinData.HitGroundAudioAction.Remove(ref manticoil.birdHitGroundSFX, vanillaHitGroundAudio);
            SkinData.FlyingAudioAction.RemoveFromSource(manticoil.flappingAudio, vanillaFlyAudio);

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnStun(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice.PlayOneShot(SkinData.StunAudioAction.WorkingClip(enemy.enemyType.stunSFX));
            }
        }

        public void OnLand(DoublewingAI manticoil)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitGroundAudioAction.WorkingClip(vanillaHitGroundAudio));
            }
        }

        public void OnTakeOff(DoublewingAI manticoil)
        {
            if(EffectsSilenced)
            {
                AudioClip[] flapClips = SkinData.FlapAudioListAction.WorkingClips(manticoil.enemyType.audioClips);
                AudioClip flapClip = flapClips[UnityEngine.Random.Range(0, flapClips.Length)];
                modCreatureEffects.PlayOneShot(flapClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, flapClip, 0.7f);
            }
        }

        public void OnScreech(DoublewingAI manticoil)
        {
            if (VoiceSilenced)
            {
                AudioClip[] screechClips = SkinData.ScreechAudioListAction.WorkingClips(vanillaScreechAudio);
                RoundManager.PlayRandomClip(modCreatureVoice, screechClips);
                WalkieTalkie.TransmitOneShotAudio(modCreatureVoice, screechClips[UnityEngine.Random.Range(0, screechClips.Length)]);
            }
        }
    }
}