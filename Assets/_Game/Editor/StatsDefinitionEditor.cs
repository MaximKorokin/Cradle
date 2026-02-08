using Assets._Game.Scripts.Entities.Stats;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(StatsDefinition))]
public sealed class StatsDefinitionEditor : Editor
{
    private const string StatsPropName = "<Stats>k__BackingField";
    private const string IdPropName = "<Id>k__BackingField";
    private const string DefaultBasePropName = "<DefaultBase>k__BackingField";

    private SerializedProperty _statsProp;
    private ReorderableList _list;

    private void OnEnable()
    {
        _statsProp = serializedObject.FindProperty(StatsPropName);

        _list = new ReorderableList(serializedObject, _statsProp,
            draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);

        _list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Stats");
        };

        _list.elementHeightCallback = index =>
        {
            var el = _statsProp.GetArrayElementAtIndex(index);
            // We draw compact (1 line), so fixed height works.
            return EditorGUIUtility.singleLineHeight + 6f;
        };

        _list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            var el = _statsProp.GetArrayElementAtIndex(index);
            var idProp = el.FindPropertyRelative(IdPropName);
            var baseProp = el.FindPropertyRelative(DefaultBasePropName);

            if (idProp == null || baseProp == null)
            {
                EditorGUI.LabelField(rect, $"StatDefinition fields not found (Id/Base).");
                return;
            }

            string idName = GetEnumDisplayName(idProp);
            bool isDup = IsDuplicate(index, idProp);

            if (isDup)
            {
                var bg = rect;
                bg.xMin -= 2f; bg.xMax += 2f;
                EditorGUI.DrawRect(bg, new Color(1f, 0.3f, 0.3f, 0.18f));
            }

            const float gap = 6f;
            float w = rect.width;

            float idW = Mathf.Clamp(w * 0.62f, 140f, 260f);
            float baseW = Mathf.Max(80f, w - idW - gap);

            var idRect = new Rect(rect.x, rect.y, idW, rect.height);
            var baseRect = new Rect(idRect.xMax + gap, rect.y, baseW, rect.height);

            // Id (no label, but element label is StatId)
            EditorGUI.PropertyField(idRect, idProp, GUIContent.none);

            // DefaultBase label + value
            DrawFloatWithLabel(baseRect, "Default", baseProp);

            // Left-side label (foldout-style title replacement)
            // ReorderableList doesn't use element labels by default, so we draw a small caption ourselves.
            var captionRect = rect;
            captionRect.width = 120f;
            captionRect.x -= 122f; // put it slightly left if there is space (optional)
            // In most inspectors it will clip; so we instead show it as tooltip on Id dropdown.
            var tooltip = isDup ? "Duplicate StatId" : string.Empty;

