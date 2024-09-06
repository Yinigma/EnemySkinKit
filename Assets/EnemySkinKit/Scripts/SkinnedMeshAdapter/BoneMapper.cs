using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.SkinAction
{ 
    internal class BoneMapper
    {
        public static void Apply(SkinnedMeshRenderer sourceSkin, SkinnedMeshRenderer targetSkin)
        {
            if(sourceSkin.bones.Length != targetSkin.bones.Length)
            {
                if(EnemySkinKit.LogLevelSetting >= LogLevel.WARN) EnemySkinKit.SkinKitLogger.LogWarning($"Modded armature does not contain the same number of bones as the vanilla armature. Vanilla bone number: {sourceSkin.bones.Length}. Modded bone number {targetSkin.bones.Length}. You're encouraged to use the armatures from the models provided in the EnemySkinKit's EnemySourceModels folder. See readme for more details.");
            }
            Transform[] newBones = new Transform[targetSkin.bones.Length];
            Dictionary<string, Transform> boneCache = sourceSkin.bones.ToDictionary((Transform bone) => (bone.name), (Transform bone) => bone);
            List<string> missingBones = new List<string>();
            for (int i = 0; i < targetSkin.bones.Length; i++)
            {
                if (boneCache.ContainsKey(targetSkin.bones[i].name))
                {
                    newBones[i] = boneCache[targetSkin.bones[i].name];
                }
                else
                {
                    missingBones.Add(targetSkin.bones[i].name);
                }
            }
            if (missingBones.Count > 0)
            {
                EnemySkinKit.SkinKitLogger.LogWarning($"Vanilla armature was missing the bones [{missingBones.Aggregate("", (cur, name) => $"{cur}\"{name}\", ")}] present in modded armature. All other bones (if any) in the modded armature have");
            }
            targetSkin.bones = newBones;
            targetSkin.rootBone = sourceSkin.rootBone;
        }
    }
}
