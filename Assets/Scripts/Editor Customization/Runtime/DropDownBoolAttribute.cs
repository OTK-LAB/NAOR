using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DropdownBoolAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DropdownBoolAttribute))]
public class DropdownBoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate the rect for the foldout arrow
        Rect foldoutRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        // Draw the foldout arrow
        property.boolValue = EditorGUI.Foldout(foldoutRect, property.boolValue, GUIContent.none);

        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
        EditorGUI.EndProperty();
    }
}
#endif