            if (isDup)
            {
                // Right-side warning
                var warnRect = baseRect;
                warnRect.xMin = baseRect.xMax - 90f;
                EditorGUI.LabelField(warnRect, "DUPLICATE", GetDupStyle());
            }
        };

        _list.onAddDropdownCallback = (buttonRect, list) =>
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Empty"), false, () =>
            {
                AddEmpty();
            });

            menu.AddItem(new GUIContent("Add Missing StatIds"), false, () =>
            {
                AddMissing();
            });

            menu.ShowAsContext();
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_statsProp == null)
        {
            EditorGUILayout.HelpBox($"Could not find backing field for Stats. Expected '{StatsPropName}'.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawToolbar();

        var duplicates = CollectDuplicates();
        if (duplicates.Count > 0)
        {
            EditorGUILayout.HelpBox(
                "Duplicate StatId detected:\n- " + string.Join("\n- ", duplicates),
                MessageType.Error);
        }

        _list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    // ---------------- Toolbar ----------------

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Sort by StatId", GUILayout.Height(22)))
                SortById();

            if (GUILayout.Button("Remove Duplicates (keep first)", GUILayout.Height(22)))
                RemoveDuplicatesKeepFirst();
        }
        EditorGUILayout.Space(4);
    }

    // ---------------- Actions ----------------

    private void AddEmpty()
    {
        int i = _statsProp.arraySize;
        _statsProp.InsertArrayElementAtIndex(i);

        // Ensure we don't clone previous element values (Unity sometimes duplicates)
        var el = _statsProp.GetArrayElementAtIndex(i);
        var idProp = el.FindPropertyRelative(IdPropName);
        var baseProp = el.FindPropertyRelative(DefaultBasePropName);

        if (idProp != null && idProp.propertyType == SerializedPropertyType.Enum)
            idProp.enumValueIndex = 0;

        if (baseProp != null && baseProp.propertyType == SerializedPropertyType.Float)
            baseProp.floatValue = 0f;

        serializedObject.ApplyModifiedProperties();
    }

    private void AddMissing()
    {
        // Determine which StatIds already exist (by enum name)
        var existing = new HashSet<string>();

        for (int i = 0; i < _statsProp.arraySize; i++)
        {
            var el = _statsProp.GetArrayElementAtIndex(i);
            var idProp = el.FindPropertyRelative(IdPropName);
            existing.Add(GetEnumName(idProp));
        }

        // Add ones that are missing
        var values = Enum.GetValues(typeof(StatId));
        foreach (var v in values)
        {
            string name = v.ToString();
            if (existing.Contains(name)) continue;

            int i = _statsProp.arraySize;
            _statsProp.InsertArrayElementAtIndex(i);

            var el = _statsProp.GetArrayElementAtIndex(i);
            var idProp = el.FindPropertyRelative(IdPropName);
            var baseProp = el.FindPropertyRelative(DefaultBasePropName);

            if (idProp != null && idProp.propertyType == SerializedPropertyType.Enum)
            {
                int idx = Array.IndexOf(idProp.enumNames, name);
                if (idx >= 0) idProp.enumValueIndex = idx;
            }

            if (baseProp != null && baseProp.propertyType == SerializedPropertyType.Float)
                baseProp.floatValue = 0f;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SortById()
    {
        // Stable bubble sort by enumValueIndex
        int n = _statsProp.arraySize;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                var a = GetIdIndex(_statsProp.GetArrayElementAtIndex(j));
                var b = GetIdIndex(_statsProp.GetArrayElementAtIndex(j + 1));
                if (a > b)
                    _statsProp.MoveArrayElement(j + 1, j);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveDuplicatesKeepFirst()
    {
        var seen = new HashSet<string>();

        for (int i = _statsProp.arraySize - 1; i >= 0; i--)
        {
            var el = _statsProp.GetArrayElementAtIndex(i);
            var idProp = el.FindPropertyRelative(IdPropName);
            string key = GetEnumName(idProp);
            if (string.IsNullOrEmpty(key)) continue;

            if (!seen.Add(key))
                DeleteArrayElementSafely(_statsProp, i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    // ---------------- Duplicate checks ----------------

    private bool IsDuplicate(int index, SerializedProperty idProp)
    {
        string key = GetEnumName(idProp);
        if (string.IsNullOrEmpty(key)) return false;

        for (int i = 0; i < _statsProp.arraySize; i++)
        {
            if (i == index) continue;

            var other = _statsProp.GetArrayElementAtIndex(i).FindPropertyRelative(IdPropName);
            if (GetEnumName(other) == key)
                return true;
        }

        return false;
    }

    private List<string> CollectDuplicates()
    {
        var first = new Dictionary<string, int>();
        var res = new List<string>();

        for (int i = 0; i < _statsProp.arraySize; i++)
        {
            var el = _statsProp.GetArrayElementAtIndex(i);
            var idProp = el.FindPropertyRelative(IdPropName);

            string key = GetEnumName(idProp);
            if (string.IsNullOrEmpty(key)) continue;

            if (first.TryGetValue(key, out int firstIndex))
            {
                res.Add($"{GetEnumDisplayName(idProp)} (at {firstIndex} and {i})");
            }
            else
            {
                first[key] = i;
            }
        }

        return res;
    }

    // ---------------- Utility ----------------

    private static int GetIdIndex(SerializedProperty element)
    {
        var idProp = element.FindPropertyRelative(IdPropName);
        if (idProp == null || idProp.propertyType != SerializedPropertyType.Enum) return int.MaxValue;

        return Mathf.Clamp(idProp.enumValueIndex, 0, Math.Max(0, idProp.enumNames.Length - 1));
    }

    private static string GetEnumName(SerializedProperty enumProp)
    {
        if (enumProp == null || enumProp.propertyType != SerializedPropertyType.Enum) return string.Empty;
        int idx = Mathf.Clamp(enumProp.enumValueIndex, 0, Math.Max(0, enumProp.enumNames.Length - 1));
        return enumProp.enumNames.Length > 0 ? enumProp.enumNames[idx] : string.Empty;
    }

    private static string GetEnumDisplayName(SerializedProperty enumProp)
    {
        if (enumProp == null || enumProp.propertyType != SerializedPropertyType.Enum) return "Stat";
        int idx = Mathf.Clamp(enumProp.enumValueIndex, 0, Math.Max(0, enumProp.enumDisplayNames.Length - 1));
        return enumProp.enumDisplayNames.Length > 0 ? enumProp.enumDisplayNames[idx] : "Stat";
    }

    private static void DeleteArrayElementSafely(SerializedProperty arrayProp, int index)
    {
        // Unity quirk: for reference/managed types, first delete can null, second delete removes slot
        arrayProp.DeleteArrayElementAtIndex(index);

        if (index < arrayProp.arraySize)
        {
            var el = arrayProp.GetArrayElementAtIndex(index);

            bool stillEmpty =
                (el.propertyType == SerializedPropertyType.ObjectReference && el.objectReferenceValue == null) ||
                (el.propertyType == SerializedPropertyType.ManagedReference && string.IsNullOrEmpty(el.managedReferenceFullTypename));

            if (stillEmpty)
                arrayProp.DeleteArrayElementAtIndex(index);
        }
    }

    private static void DrawFloatWithLabel(Rect rect, string label, SerializedProperty floatProp)
    {
        const float labelW = 52f;
        const float gap = 4f;

        var labelRect = new Rect(rect.x, rect.y, labelW, rect.height);
        var fieldRect = new Rect(labelRect.xMax + gap, rect.y, Mathf.Max(30f, rect.width - labelW - gap), rect.height);

        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(fieldRect, floatProp, GUIContent.none);
    }

    private static GUIStyle GetDupStyle()
    {
        var s = new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Bold
        };
        s.normal.textColor = new Color(0.85f, 0.2f, 0.2f);
        return s;
    }
}
