using Assets._Game.Scripts.Entities;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityDefinition))]
public sealed class EntityDefinitionEditor : Editor
{
    SerializedProperty _idProp;

    void OnEnable()
    {
        _idProp = serializedObject.FindProperty("<Id>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawIdBlock();
        EditorGUILayout.Space(6);

        DrawAllExceptIdAndScript();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawIdBlock()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);

            if (_idProp != null)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(_idProp, new GUIContent("Id"));
            }
            else
            {
                EditorGUILayout.HelpBox("Id property not found (backing field).", MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Regenerate Id", GUILayout.Width(120)))
                {
                    Undo.RecordObject(target, "Regenerate EntityDefinition Id");

                    var def = (EntityDefinition)target;
                    def.Id = Guid.NewGuid().ToString();

                    EditorUtility.SetDirty(def);
                    serializedObject.Update();
                }
            }
        }
    }

    void DrawAllExceptIdAndScript()
    {
        SerializedProperty it = serializedObject.GetIterator();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (it.propertyPath == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(it);
                continue;
            }

            if (_idProp != null && it.propertyPath == _idProp.propertyPath)
                continue;

            EditorGUILayout.PropertyField(it, true);
        }
    }
}
