using AntlerShed.EnemySkinKit.SkinAction;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SkinnedMeshAction))]
internal class StaticMeshDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        Foldout foldout = new Foldout();
        foldout.text = property.displayName;
        PropertyField actionField = new PropertyField(property.FindPropertyRelative("actionType"));
        PropertyField replacementField = new PropertyField(property.FindPropertyRelative("replacementObject"));
        PropertyField mapField = new PropertyField(property.FindPropertyRelative("armatureMap"));
        foldout.contentContainer.Add(actionField);
        foldout.contentContainer.Add(replacementField);
        foldout.contentContainer.Add(mapField);
        if (!property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE"))
        {
            replacementField.style.display = DisplayStyle.None;
            mapField.style.display = DisplayStyle.None;
        }
        actionField.RegisterValueChangeCallback
        (
            (ev) =>
            {
                if (property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex].Equals("REPLACE"))
                {
                    replacementField.style.display = DisplayStyle.Flex;
                    mapField.style.display = DisplayStyle.Flex;
                }
                else
                {
                    replacementField.style.display = DisplayStyle.None;
                    mapField.style.display = DisplayStyle.None;
                }
            }
        );
        return foldout;
    }
}