using Assets._Game.Scripts.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDefinition))]
public class ItemDefinitionEditor : Editor
{
    SerializedProperty _traitsProp;

    List<Type> _traitTypes = new();
    string[] _traitTypeNames = Array.Empty<string>();
    int _addTypeIndex;

    void OnEnable()
    {
        _traitsProp = FindManagedReferenceArrayProperty(serializedObject);
        BuildTraitTypeCache();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Рисуем все свойства кроме m_Script и Traits (Traits — отдельным блоком)
        DrawAllPropertiesExceptTraits();

        EditorGUILayout.Space(8);

        // Рисуем Traits (если найден)
        if (_traitsProp != null)
            DrawTraitsBlock();
        else
            EditorGUILayout.HelpBox("Не найден массив [SerializeReference] traits (managed reference array).", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawAllPropertiesExceptTraits()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("ItemDefinition", EditorStyles.boldLabel);

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

                if (_traitsProp != null && it.propertyPath == _traitsProp.propertyPath)
                    continue;

                EditorGUILayout.PropertyField(it, true);
            }
        }
    }

    void DrawTraitsBlock()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(_traitsProp.displayName), EditorStyles.boldLabel);

            DrawAddTraitRow();

            if (_traitsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No traits.", MessageType.None);
                return;
            }

            for (int i = 0; i < _traitsProp.arraySize; i++)
            {
                var element = _traitsProp.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    DrawTraitHeader(i, element);
                    DrawTraitBody(element);
                }
            }
        }
    }

    void DrawAddTraitRow()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Add", GUILayout.Width(30));

            if (_traitTypeNames.Length == 0)
            {
                EditorGUILayout.HelpBox("Не найдено ни одного trait-типа (наследника ItemTraitBase).", MessageType.Warning);
                return;
            }

            _addTypeIndex = Mathf.Clamp(_addTypeIndex, 0, _traitTypeNames.Length - 1);
            _addTypeIndex = EditorGUILayout.Popup(_addTypeIndex, _traitTypeNames);

            if (GUILayout.Button("Add Trait", GUILayout.Width(90)))
            {
                Undo.RecordObject(target, "Add Trait");

                int idx = _traitsProp.arraySize;
                _traitsProp.InsertArrayElementAtIndex(idx);

                var el = _traitsProp.GetArrayElementAtIndex(idx);
                el.managedReferenceValue = Activator.CreateInstance(_traitTypes[_addTypeIndex]);

                // важно: закрепляем managed reference в объекте
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }

    void DrawTraitHeader(int index, SerializedProperty element)
    {
        string typeName = GetManagedTypeShortName(element);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField($"{index}: {typeName}", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(index == 0))
            {
                if (GUILayout.Button("▲", GUILayout.Width(26)))
                {
                    Undo.RecordObject(target, "Move Trait Up");
                    _traitsProp.MoveArrayElement(index, index - 1);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }
            }

            using (new EditorGUI.DisabledScope(index == _traitsProp.arraySize - 1))
            {
                if (GUILayout.Button("▼", GUILayout.Width(26)))
                {
                    Undo.RecordObject(target, "Move Trait Down");
                    _traitsProp.MoveArrayElement(index, index + 1);
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(26)))
            {
                Undo.RecordObject(target, "Remove Trait");
                _traitsProp.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                GUIUtility.ExitGUI();
            }
        }
    }

    void DrawTraitBody(SerializedProperty element)
    {
        // Рисуем все поля managed reference объекта
        var copy = element.Copy();
        var end = copy.GetEndProperty();

        bool enterChildren = true;
        while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
        {
            enterChildren = false;

            // Unity внутренности иногда всплывают — пропускаем
            if (copy.propertyPath.EndsWith(".managedReferenceId", StringComparison.OrdinalIgnoreCase))
                continue;

            EditorGUILayout.PropertyField(copy, true);
        }
    }

    static SerializedProperty FindManagedReferenceArrayProperty(SerializedObject so)
    {
        // Находим первое свойство: isArray && element.propertyType == ManagedReference
        // (если у тебя несколько таких массивов — можно расширить до выбора)
        var it = so.GetIterator();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (!it.isArray) continue;
            if (it.propertyType == SerializedPropertyType.String) continue; // строки тоже "array-like"

            if (it.arraySize == 0)
            {
                // если пусто — попробуем по типу элемента через временную вставку не лезем.
                // В этом случае надежнее ориентироваться по названию, но ты просил без хардкода.
                // Поэтому пустой массив не опознаём на 100% — оставим как null.
                continue;
            }

            var el = it.GetArrayElementAtIndex(0);
            if (el.propertyType == SerializedPropertyType.ManagedReference)
                return so.FindProperty(it.propertyPath);
        }

        // Фолбэк: если массив Traits пустой, а ты всё равно хочешь его найти,
        // то самый безопасный НЕ-хардкодный способ без рефлексии по backing field невозможен.
        // Поэтому: если хочешь поддержку пустого Traits — просто положи 1 trait один раз, потом можно удалять.
        return null;
    }

    static string GetManagedTypeShortName(SerializedProperty p)
    {
        // "AssemblyName Full.Type.Name"
        var full = p.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(full)) return "(null)";

        int space = full.IndexOf(' ');
        if (space < 0 || space == full.Length - 1) return full;

        var typeName = full.Substring(space + 1);
        int lastDot = typeName.LastIndexOf('.');
        return lastDot >= 0 ? typeName.Substring(lastDot + 1) : typeName;
    }

    void BuildTraitTypeCache()
    {
        var baseType = typeof(ItemTraitBase);

        _traitTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
            })
            .Where(t => t != null)
            .Where(t => baseType.IsAssignableFrom(t))
            .Where(t => t.IsClass && !t.IsAbstract)
            .OrderBy(t => t.Name)
            .ToList();

        _traitTypeNames = _traitTypes.Select(t => t.Name).ToArray();
        _addTypeIndex = Mathf.Clamp(_addTypeIndex, 0, Math.Max(0, _traitTypeNames.Length - 1));
    }
}
