using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Quests.Objectives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestDefinition))]
public sealed class QuestDefinitionEditor : Editor
{
    // Id (auto-property backing field)
    SerializedProperty _idProp;

    // Title (auto-property backing field)
    SerializedProperty _titleProp;

    // Objectives (SerializeReference array)
    SerializedProperty _objectivesProp;

    List<Type> _objectiveTypes = new();
    string[] _objectiveTypeNames = Array.Empty<string>();
    int _addTypeIndex;

    void OnEnable()
    {
        _idProp = serializedObject.FindProperty("<Id>k__BackingField");
        _titleProp = serializedObject.FindProperty("<Title>k__BackingField");
        _objectivesProp = serializedObject.FindProperty("<Objectives>k__BackingField");
        BuildObjectiveTypeCache();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(8);

        // Draw main properties
        DrawMainProperties();

        EditorGUILayout.Space(8);

        // Draw Objectives block
        if (_objectivesProp != null)
            DrawObjectivesBlock();
        else
            EditorGUILayout.HelpBox("Objectives property not found.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawMainProperties()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Quest Definition", EditorStyles.boldLabel);

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

                if (_objectivesProp != null && it.propertyPath == _objectivesProp.propertyPath)
                    continue;

                EditorGUILayout.PropertyField(it, true);
            }
        }
    }

    void DrawObjectivesBlock()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);

            DrawAddObjectiveRow();

            if (_objectivesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No objectives. Add at least one objective to the quest.", MessageType.Info);
                return;
            }

            int indexToRemove = -1;
            int indexToMoveUp = -1;
            int indexToMoveDown = -1;

            for (int i = 0; i < _objectivesProp.arraySize; i++)
            {
                var element = _objectivesProp.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    DrawObjectiveHeader(i, element, out bool remove, out bool moveUp, out bool moveDown);

                    if (remove)
                        indexToRemove = i;
                    if (moveUp)
                        indexToMoveUp = i;
                    if (moveDown)
                        indexToMoveDown = i;

                    if (indexToRemove < 0 && indexToMoveUp < 0 && indexToMoveDown < 0)
                        DrawObjectiveBody(element);
                }
            }

            // Handle actions after iteration to avoid SerializedProperty invalidation
            if (indexToRemove >= 0)
            {
                Undo.RecordObject(target, "Remove Objective");
                _objectivesProp.DeleteArrayElementAtIndex(indexToRemove);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
            else if (indexToMoveUp >= 0)
            {
                Undo.RecordObject(target, "Move Objective Up");
                _objectivesProp.MoveArrayElement(indexToMoveUp, indexToMoveUp - 1);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
            else if (indexToMoveDown >= 0)
            {
                Undo.RecordObject(target, "Move Objective Down");
                _objectivesProp.MoveArrayElement(indexToMoveDown, indexToMoveDown + 1);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
        }
    }

    void DrawAddObjectiveRow()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Add", GUILayout.Width(30));

            if (_objectiveTypeNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No ObjectiveDefinition types found.", MessageType.Warning);
                return;
            }

            _addTypeIndex = Mathf.Clamp(_addTypeIndex, 0, _objectiveTypeNames.Length - 1);
            _addTypeIndex = EditorGUILayout.Popup(_addTypeIndex, _objectiveTypeNames);

            if (GUILayout.Button("Add Objective", GUILayout.Width(100)))
            {
                Undo.RecordObject(target, "Add Objective");

                int idx = _objectivesProp.arraySize;
                _objectivesProp.InsertArrayElementAtIndex(idx);

                var el = _objectivesProp.GetArrayElementAtIndex(idx);
                el.managedReferenceValue = Activator.CreateInstance(_objectiveTypes[_addTypeIndex]);

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }

    void DrawObjectiveHeader(int index, SerializedProperty element, out bool remove, out bool moveUp, out bool moveDown)
    {
        remove = false;
        moveUp = false;
        moveDown = false;

        string typeName = GetManagedTypeShortName(element);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField($"{index}: {typeName}", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(index == 0))
            {
                if (GUILayout.Button("▲", GUILayout.Width(30)))
                {
                    moveUp = true;
                }
            }

            using (new EditorGUI.DisabledScope(index >= _objectivesProp.arraySize - 1))
            {
                if (GUILayout.Button("▼", GUILayout.Width(30)))
                {
                    moveDown = true;
                }
            }

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                remove = true;
            }
        }
    }

    void DrawObjectiveBody(SerializedProperty element)
    {
        if (element.managedReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Null objective", MessageType.Warning);
            return;
        }

        SerializedProperty it = element.Copy();
        SerializedProperty end = it.GetEndProperty();

        it.NextVisible(true);
        while (!SerializedProperty.EqualContents(it, end))
        {
            EditorGUILayout.PropertyField(it, true);
            if (!it.NextVisible(false))
                break;
        }
    }

    void BuildObjectiveTypeCache()
    {
        _objectiveTypes.Clear();

        var baseType = typeof(ObjectiveDefinition);
        var assembly = baseType.Assembly;

        _objectiveTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToList();

        _objectiveTypeNames = _objectiveTypes
            .Select(t => t.Name)
            .ToArray();
    }

    static string GetManagedTypeShortName(SerializedProperty prop)
    {
        string fullTypeName = prop.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(fullTypeName))
            return "null";

        int lastDot = fullTypeName.LastIndexOf('.');
        return lastDot >= 0 ? fullTypeName.Substring(lastDot + 1) : fullTypeName;
    }
}
