using Com.BAMS.TheNegativeOne;
using UnityEditor;
using UnityEngine;
/*
[CustomEditor(typeof(CharacterStat), true)]
public class StatBaseEditor : Editor {
    GUIContent statValueField = new GUIContent("BaseValue", "Current run time value.");
    GUIContent defaultValueField = new GUIContent("Default BaseValue", "Default BaseValue.");

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.BeginHorizontal();
        //BaseValue
        SerializedProperty statValue = serializedObject.FindProperty("BaseValue");
        EditorGUILayout.PropertyField(statValue, statValueField);

        //DefaultValue
        SerializedProperty defaultValue = serializedObject.FindProperty("DefaultValue");
        EditorGUILayout.PropertyField(defaultValue, defaultValueField);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }
}
*/