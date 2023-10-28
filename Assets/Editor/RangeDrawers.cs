using TerrainGeneration;
using UnityEditor;
using UnityEngine;
using RangeInt = TerrainGeneration.RangeInt;

[CustomPropertyDrawer(typeof(RangeFloat))]
public class RangeFloatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var minRect = new Rect(position.x, position.y, position.width / 2 - 20, position.height);
        var maxRect = new Rect(position.x + position.width / 2 + 20, position.y, position.width / 2 - 20,
            position.height);

        EditorGUI.PropertyField(minRect, property.FindPropertyRelative("Min"), GUIContent.none);
        EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("Max"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(RangeInt))]
    public class RangeIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var minRect = new Rect(position.x, position.y, position.width / 2 - 20, position.height);
            var maxRect = new Rect(position.x + position.width / 2 + 20, position.y, position.width / 2 - 20, position.height);

            EditorGUI.PropertyField(minRect, property.FindPropertyRelative("Min"), GUIContent.none);
            EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("Max"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

