using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntlerShed.EnemySkinKit
{
    [Serializable]
    public struct ArmatureAttachment
    {
        public string boneId;
        public GameObject attachment;
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;

        public static List<GameObject> ApplyAttachments(ArmatureAttachment[] attachments, SkinnedMeshRenderer renderer)
        {
            List<GameObject> activeAttachments = new List<GameObject>();
            foreach (ArmatureAttachment attachment in attachments)
            {
                try
                {
                    Transform parent = renderer.bones.First((tf) => tf.name.Equals(attachment.boneId));
                    GameObject instance = GameObject.Instantiate(attachment.attachment, parent);
                    instance.transform.localPosition = attachment.location;
                    instance.transform.localRotation = Quaternion.Euler(attachment.rotation);
                    instance.transform.localScale = attachment.scale;
                    activeAttachments.Add(instance);
                }
                catch (Exception e) { }
            }
            return activeAttachments;
        }

        public static void RemoveAttachments(List<GameObject> attachments)
        {
            foreach(GameObject go in attachments)
            {
                GameObject.Destroy(go);
            }
        }
    }
}
