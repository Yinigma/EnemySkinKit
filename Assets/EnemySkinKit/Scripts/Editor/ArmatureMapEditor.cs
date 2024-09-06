using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;

namespace AntlerShed.EnemySkinKit.ArmatureReflection
{
    [CustomEditor(typeof(ArmatureMapGenerator))]
    internal class ArmatureMapInspector : Editor
    {
        private Transform source;
        private SkinnedMeshRenderer destination;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            return Draw(inspector);
        }

        private VisualElement Draw(VisualElement inspector)
        {
            ArmatureMapGenerator generator = (ArmatureMapGenerator) target;
            if((generator.sourceBones?.Length ?? 0) == 0 || (generator.destBones?.Length ?? 0) == 0)
            {
                ObjectField sourceMeshField = new ObjectField("Source");
                sourceMeshField.value = source;
                sourceMeshField.objectType = typeof(Transform);
                sourceMeshField.allowSceneObjects = false;
                sourceMeshField.RegisterValueChangedCallback(ev => OnSourceChanged(ev.newValue as Transform, generator, inspector));
                ObjectField destMeshField = new ObjectField("Destination");
                destMeshField.value = destination;
                destMeshField.objectType = typeof(SkinnedMeshRenderer);
                destMeshField.allowSceneObjects = false;
                destMeshField.RegisterValueChangedCallback(ev => OnDestChanged(ev.newValue as SkinnedMeshRenderer, generator, inspector));
                inspector.Add(sourceMeshField);
                inspector.Add(destMeshField);
            }
            else
            {
                ListView listView = new ListView(generator.tfMap);
                listView.makeItem = () => new PopupField<string>();
                listView.bindItem = (elem, index) =>
                {
                    PopupField<string> popup = elem as PopupField<string>;
                    popup.formatListItemCallback = (tf) => tf ?? "__None__";
                    popup.choices = new List<string>() { null }.Concat(generator.destBones).ToList();
                    popup.index = generator.tfMap[index] + 1;
                    popup.label = generator.sourceBones[index];
                    popup.RegisterValueChangedCallback
                    (
                        ev =>
                        {
                            Undo.RecordObject(generator, "SetMapEntry");
                            int destIndex = Array.IndexOf(generator.destBones, ev.newValue);
                            generator.tfMap[index] = destIndex;
                            EditorUtility.SetDirty(target);
                        }
                    );
                };
                inspector.Add(listView);
                Button clearButton = new Button();
                clearButton.text = "Clear Data";
                clearButton.clicked += () =>
                {
                    Undo.RecordObject(generator, "Clear Map Data");
                    generator.tfMap = null;
                    generator.destBones = null;
                    generator.sourceBones = null;
                    source = null;
                    destination = null;
                    inspector.Clear();
                    Draw(inspector);
                    EditorUtility.SetDirty(generator);
                };
                /*Button saveButton = new Button();
                saveButton.text = "Save Data";
                saveButton.clicked += () =>
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                };
                inspector.Add(saveButton);*/
                inspector.Add(clearButton);
            }
            return inspector;
        }

        private void OnSourceChanged(Transform sourceTf, ArmatureMapGenerator generator, VisualElement inspector)
        {
            source = sourceTf;
            initTfMap(generator, inspector);
        }

        private void OnDestChanged(SkinnedMeshRenderer renderer, ArmatureMapGenerator generator, VisualElement inspector)
        {
            destination = renderer;
            initTfMap(generator, inspector);
        }

        private void initTfMap(ArmatureMapGenerator generator, VisualElement inspector)
        {
            Undo.RecordObject(generator, "Load Armature Data");
            if (source != null && destination != null)
            {
                Transform[] sourceBones = source.transform.GetComponentsInChildren<Transform>();
                generator.tfMap = new int[sourceBones.Length];
                generator.sourceBones = new string[sourceBones.Length];
                generator.destBones = new string[destination.bones.Length];
                for (int i = 0; i < destination.bones.Length; i++)
                {
                    generator.destBones[i] = destination.bones[i].name;
                }
                for (int i = 0; i < generator.tfMap.Length; i++)
                {
                    generator.sourceBones[i] = sourceBones[i].name;
                    int destIndex = Array.IndexOf(generator.destBones, generator.sourceBones[i]);
                    generator.tfMap[i] = destIndex;
                }
                inspector.Clear();
                Draw(inspector);
            }
            EditorUtility.SetDirty(generator);
        }
    }
}
