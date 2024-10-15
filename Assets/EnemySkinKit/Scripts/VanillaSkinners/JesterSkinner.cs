using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class JesterSkinner : BaseSkinner, JesterEventHandler
    {
        protected const string BODY_LOD0_PATH = "MeshContainer/JackInTheBoxBody";
        protected const string BODY_LOD1_PATH = "MeshContainer/JackInTheBoxBodyLowDetail";
        protected const string BODY_LOD2_PATH = "MeshContainer/JackInTheBoxBodyLowDetail2";

        protected const string SKULL_LOD0_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/UpperJaw";
        protected const string SKULL_LOD1_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/UpperJaw/UpperJawLowDetail";
        protected const string SKULL_LOD2_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/UpperJaw/UpperJawLowDetail2";

        protected const string JAW_LOD0_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/LowerJaw";
        protected const string JAW_LOD1_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/LowerJaw/LowerJawLowDetail";

        protected const string CRANK_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/BoxBone/RecordCrank";
        protected const string LID_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer/BoxBone/BoxLid";

        protected const string ANCHOR_PATH = "MeshContainer/AnimContainer/metarig/BoxContainer";

        protected const string ANIM_EVENT_PATH = "MeshContainer/AnimContainer";

        protected VanillaMaterial vanillaBodyMaterial;
        protected VanillaMaterial vanillaSkullMaterial;
        protected VanillaMaterial vanillaJawMaterial;
        protected VanillaMaterial vanillaLidMaterial;

        protected VanillaMaterial vanillaCrankMaterial; //c'mon, dawg

        protected Mesh vanillaSkullLOD0Mesh;
        protected Mesh vanillaSkullLOD1Mesh;
        protected Mesh vanillaSkullLOD2Mesh;

        protected Mesh vanillaJawLOD0Mesh;
        protected Mesh vanillaJawLOD1Mesh;

        protected Mesh vanillaCrankMesh;
        protected Mesh vanillaLidMesh;

        protected AudioClip vanillaMusic;
        protected AudioClip vanillaPopUpAudio;
        protected AudioClip vanillaScreamAudio;
        protected AudioClip vanillaKillPlayerAudio;
        protected AudioClip vanillaFootstepAudio;
        protected AudioClip[] vanillaStompAudio;
        protected AudioClip[] vanillaCrankAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;
        protected bool EffectsSilenced => SkinData.HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

        protected JesterSkin SkinData { get; }

        public JesterSkinner(JesterSkin skinData)
        {
            SkinData = skinData;
        }

        //This enemy has so many goddamn parts
        public override void Apply(GameObject enemy)
        {
            JesterAI jester = enemy.GetComponent<JesterAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            if (EffectsSilenced)
            {
                modCreatureEffects = CreateModdedAudioSource(jester.creatureSFX, "modEffects");
                jester.creatureSFX.mute = true;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = modCreatureEffects;
                }
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(BODY_LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaSkullMaterial = SkinData.SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaJawMaterial = SkinData.JawMaterialAction.Apply(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkinData.JawMaterialAction.Apply(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaCrankMaterial = SkinData.CrankMaterialAction.Apply(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLidMaterial = SkinData.LidMaterialAction.Apply(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaSkullLOD0Mesh = SkinData.SkullLOD0Action.Apply(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSkullLOD1Mesh = SkinData.SkullLOD1Action.Apply(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSkullLOD2Mesh = SkinData.SkullLOD2Action.Apply(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaJawLOD0Mesh = SkinData.JawLOD0Action.Apply(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaJawLOD1Mesh = SkinData.JawLOD1Action.Apply(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaCrankMesh = SkinData.CrankMeshAction.Apply(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaLidMesh = SkinData.LidMeshAction.Apply(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaScreamAudio = SkinData.ScreamingAudioAction.Apply(ref jester.screamingSFX);
            vanillaPopUpAudio = SkinData.PopUpAudioAction.Apply(ref jester.popUpSFX);
            vanillaKillPlayerAudio = SkinData.KillPlayerAudioAction.Apply(ref jester.killPlayerSFX);
            vanillaMusic = SkinData.PopGoesTheWeaselMusicAudioAction.Apply(ref jester.popGoesTheWeaselTheme);

            if(audioAnimEvents != null)
            {
                vanillaCrankAudio = SkinData.CrankAudioListAction.Apply(ref audioAnimEvents.randomClips);
                vanillaStompAudio = SkinData.StompAudioListAction.Apply(ref audioAnimEvents.randomClips2);
                vanillaFootstepAudio = SkinData.FootstepAudioAction.Apply(ref audioAnimEvents.audioClip);
            }

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(jester, this);
        }

        public override void Remove(GameObject enemy)
        {
            JesterAI jester = enemy.GetComponent<JesterAI>();
            PlayAudioAnimationEvent audioAnimEvents = enemy.transform.Find(ANIM_EVENT_PATH)?.gameObject?.GetComponent<PlayAudioAnimationEvent>();
            EnemySkinRegistry.RemoveEnemyEventHandler(jester, this);
            if (EffectsSilenced)
            {
                DestroyModdedAudioSource(modCreatureEffects);
                jester.creatureSFX.mute = false;
                if (audioAnimEvents != null)
                {
                    audioAnimEvents.audioToPlay = jester.creatureSFX;
                }
            }

            ArmatureAttachment.RemoveAttachments(activeAttachments);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            SkinData.BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            SkinData.SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);
            SkinData.SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);
            SkinData.SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);

            SkinData.JawMaterialAction.Remove(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaJawMaterial);
            SkinData.JawMaterialAction.Remove(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaJawMaterial);

            SkinData.CrankMaterialAction.Remove(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaCrankMaterial);
            SkinData.LidMaterialAction.Remove(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLidMaterial);

            SkinData.SkullLOD0Action.Remove(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD0Mesh);
            SkinData.SkullLOD1Action.Remove(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD1Mesh);
            SkinData.SkullLOD2Action.Remove(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD2Mesh);

            SkinData.JawLOD0Action.Remove(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaJawLOD0Mesh);
            SkinData.JawLOD1Action.Remove(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaJawLOD1Mesh);

            SkinData.CrankMeshAction.Remove(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaCrankMesh);
            SkinData.LidMeshAction.Remove(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLidMesh);

            SkinData.ScreamingAudioAction.Remove(ref jester.screamingSFX, vanillaScreamAudio);
            SkinData.PopUpAudioAction.Remove(ref jester.popUpSFX, vanillaPopUpAudio);
            SkinData.KillPlayerAudioAction.Remove(ref jester.killPlayerSFX, vanillaKillPlayerAudio);
            SkinData.PopGoesTheWeaselMusicAudioAction.Remove(ref jester.popGoesTheWeaselTheme, vanillaMusic);

            if (audioAnimEvents != null)
            {
                SkinData.CrankAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaCrankAudio);
                SkinData.FootstepAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaFootstepAudio);
                SkinData.StompAudioListAction.Remove(ref audioAnimEvents.randomClips2, vanillaStompAudio);
            }

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                skinnedMeshReplacement
            );
        }

        public void OnKillPlayer(JesterAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
            }
        }

        public void OnEnterPoppedState(JesterAI instance)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(SkinData.PopUpAudioAction.WorkingClip(vanillaPopUpAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.PopUpAudioAction.WorkingClip(vanillaPopUpAudio));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if(EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, SkinData.HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }
}