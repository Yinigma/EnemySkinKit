using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AntlerShed.EnemySkinKit.SkinAction
{
    [CustomPropertyDrawer(typeof(MaterialAction))]
    internal class MaterialActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Foldout foldout = new Foldout();
            foldout.text = property.displayName;
            PropertyField actionField = new PropertyField(property.FindPropertyRelative("actionType"));
            PropertyField materialReplacementField = new PropertyField(property.FindPropertyRelative("replacementMaterial"));
            PropertyField textureReplacementField = new PropertyField(property.FindPropertyRelative("replacementTexture"));
            foldout.contentContainer.Add(actionField);
            foldout.contentContainer.Add(materialReplacementField);
            foldout.contentContainer.Add(textureReplacementField);
            UpdateAppearance(property, materialReplacementField, textureReplacementField);
            actionField.RegisterValueChangeCallback
            (
                (ev) =>
                {
                    UpdateAppearance(property, materialReplacementField, textureReplacementField);
                }
            );
            return foldout;
        }
        private static void UpdateAppearance(SerializedProperty property, PropertyField materialReplacementField, PropertyField textureReplacementField)
        {
            if (property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE"))
            {
                materialReplacementField.style.display = DisplayStyle.Flex;
                textureReplacementField.style.display = DisplayStyle.None;
            }
            else if(property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE_TEXTURE"))
            {
                textureReplacementField.style.display = DisplayStyle.Flex;
                materialReplacementField.style.display = DisplayStyle.None;
            }
            else
            {
                textureReplacementField.style.display = DisplayStyle.None;
                materialReplacementField.style.display = DisplayStyle.None;
            }
        }
    }
}