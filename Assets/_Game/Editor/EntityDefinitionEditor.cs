using Assets._Game.Scripts.Entities;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityDefinition))]
public sealed class EntityDefinitionEditor : Editor
{
    SerializedProperty _idProp;
    SerializedProperty _modulesProp;

    Type[] _moduleTypes = Array.Empty<Type>();
    string[] _moduleTypeNames = Array.Empty<string>();
    int _addTypeIndex;

    void OnEnable()
    {
        _idProp = serializedObject.FindProperty("<Id>k__BackingField");
        _modulesProp = serializedObject.FindProperty("_modules"); // важно: имя поля

        BuildModuleTypeCache();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(8);

        DrawAllPropertiesExceptIdScriptAndModules();

        EditorGUILayout.Space(8);

        if (_modulesProp != null)
            DrawModulesBlock();
        else
            EditorGUILayout.HelpBox("Не найдено поле '_modules'. Проверь имя поля в EntityDefinition.", MessageType.Error);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawAllPropertiesExceptIdScriptAndModules()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("EntityDefinition", EditorStyles.boldLabel);

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

                if (_modulesProp != null && it.propertyPath == _modulesProp.propertyPath)
                    continue;

                EditorGUILayout.PropertyField(it, true);
            }
        }
    }

    void DrawModulesBlock()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);

            DrawAddModuleRow();

            if (_modulesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No modules.", MessageType.None);
                return;
            }

            for (int i = 0; i < _modulesProp.arraySize; i++)
            {
                var element = _modulesProp.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    DrawModuleHeader(i, element);
                    DrawModuleBody(element);
                }
            }
        }
    }

    void DrawAddModuleRow()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Add", GUILayout.Width(30));

            if (_moduleTypeNames.Length == 0)
            {
                EditorGUILayout.HelpBox("Не найдено ни одного типа модуля (наследника EntityModuleDefinition).", MessageType.Warning);
                return;
            }

            _addTypeIndex = Mathf.Clamp(_addTypeIndex, 0, _moduleTypeNames.Length - 1);
            _addTypeIndex = EditorGUILayout.Popup(_addTypeIndex, _moduleTypeNames);

            if (GUILayout.Button("Add Module", GUILayout.Width(100)))
            {
                Undo.RecordObject(target, "Add Module");

                int idx = _modulesProp.arraySize;
                _modulesProp.InsertArrayElementAtIndex(idx);

                var el = _modulesProp.GetArrayElementAtIndex(idx);
                el.managedReferenceValue = Activator.CreateInstance(_moduleTypes[_addTypeIndex]);
                el.isExpanded = true;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
        }
    }

    void DrawModuleHeader(int index, SerializedProperty element)
    {
        string typeName = GetManagedTypeShortName(element);

        using (new EditorGUILayout.HorizontalScope())
        {
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, $"{index}: {typeName}", true, EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(index == 0))
            {
                if (GUILayout.Button("▲", GUILayout.Width(26)))
                {
                    Undo.RecordObject(target, "Move Module Up");
                    _modulesProp.MoveArrayElement(index, index - 1);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                    GUIUtility.ExitGUI();
                }
            }

            using (new EditorGUI.DisabledScope(index == _modulesProp.arraySize - 1))
            {
                if (GUILayout.Button("▼", GUILayout.Width(26)))
                {
                    Undo.RecordObject(target, "Move Module Down");
                    _modulesProp.MoveArrayElement(index, index + 1);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                    GUIUtility.ExitGUI();
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(26)))
            {
                Undo.RecordObject(target, "Remove Module");

                // ВАЖНО: для ManagedReference достаточно одного delete.
                _modulesProp.DeleteArrayElementAtIndex(index);

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
        }
    }

    void DrawModuleBody(SerializedProperty element)
    {
        if (!element.isExpanded)
            return;

        // Рисуем поля managed reference объекта (как у тебя в traits)
        var copy = element.Copy();
        var end = copy.GetEndProperty();

        bool enterChildren = true;
        while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
        {
            enterChildren = false;

            if (copy.propertyPath.EndsWith(".managedReferenceId", StringComparison.OrdinalIgnoreCase))
                continue;

            EditorGUILayout.PropertyField(copy, true);
        }
    }

    static string GetManagedTypeShortName(SerializedProperty p)
    {
        var full = p.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(full))
            return "(null)";

        int space = full.IndexOf(' ');
        if (space < 0 || space == full.Length - 1)
            return full;

        var typeName = full.Substring(space + 1);
        int lastDot = typeName.LastIndexOf('.');
        return lastDot >= 0 ? typeName.Substring(lastDot + 1) : typeName;
    }

    void BuildModuleTypeCache()
    {
        // Unity 6: TypeCache быстрый и стабильный
        var baseType = typeof(EntityModuleDefinition);

        _moduleTypes = TypeCache.GetTypesDerivedFrom<EntityModuleDefinition>()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToArray();

        _moduleTypeNames = _moduleTypes.Select(t => t.Name).ToArray();
        _addTypeIndex = Mathf.Clamp(_addTypeIndex, 0, Math.Max(0, _moduleTypeNames.Length - 1));
    }
}
