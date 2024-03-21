using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class JesterSkinner : BaseSkinner
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
        protected ArmatureAttachment[] Attachments { get; }

        public JesterSkinner
        (
            bool muteSoundEffects,
            bool muteVoice,
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
            AudioAction killPlayerAudioAction
        ) : base(muteSoundEffects, muteVoice)
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
            Attachments = attachments;
        }

        //This enemy has so many goddamn parts
        public override void Apply(GameObject enemy)
        {
            base.Apply(enemy);
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

            vanillaScreamAudio = ScreamingAudioAction.Apply(ref enemy.GetComponent<JesterAI>().screamingSFX);
            vanillaPopUpAudio = PopUpAudioAction.Apply(ref enemy.GetComponent<JesterAI>().popUpSFX);
            vanillaKillPlayerAudio = KillPlayerAudioAction.Apply(ref enemy.GetComponent<JesterAI>().killPlayerSFX);
            vanillaMusic = MusicAudioAction.Apply(ref enemy.GetComponent<JesterAI>().popGoesTheWeaselTheme);

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
        }

        public override void Remove(GameObject enemy)
        {
            base.Remove(enemy);
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

            ScreamingAudioAction.Remove(ref enemy.GetComponent<JesterAI>().screamingSFX, vanillaScreamAudio);
            PopUpAudioAction.Remove(ref enemy.GetComponent<JesterAI>().popUpSFX, vanillaPopUpAudio);
            KillPlayerAudioAction.Remove(ref enemy.GetComponent<JesterAI>().killPlayerSFX, vanillaKillPlayerAudio);
            MusicAudioAction.Remove(ref enemy.GetComponent<JesterAI>().popGoesTheWeaselTheme, vanillaMusic);

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
    }
}