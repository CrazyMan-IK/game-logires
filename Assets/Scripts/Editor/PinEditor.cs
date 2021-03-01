using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CrazyGames.Logires
{
    [CustomEditor(typeof(Pin<>), true)]
    public class PinEditor : Editor
    {
        private SerializedProperty _isInputProperty;
        private SerializedProperty _defaultValueProperty;
        private SerializedProperty _valueProperty;

        private void OnEnable()
        {
            _isInputProperty = serializedObject.FindProperty("_isInput");
            _defaultValueProperty = serializedObject.FindProperty("_defaultValue");
            _valueProperty = serializedObject.FindProperty("_value");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_isInputProperty);
            if (_defaultValueProperty != null)
            {
                EditorGUILayout.PropertyField(_defaultValueProperty);
                EditorGUI.BeginDisabledGroup(true);
                if (_defaultValueProperty.isArray && _defaultValueProperty.arrayElementType == "bool")
                {
                    string bits = "";
                    for (int i = 0; i < _valueProperty.arraySize; i++)
                    {
                        bits += _valueProperty.GetArrayElementAtIndex(i).boolValue ? "1" : "0";
                    }
                    EditorGUILayout.TextField(bits);
                }
                else
                {
                    EditorGUILayout.PropertyField(_valueProperty);
                }
                EditorGUI.EndDisabledGroup();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}