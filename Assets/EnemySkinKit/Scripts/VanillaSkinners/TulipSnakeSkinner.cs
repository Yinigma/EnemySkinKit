using AntlerShed.EnemySkinKit.SkinAction;
using AntlerShed.SkinRegistry;
using AntlerShed.SkinRegistry.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.Vanilla
{
    public class TulipSnakeSkinner : BaseSkinner, TulipSnakeEventHandler
    {
        protected const string LOD0_PATH = "FlowerLizardModel/LODContainer/LOD0";
        protected const string LOD1_PATH = "FlowerLizardModel/LODContainer/LOD1";
        protected const string LOD2_PATH = "FlowerLizardModel/LODContainer/LOD2";
        protected const string ANCHOR_PATH = "FlowerLizardModel/AnimContainer";

        protected VanillaMaterial[] vanillaBodyMaterial = new VanillaMaterial[3];

        protected bool VoiceSilenced => SkinData.FlapAudioListAction.actionType != AudioListActionType.RETAIN ||
            SkinData.LeapAudioListAction.actionType != AudioListActionType.RETAIN ||
            SkinData.ChuckleAudioListAction.actionType != AudioListActionType.RETAIN;

        protected bool FlapSilenced => SkinData.FlapAudioListAction.actionType != AudioListActionType.RETAIN ||
            SkinData.ScurryAudioAction.actionType != AudioActionType.RETAIN;

        protected AudioClip[] vanillaFlapClips;
        protected AudioClip[] vanillaChuckleClips;
        protected AudioClip[] vanillaLeapClips;
        protected AudioSource modCreatureVoice;
        protected AudioSource modFlapAudio;
        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected TulipSnakeSkin SkinData { get; }

        public TulipSnakeSkinner(TulipSnakeSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            FlowerSnakeEnemy friend = enemy.GetComponent<FlowerSnakeEnemy>();
            vanillaFlapClips = new AudioClip[] { friend.enemyType.audioClips[4], friend.enemyType.audioClips[5] };
            vanillaChuckleClips = new AudioClip[] { friend.enemyType.audioClips[0], friend.enemyType.audioClips[1], friend.enemyType.audioClips[2], friend.enemyType.audioClips[3] };
            vanillaLeapClips = new AudioClip[] { friend.enemyType.audioClips[6], friend.enemyType.audioClips[7], friend.enemyType.audioClips[10] };
            if (VoiceSilenced)
            {
                modCreatureVoice = CreateModdedAudioSource(friend.creatureVoice, "modVoice");
                friend.creatureVoice.mute = true;
            }
            if (VoiceSilenced)
            {
                modFlapAudio = CreateModdedAudioSource(friend.creatureVoice, "modFlapAudio");
                friend.flappingAudio.mute = true;
            }
            activeAttachments = ArmatureAttachment.ApplyAttachments(SkinData.Attachments, enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>());

            SkinData.BodyMaterialAction.ApplyRef(ref friend.randomSkinColor[0]);
            SkinData.BodyMaterialAction.ApplyRef(ref friend.randomSkinColor[1]);
            SkinData.BodyMaterialAction.ApplyRef(ref friend.randomSkinColor[2]);

            vanillaBodyMaterial[0] = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD0_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial[1] = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD1_PATH)?.gameObject.GetComponent<Renderer>(), 0);
            vanillaBodyMaterial[2] = SkinData.BodyMaterialAction.Apply(enemy.transform.Find(LOD2_PATH)?.gameObject.GetComponent<Renderer>(), 0);

            skinnedMeshReplacement = SkinData.BodyMeshAction.Apply
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
                    { "Armature", enemy.transform.Find( $"{ANCHOR_PATH}/Armature" ) },
                    { "Abdomen", enemy.transform.Find( $"{ANCHOR_PATH}/Armature/Belly/Abdomen" ) }
                }
            );
            EnemySkinRegistry.RegisterEnemyEventHandler(friend, this);
        }

        public override void Remove(GameObject enemy)
        {
            FlowerSnakeEnemy friend = enemy.GetComponent<FlowerSnakeEnemy>();
            EnemySkinRegistry.RemoveEnemyEventHandler(friend, this);
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modCreatureVoice);
                friend.creatureVoice.mute = false;
            }
            if (VoiceSilenced)
            {
                DestroyModdedAudioSource(modFlapAudio);
                friend.flappingAudio.mute = false;
            }
            ArmatureAttachment.RemoveAttachments(activeAttachments);

            //Thank god for deterministic RNG
            int random = new System.Random(StartOfRound.Instance.randomMapSeed + (int)friend.NetworkObjectId).Next(0, 100);
            VanillaMaterial mat = vanillaBodyMaterial[random < 33 ? 0 : random <= 66 ? 2 : 1];

            SkinData.BodyMaterialAction.RemoveRef(ref friend.randomSkinColor[0], vanillaBodyMaterial[0]);
            SkinData.BodyMaterialAction.RemoveRef(ref friend.randomSkinColor[1], vanillaBodyMaterial[1]);
            SkinData.BodyMaterialAction.RemoveRef(ref friend.randomSkinColor[2], vanillaBodyMaterial[2]);

            if(SkinData.BodyMaterialAction.actionType == MaterialActionType.REPLACE && mat.material != null)
            {
                foreach (SkinnedMeshRenderer rend in friend.skinnedMeshRenderers)
                {
                    rend.sharedMaterial = mat.material;
                }
            }
            else if(SkinData.BodyMaterialAction.actionType == MaterialActionType.REPLACE_TEXTURE && mat.texture != null)
            {
                foreach (SkinnedMeshRenderer rend in friend.skinnedMeshRenderers)
                {
                    rend.sharedMaterial.mainTexture = mat.texture;
                }
            }

            SkinData.BodyMeshAction.Remove
            (
                new SkinnedMeshRenderer[]
                {
                    enemy.transform.Find(LOD0_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD1_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>(),
                    enemy.transform.Find(LOD2_PATH)?.gameObject?.GetComponent<SkinnedMeshRenderer>()
                },
                skinnedMeshReplacement
            );
        }

        public void OnChuckle(FlowerSnakeEnemy snake)
        {
            if (VoiceSilenced && modCreatureVoice != null)
            {
                AudioClip[] workingEffects = SkinData.FlapAudioListAction.WorkingClips(vanillaFlapClips).Concat(SkinData.ChuckleAudioListAction.WorkingClips(vanillaChuckleClips)).ToArray();
                if(workingEffects.Length > 0)
                {
                    RoundManager.PlayRandomClip(modCreatureVoice, workingEffects, randomize: true, 1f, 0, workingEffects.Length);
                }
            }
        }

        public void OnStartLeap(FlowerSnakeEnemy snake)
        {
            if (FlapSilenced)
            {
                modFlapAudio?.Stop();
            }
            if (VoiceSilenced)
            {
                modCreatureVoice?.Stop();
                AudioClip[] leapEffects = SkinData.LeapAudioListAction.WorkingClips(vanillaLeapClips);
                if(leapEffects.Length > 0)
                {
                    modCreatureVoice?.PlayOneShot(leapEffects[UnityEngine.Random.Range(0, leapEffects.Length - 1)]);
                }
            }
        }

        public void OnStopLeap(FlowerSnakeEnemy snake)
        {
            if (FlapSilenced)
            {
                if (!snake.isEnemyDead)
                {
                    modFlapAudio?.PlayOneShot(SkinData.ScurryAudioAction.WorkingClip(snake.enemyType.audioClips[9]));
                }
            }
        }

        public void OnStopCling(FlowerSnakeEnemy snake)
        {
            if (FlapSilenced)
            {
                if (!snake.isEnemyDead)
                {
                    modFlapAudio?.PlayOneShot(SkinData.ScurryAudioAction.WorkingClip(snake.enemyType.audioClips[9]));
                }
            }
        }

        public void OnStartedFlapping(FlowerSnakeEnemy snake)
        {
            if(FlapSilenced && modFlapAudio!=null)
            {
                AudioClip[] flapEffects = SkinData.FlapAudioListAction.WorkingClips(vanillaFlapClips);
                if(flapEffects.Length > 0)
                {
                    modFlapAudio.pitch = UnityEngine.Random.Range(0.85f, 1.1f);
                    modFlapAudio.PlayOneShot(flapEffects[UnityEngine.Random.Range(0, flapEffects.Length-1)]);
                }
            }
        }

        public void OnStoppedFlapping(FlowerSnakeEnemy snake)
        {
            if (FlapSilenced)
            {
                modFlapAudio?.Stop();
            }
        }

        public void OnSpawn(EnemyAI enemy)
        {
            if (FlapSilenced && modFlapAudio != null)
            {
                modFlapAudio.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                modFlapAudio.PlayOneShot(SkinData.ScurryAudioAction.WorkingClip(enemy.enemyType.audioClips[9]));
            }
        }

        public void OnKilled(EnemyAI enemy)
        {
            if(VoiceSilenced)
            {
                modCreatureVoice?.Stop();
            }
            if(FlapSilenced)
            {
                modFlapAudio?.Stop();
            }
        }

        public void OnEnemyUpdate(EnemyAI enemy)
        {
            if(FlapSilenced && modFlapAudio != null)
            {
                FlowerSnakeEnemy friend = enemy as FlowerSnakeEnemy;
                modFlapAudio.volume = friend.flappingAudio.volume;
            }
        }
    }
}
