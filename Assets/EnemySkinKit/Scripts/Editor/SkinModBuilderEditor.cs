using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AntlerShed.EnemySkinKit
{
    [CustomEditor(typeof(SkinModBuilder))]
    internal class SkinModBuilderEditor : Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement editor = new VisualElement();

            Foldout manifest = new Foldout();
            manifest.text = "Mod Info";
            manifest.Add(new PropertyField(serializedObject.FindProperty("modName")));
            manifest.Add(new PropertyField(serializedObject.FindProperty("author")));
            manifest.Add(new PropertyField(serializedObject.FindProperty("modGUID")));
            manifest.Add(new PropertyField(serializedObject.FindProperty("description")));
            manifest.Add(new PropertyField(serializedObject.FindProperty("readMe")));
            manifest.Add(new PropertyField(serializedObject.FindProperty("icon")));

            Foldout version = new Foldout();
            version.text = "Version";
            version.Add(new PropertyField(serializedObject.FindProperty("major")));
            version.Add(new PropertyField(serializedObject.FindProperty("minor")));
            version.Add(new PropertyField(serializedObject.FindProperty("patch")));
            manifest.Add(version);

            editor.Add(manifest);

            editor.Add(new PropertyField(serializedObject.FindProperty("skins")));
            editor.Add(new PropertyField(serializedObject.FindProperty("configs")));
            Button createButton = new Button();
            createButton.text = "Generate Mod Files";
            createButton.clicked += genModFiles;
            editor.Add(createButton);
            return editor;
        }

        private void genModFiles()
        {
            (target as SkinModBuilder).BuildMod();
        }
    }
}
