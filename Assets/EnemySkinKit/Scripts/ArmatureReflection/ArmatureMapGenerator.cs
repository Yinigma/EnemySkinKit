using System;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit.ArmatureReflection
{
    [CreateAssetMenu(fileName = "ArmatureMap", menuName = "EnemySkinKit/Animation/ArmatureMapping", order = 1)]
    public class ArmatureMapGenerator : ScriptableObject
    {
        [SerializeField]
        public int[] tfMap;
        [SerializeField]
        public string[] sourceBones;
        [SerializeField]
        public string[] destBones;
    }
}