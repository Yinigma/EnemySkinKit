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

        protected Material vanillaBodyMaterial;
        protected Material vanillaSkullMaterial;
        protected Material vanillaJawMaterial;
        protected Material vanillaLidMaterial;

        protected Material vanillaCrankMaterial; //c'mon, dawg

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

        protected MaterialAction SkullMaterialAction { get; }
        protected MaterialAction JawMaterialAction { get; }
        protected MaterialAction LidMaterialAction { get; }
        protected MaterialAction CrankMaterialAction { get; }
        protected MaterialAction BodyMaterialAction { get; }
        protected StaticMeshAction SkullLOD0Action { get; }
        protected StaticMeshAction SkullLOD1Action { get; }
        protected StaticMeshAction SkullLOD2Action { get; }
        protected StaticMeshAction JawLOD0Action { get; }
        protected StaticMeshAction JawLOD1Action { get; }
        protected StaticMeshAction LidMeshAction { get; }
        protected StaticMeshAction CrankMeshAction { get; }
        protected SkinnedMeshAction BodyMeshAction { get; }

        protected AudioAction MusicAudioAction { get; }
        protected AudioAction PopUpAudioAction { get; }
        protected AudioAction ScreamingAudioAction { get; }
        protected AudioAction KillPlayerAudioAction { get; }
        protected AudioAction HitBodyAudioAction { get; }
        protected AudioAction FootstepAudioAction { get; }
        protected AudioListAction ChaseStompAudioListAction { get; }
        protected AudioListAction CrankAudioListAction { get; }
        protected ArmatureAttachment[] Attachments { get; }

        protected bool EffectsSilenced => HitBodyAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioSource modCreatureEffects;

        public JesterSkinner
        (
            ArmatureAttachment[] attachments,
            MaterialAction skullMaterialAction, 
            MaterialAction jawMaterialAction, 
            MaterialAction lidMaterialAction, 
            MaterialAction crankMaterialAction, 
            MaterialAction bodyMaterialAction, 
            StaticMeshAction skullLOD0Action, 
            StaticMeshAction skullLOD1Action, 
            StaticMeshAction skullLOD2Action, 
            StaticMeshAction jawLOD0Action, 
            StaticMeshAction jawLOD1Action, 
            StaticMeshAction lidMeshAction, 
            StaticMeshAction crankMeshAction, 
            SkinnedMeshAction bodyMeshAction,
            AudioAction musicAudioAction,
            AudioAction popUpAudioAction,
            AudioAction screamingAudioAction,
            AudioAction killPlayerAudioAction,
            AudioAction hitBodyAudioAction,
            AudioAction footstepAudioAction,
            AudioListAction crankAudioListAction,
            AudioListAction stompAudioListAction
        )
        {
            SkullMaterialAction = skullMaterialAction;
            JawMaterialAction = jawMaterialAction;
            LidMaterialAction = lidMaterialAction;
            CrankMaterialAction = crankMaterialAction;
            BodyMaterialAction = bodyMaterialAction;
            SkullLOD0Action = skullLOD0Action;
            SkullLOD1Action = skullLOD1Action;
            SkullLOD2Action = skullLOD2Action;
            JawLOD0Action = jawLOD0Action;
            JawLOD1Action = jawLOD1Action;
            LidMeshAction = lidMeshAction;
            CrankMeshAction = crankMeshAction;
            BodyMeshAction = bodyMeshAction;
            MusicAudioAction = musicAudioAction;
            PopUpAudioAction = popUpAudioAction;
            ScreamingAudioAction = screamingAudioAction;
            KillPlayerAudioAction = killPlayerAudioAction;
            HitBodyAudioAction = hitBodyAudioAction;
            FootstepAudioAction = footstepAudioAction;
            CrankAudioListAction = crankAudioListAction;
            ChaseStompAudioListAction = stompAudioListAction;
            Attachments = attachments;
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
            activeAttachments = ArmatureAttachment.ApplyAttachments(Attachments, enemy.transform.Find(BODY_LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());
            vanillaBodyMaterial = BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            BodyMaterialAction.Apply(enemy.transform.Find(BODY_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaSkullMaterial = SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            SkullMaterialAction.Apply(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaJawMaterial = JawMaterialAction.Apply(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            JawMaterialAction.Apply(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaCrankMaterial = CrankMaterialAction.Apply(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaLidMaterial = LidMaterialAction.Apply(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            vanillaSkullLOD0Mesh = SkullLOD0Action.Apply(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSkullLOD1Mesh = SkullLOD1Action.Apply(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaSkullLOD2Mesh = SkullLOD2Action.Apply(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaJawLOD0Mesh = JawLOD0Action.Apply(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaJawLOD1Mesh = JawLOD1Action.Apply(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaCrankMesh = CrankMeshAction.Apply(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<MeshFilter>());
            vanillaLidMesh = LidMeshAction.Apply(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<MeshFilter>());

            vanillaScreamAudio = ScreamingAudioAction.Apply(ref jester.screamingSFX);
            vanillaPopUpAudio = PopUpAudioAction.Apply(ref jester.popUpSFX);
            vanillaKillPlayerAudio = KillPlayerAudioAction.Apply(ref jester.killPlayerSFX);
            vanillaMusic = MusicAudioAction.Apply(ref jester.popGoesTheWeaselTheme);

            if(audioAnimEvents != null)
            {
                vanillaCrankAudio = CrankAudioListAction.Apply(ref audioAnimEvents.randomClips);
                vanillaStompAudio = ChaseStompAudioListAction.Apply(ref audioAnimEvents.randomClips2);
                vanillaFootstepAudio = FootstepAudioAction.Apply(ref audioAnimEvents.audioClip);
            }

            BodyMeshAction.Apply
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
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);
            BodyMaterialAction.Remove(enemy.transform.Find(BODY_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaBodyMaterial);

            SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);
            SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);
            SkullMaterialAction.Remove(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaSkullMaterial);

            JawMaterialAction.Remove(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaJawMaterial);
            JawMaterialAction.Remove(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaJawMaterial);

            CrankMaterialAction.Remove(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaCrankMaterial);
            LidMaterialAction.Remove(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<Renderer>(), 0, vanillaLidMaterial);

            SkullLOD0Action.Remove(enemy.transform.Find(SKULL_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD0Mesh);
            SkullLOD1Action.Remove(enemy.transform.Find(SKULL_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD1Mesh);
            SkullLOD2Action.Remove(enemy.transform.Find(SKULL_LOD2_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaSkullLOD2Mesh);

            JawLOD0Action.Remove(enemy.transform.Find(JAW_LOD0_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaJawLOD0Mesh);
            JawLOD1Action.Remove(enemy.transform.Find(JAW_LOD1_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaJawLOD1Mesh);

            CrankMeshAction.Remove(enemy.transform.Find(CRANK_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaCrankMesh);
            LidMeshAction.Remove(enemy.transform.Find(LID_PATH)?.gameObject.GetComponent<MeshFilter>(), vanillaLidMesh);

            ScreamingAudioAction.Remove(ref jester.screamingSFX, vanillaScreamAudio);
            PopUpAudioAction.Remove(ref jester.popUpSFX, vanillaPopUpAudio);
            KillPlayerAudioAction.Remove(ref jester.killPlayerSFX, vanillaKillPlayerAudio);
            MusicAudioAction.Remove(ref jester.popGoesTheWeaselTheme, vanillaMusic);

            if (audioAnimEvents != null)
            {
                CrankAudioListAction.Remove(ref audioAnimEvents.randomClips, vanillaCrankAudio);
                FootstepAudioAction.Remove(ref audioAnimEvents.audioClip, vanillaFootstepAudio);
                ChaseStompAudioListAction.Remove(ref audioAnimEvents.randomClips2, vanillaStompAudio);
            }

            BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(BODY_LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(BODY_LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                },
                enemy.transform.Find(ANCHOR_PATH)
            );
        }

        public void OnKillPlayer(JesterAI instance, GameNetcodeStuff.PlayerControllerB playerControllerB)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(KillPlayerAudioAction.WorkingClip(vanillaKillPlayerAudio));
            }
        }

        public void OnEnterPoppedState(JesterAI instance)
        {
            if(EffectsSilenced)
            {
                modCreatureEffects.PlayOneShot(PopUpAudioAction.WorkingClip(vanillaPopUpAudio));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, PopUpAudioAction.WorkingClip(vanillaPopUpAudio));
            }
        }

        public void OnHit(EnemyAI enemy, GameNetcodeStuff.PlayerControllerB attackingPlayer, bool playSoundEffect)
        {
            if(EffectsSilenced && playSoundEffect)
            {
                modCreatureEffects.PlayOneShot(HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
                WalkieTalkie.TransmitOneShotAudio(modCreatureEffects, HitBodyAudioAction.WorkingClip(enemy.enemyType.hitBodySFX));
            }
        }
    }
}