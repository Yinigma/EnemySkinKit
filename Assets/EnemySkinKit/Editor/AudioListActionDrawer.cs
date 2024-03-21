using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AntlerShed.EnemySkinKit.SkinAction
{
    [CustomPropertyDrawer(typeof(AudioListAction))]
    internal class AudioListActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Foldout foldout = new Foldout();
            foldout.text = property.displayName;
            PropertyField actionField = new PropertyField(property.FindPropertyRelative("actionType"));
            PropertyField replacementField = new PropertyField(property.FindPropertyRelative("replacementClips"));
            foldout.contentContainer.Add(actionField);
            foldout.contentContainer.Add(replacementField);
            if (!property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE"))
            {
                replacementField.style.display = DisplayStyle.None;

            }
            actionField.RegisterValueChangeCallback
            (
                (ev) =>
                {
                    if (property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE"))
                    {
                        replacementField.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        replacementField.style.display = DisplayStyle.None;
                    }
                }
            );
            return foldout;
        }
    }
}