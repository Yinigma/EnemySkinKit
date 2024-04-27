using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class ButlerSkinner : BaseSkinner, ButlerEventHandler
    {
        protected const string LOD0_PATH = "MeshContainer/BodyLOD0";
        protected const string LOD1_PATH = "MeshContainer/BodyLOD1";
        protected const string LOD2_PATH = "MeshContainer/BodyLOD2";
        protected const string ANCHOR_PATH = "MeshContainer";
        protected const string BROOM_PATH = "MeshContainer/metarig/spine/Broom";
        protected const string HAIR_PATH = "MeshContainer/metarig/spine/spine.001/NeckContainer/spine.004/face/Hair";
        protected const string TEETH_PATH = "MeshContainer/metarig/spine/spine.001/NeckContainer/spine.004/face/Teeth";

        protected Material vanillaBodyMaterial;
        protected Material vanillaBroomMaterial;
        protected Material vanillaTeethMaterial;
        protected Material vanillaHairMaterial;
        protected List<GameObject> activeAttachments;
        protected Mesh vanillaBroomMesh;
        protected Mesh vanillaTeethMesh;
        protected Mesh vanillaHairMesh;

        protected AudioClip vanillaAmbience;
        protected AudioClip vanillaBuzzingAmbience;
        //protected AudioSource sweepingAudio;
        protected AudioClip[] vanillaFootsteps;
        protected AudioClip[] vanillaBroomSweeps;
        protected AudioClip vanillaPopReverb;
        protected AudioClip vanillaMurderMusic;
        protected AudioClip vanillaPopAudio;

        protected MaterialAction BodyMaterialAction { get; }
        
        protected SkinnedMeshAction BodyMeshAction { get; }
        protected ArmatureAttachment[] Attachments { get; }
        protected MaterialAction BroomMaterialAction { get; }
        protected MaterialAction HairMaterialAction { get; }
        protected MaterialAction TeethMaterialAction { get; }
        protected StaticMeshAction BroomMeshAction { get; }
        protected StaticMeshAction HairMeshAction { get; }
        protected StaticMeshAction TeethMeshAction { get; }
        protected AudioListAction FootstepsAudioAction { get; }
        protected AudioListAction SweepsAudioAction { get; }
        protected AudioAction PopReverbAudioAction { get; }
        protected AudioAction MurderMusicAudioAction { get; }
        protected AudioAction DefaultAmbienceAudioAction { get; }
        protected AudioAction BuzzingAmbienceAudioAction { get; }
        protected AudioAction StabPlayerAudioAction { get; }
        protected AudioAction CoatRustleAudioAction { get; }
        protected AudioAction BrandishKnifeAudioAction { get; }
        protected AudioAction PopAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioAction InflateAudioAction { get; }

        protected bool PopSilenced => InflateAudioAction.actionType != AudioActionType.RETAIN;
        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN ||
            BrandishKnifeAudioAction.actionType != AudioActionType.RETAIN ||
            CoatRustleAudioAction.actionType != AudioActionType.RETAIN ||
            StabPlayerAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;
        protected AudioSource modPopNear;

        public ButlerSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction bodyMaterialAction,
            MaterialAction broomMaterialAction,
            MaterialAction teethMaterialAction,
            MaterialAction hairMaterialAction,
            SkinnedMeshAction bodyMeshAction,
            StaticMeshAction broomMeshAction,
            StaticMeshAction teethMeshAction,
            StaticMeshAction hairMeshAction,
            AudioListAction footstepsAudioAction,
            AudioListAction sweepsAudioAction,
            AudioAction popReverbAudioAction,
            AudioAction murderMusicAudioAction,
            AudioAction defaultAmbienceAudioAction,
            AudioAction buzzingAmbienceAudioAction,
            AudioAction stabPlayerAudioAction,
            AudioAction coatRustleAudioAction,
            AudioAction brandishKnifeAudioAction,
            AudioAction popAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction inflateAudioAction
        )
        {
            BodyMaterialAction = bodyMaterialAction;
            BodyMeshAction = bodyMeshAction;
            Attachments = attachments;
            BroomMeshAction = broomMeshAction;
            TeethMeshAction = teethMeshAction;
            HairMeshAction = hairMeshAction;
            BroomMaterialAction = broomMaterialAction;
            TeethMaterialAction = teethMaterialAction;
            HairMaterialAction = hairMaterialAction;
            FootstepsAudioAction = footstepsAudioAction;
            SweepsAudioAction = sweepsAudioAction;
            PopReverbAudioAction = popReverbAudioAction;
            MurderMusicAudioAction = murderMusicAudioAction;
            DefaultAmbienceAudioAction = defaultAmbienceAudioAction;
            BuzzingAmbienceAudioAction = buzzingAmbienceAudioAction;
            StabPlayerAudioAction = stabPlayerAudioAction;
            CoatRustleAudioAction = coatRustleAudioAction;
            BrandishKnifeAudioAction = brandishKnifeAudioAction;
            PopAudioAction = popAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            InflateAudioAction = inflateAudioAction;
        }

        public override void Apply(GameObject enemy)
        {
            ButlerEnemyAI butler = enemy.GetComponent<ButlerEnemyAI>();

            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBroomMaterial = BroomMaterialAction.Apply(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaTeethMaterial = TeethMaterialAction.Apply(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaHairMaterial = HairMaterialAction.Apply(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<Renderer>(), 0);
            vanillaBroomMesh = BroomMeshAction.Apply(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaTeethMesh = TeethMeshAction.Apply(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<MeshFilter>());
            vanillaHairMesh = HairMeshAction.Apply(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<MeshFilter>());

            vanillaAmbience = DefaultAmbienceAudioAction.ApplyToSource(butler.ambience1);
            vanillaBuzzingAmbience = BuzzingAmbienceAudioAction.ApplyToSource(butler.buzzingAmbience);
            vanillaFootsteps = FootstepsAudioAction.Apply(ref butler.footsteps);
            vanillaBroomSweeps = SweepsAudioAction.Apply(ref butler.broomSweepSFX);
            vanillaPopReverb = PopReverbAudioAction.ApplyToSource(butler.popAudioFar);
            vanillaMurderMusic = MurderMusicAudioAction.ApplyToSource(butler.ambience2);
            vanillaPopAudio = PopAudioAction.ApplyToSource(butler.popAudio);

            if (PopSilenced)
            {
                modPopNear = CreateModdedAudioSource(butler.popAudio, "modPopAudio");
                butler.popAudio.mute = true;
            }
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(butler.creatureSFX, "modEffects");
                butler.creatureSFX.mute = true;
            }

            BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH),
                new Dictionary<string, Transform>()
                {
                    { "metarig", enemy.transform.Find($"{ANCHOR_PATH}/metarig") },
                    { "NeckContainer", enemy.transform.Find($"{ANCHOR_PATH}/metarig/spine/spine.001/NeckContainer") } 
                }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(butler, this);
        }

        public override void Remove(GameObject enemy)
        {
            ButlerEnemyAI butler = enemy.GetComponent<ButlerEnemyAI>();
            EnemySkinRegistry.RemoveEnemyEventHandler(butler, this);
            if (PopSilenced)
            {
                DestroyModdedAudioSource(butler.popAudio);
                butler.popAudio.mute = false;
            }
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(butler.creatureSFX);
                butler.creatureSFX.mute = false;
            }

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BroomMaterialAction.Remove(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaBroomMaterial);
            TeethMaterialAction.Remove(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaTeethMaterial);
            HairMaterialAction.Remove(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<Renderer>(), 0, vanillaHairMaterial);
            BroomMeshAction.Remove(enemy.transform.Find(BROOM_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaBroomMesh);
            TeethMeshAction.Remove(enemy.transform.Find(TEETH_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaTeethMesh);
            HairMeshAction.Remove(enemy.transform.Find(HAIR_PATH)?.gameObject?.GetComponent<MeshFilter>(), vanillaHairMesh);

            DefaultAmbienceAudioAction.RemoveFromSource(butler.ambience1, vanillaAmbience);
            BuzzingAmbienceAudioAction.RemoveFromSource(butler.buzzingAmbience, vanillaBuzzingAmbience);
            FootstepsAudioAction.Remove(ref butler.footsteps, vanillaFootsteps);
            SweepsAudioAction.Remove(ref butler.broomSweepSFX, vanillaBroomSweeps);
            PopAudioAction.RemoveFromSource(butler.popAudioFar, vanillaPopReverb);
            MurderMusicAudioAction.RemoveFromSource(butler.ambience2, vanillaMurderMusic);

            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnStep(ButlerEnemyAI instance)
        {
            if(EffectsSilenced)
            {
                AudioClip[] stepClips = FootstepsAudioAction.WorkingClips(vanillaFootsteps);
                AudioClip stepClip = stepClips[UnityEngine.Random.Range(0, stepClips.Length)];
                modCreatureEffects.PlayOneShot(stepClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, stepClip);
            }
            
        }

        public void OnSweep(ButlerEnemyAI instance)
        {
            if(EffectsSilenced)
            {
                AudioClip[] sweepClips = SweepsAudioAction.WorkingClips(vanillaBroomSweeps);
                AudioClip sweepClip = sweepClips[UnityEngine.Random.Range(0, sweepClips.Length)];
                modCreatureEffects.PlayOneShot(sweepClip);
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, sweepClip);
            }
            
        }

        public void OnEnterSweepeingState(ButlerEnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
            }
        }

        public void OnEnterPremeditatingState(EnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, CoatRustleAudioAction.WorkingClip(instance.enemyType.audioClips[1]));
            }
        }

        public void OnEnterMurderingState(ButlerEnemyAI instance)
        {
            if (EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(BrandishKnifeAudioAction.WorkingClip(instance.enemyType.audioClips[2]));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, BrandishKnifeAudioAction.WorkingClip(instance.enemyType.audioClips[2]));
            }
        }

        public void OnPop(ButlerEnemyAI instance)
        {
            if(PopSilenced)
            {
                modPopNear.Play();
            }
        }

        public void OnInflate(ButlerEnemyAI instance)
        {
            if(PopSilenced)
            {
                modPopNear.PlayOneShot(InflateAudioAction.WorkingClip(instance.enemyType.audioClips[3]));
            }
        }

        public void OnStabPlayer(ButlerEnemyAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(StabPlayerAudioAction.WorkingClip(instance.enemyType.audioClips[0]));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if (EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }
}