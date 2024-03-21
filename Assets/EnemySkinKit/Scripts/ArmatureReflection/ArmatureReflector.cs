using AntlerShed.SkinRegistry;
using DunGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AntlerShed.EnemySkinKit.ArmatureReflection
{
    public class ArmatureReflector : MonoBehaviour
    {
        private SkinnedMeshRenderer target;
        private SkinnedMeshRenderer source;

        private Matrix4x4[] destRestPose;
        private Matrix4x4[] sourceBindPose;
        private int?[] indexMap;
        private Dictionary<Transform, int> additionalMappings = new Dictionary<Transform, int>();

        public void Init(SkinnedMeshRenderer target, SkinnedMeshRenderer source, ArmatureMapGenerator generator, Dictionary<string, Transform> manualMappings)
        {
            this.target = target;
            this.source = source;
            
            destRestPose = target.bones.Select(tf => Matrix4x4.TRS(tf.localPosition, tf.localRotation, tf.localScale)).ToArray();
            sourceBindPose = source.bones.Select(tf => Matrix4x4.TRS(tf.localPosition, tf.localRotation, tf.localScale)).ToArray();

            Dictionary<string, int> nameMap = new Dictionary<string, int>();
            for( int i = 0; i < generator.tfMap.Length; i++ )
            {
                if(generator.tfMap[i] != -1)
                {
                    int dex = Array.FindIndex( target.bones, (bone) => bone.name.Equals( generator.destBones[ generator.tfMap[i] ] ) );
                    if(dex!=-1)
                    {
                        nameMap.Add(generator.sourceBones[i], dex);
                    }
                }
            }

            indexMap = source.bones.Select<Transform, int?>((tf) => nameMap.ContainsKey(tf.name) ? nameMap[tf.name] : null).ToArray();

            if(manualMappings != null)
            {
                foreach (string boneName in manualMappings.Keys)
                {
                    int dex = Array.FindIndex(target.bones, (bone) => bone.name.Equals(boneName));
                    if (dex != -1)
                    {
                        additionalMappings.Add(manualMappings[boneName], dex);
                    }
                }
            }
        }

        void Update()
        {
            if (target != null && source != null)
            {
                reflectAnimation(source.bones, target.bones);
            }
        }

        private void reflectAnimation(Transform[] sourceInstance, Transform[] destInstance)
        {
            for (int i = 0; i < sourceInstance.Length; i++)
            {
                if (indexMap[i] != null)
                {

                    int destDex = indexMap[i] ?? 0;
                    //Matrix4x4 differentialPose = destRestPose[destDex] * (Matrix4x4.TRS(sourceInstance[i].localPosition, sourceInstance[i].localRotation, sourceInstance[i].localScale) * sourceBindPose[i]);

                    destInstance[destDex].localPosition = destRestPose[destDex].GetPosition() + (sourceInstance[i].localPosition - sourceBindPose[i].GetPosition());
                    destInstance[destDex].localRotation = sourceInstance[i].localRotation * Quaternion.Inverse(sourceBindPose[i].rotation) * destRestPose[destDex].rotation;
                    destInstance[destDex].localScale = Vector3.Scale(destRestPose[destDex].lossyScale, Vector3.Scale(sourceInstance[i].localScale, new Vector3(1.0f / sourceBindPose[i].lossyScale.x, 1.0f / sourceBindPose[i].lossyScale.y, 1.0f / sourceBindPose[i].lossyScale.z)));
                    //Debug.Log($"mapping {sourceInstance[i].name} to {destInstance[destDex].name}");
                }
            }

            foreach(Transform source in additionalMappings.Keys)
            {
                if(source!=null)
                {
                    int destDex = additionalMappings[source];
                    destInstance[destDex].localPosition = source.localPosition;
                    destInstance[destDex].localRotation = source.localRotation;
                    destInstance[destDex].localScale = source.localScale;
                }
            }
        }
    }
}
