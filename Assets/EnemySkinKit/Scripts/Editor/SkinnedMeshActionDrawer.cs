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
        TextElement deprecatedText = new TextElement();
        deprecatedText.text = "This method of skinning is deprecated. See the readme for more info.";
        PropertyField actionField = new PropertyField(property.FindPropertyRelative("actionType"));
        PropertyField replacementField = new PropertyField(property.FindPropertyRelative("replacementObject"));
        PropertyField mapField = new PropertyField(property.FindPropertyRelative("armatureMap"));
        foldout.contentContainer.Add(actionField);
        foldout.contentContainer.Add(deprecatedText);
        foldout.contentContainer.Add(replacementField);
        foldout.contentContainer.Add(mapField);
        UpdateDisplay(property, replacementField, mapField, deprecatedText);
        actionField.RegisterValueChangeCallback
        (
            (ev) =>
            {
                UpdateDisplay(property, replacementField, mapField, deprecatedText);
            }
        );
        return foldout;
    }

    private static void UpdateDisplay(SerializedProperty property, PropertyField replacementField, PropertyField mapField, TextElement deprecated)
    {
        string actionType = property.FindPropertyRelative("actionType").enumNames[property.FindPropertyRelative("actionType").enumValueIndex];
        if (actionType.Equals("REPLACE"))
        {
            replacementField.style.display = DisplayStyle.Flex;
            mapField.style.display = DisplayStyle.Flex;
            deprecated.style.display = DisplayStyle.Flex;
        }
        else if (actionType.Equals("REPLACE_MESH"))
        {
            replacementField.style.display = DisplayStyle.Flex;
            mapField.style.display = DisplayStyle.None;
            deprecated.style.display = DisplayStyle.None;
        }
        else
        {
            replacementField.style.display = DisplayStyle.None;
            mapField.style.display = DisplayStyle.None;
            deprecated.style.display = DisplayStyle.None;
        }
    }
}