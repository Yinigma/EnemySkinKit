using AntlerShed.EnemySkinKit.AudioReflection;
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

        protected List<GameObject> activeAttachments;
        protected GameObject skinnedMeshReplacement;

        protected Dictionary<string, AudioReplacement> clipMap = new Dictionary<string, AudioReplacement>();
        protected AudioReflector modFlappingAudio;
        protected AudioReflector modCreatureEffects;
        protected AudioReflector modCreatureVoice;

        protected TulipSnakeSkin SkinData { get; }

        public TulipSnakeSkinner(TulipSnakeSkin skinData)
        {
            SkinData = skinData;
        }

        public override void Apply(GameObject enemy)
        {
            FlowerSnakeEnemy friend = enemy.GetComponent<FlowerSnakeEnemy>();
            SkinData.FlapAudioListAction.ApplyToMap(new AudioClip[] { friend.enemyType.audioClips[4], friend.enemyType.audioClips[5] }, clipMap);
            SkinData.ChuckleAudioListAction.ApplyToMap(new AudioClip[] { friend.enemyType.audioClips[0], friend.enemyType.audioClips[1], friend.enemyType.audioClips[2], friend.enemyType.audioClips[3] }, clipMap);
            SkinData.LeapAudioListAction.ApplyToMap(new AudioClip[] { friend.enemyType.audioClips[6], friend.enemyType.audioClips[7], friend.enemyType.audioClips[10] }, clipMap);
            SkinData.ScurryAudioAction.ApplyToMap(friend.enemyType.audioClips[9], clipMap);


            modFlappingAudio = CreateAudioReflector(friend.flappingAudio, clipMap, friend.NetworkObjectId); 
            friend.flappingAudio.mute = true;
            modCreatureEffects = CreateAudioReflector(friend.creatureSFX, clipMap, friend.NetworkObjectId); 
            friend.creatureSFX.mute = true;
            modCreatureVoice = CreateAudioReflector(friend.creatureVoice, clipMap, friend.NetworkObjectId); 
            friend.creatureVoice.mute = true;

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
    }
}
