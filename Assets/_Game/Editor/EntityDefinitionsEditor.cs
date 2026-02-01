// Assets/Editor/EntityTableEditor_WithSlotsSummary.cs
//
// Обновление:
// + колонка Id (read-only)
// + валидация: пустой Prefab / дубликаты EntityId (подсветка + подсказка)
// + Open (ping + select)
// + Slots Summary
// + колонки генерятся без хардкода (кроме Id/EquipmentSlots, которые вынесены отдельно)

using Assets._Game.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public sealed class EntityDefinitionsEditor : EditorWindow
{
    [Serializable]
    private sealed class Row
    {
        public EntityDefinition entity;
        public bool selected = true;

        public SerializedObject so;
        public SerializedProperty idProp;              // "<Id>k__BackingField"
        public SerializedProperty entityIdProp;        // "<EntityId>k__BackingField"
        public SerializedProperty prefabProp;          // "<Prefab>k__BackingField" (или "Prefab" если не автопроп)
        public SerializedProperty equipmentSlotsProp;  // "<EquipmentSlots>k__BackingField"

        public string slotsSummary;
        public double nextSummaryRefresh;
    }

    private sealed class Column
    {
        public string path;
        public string label;
        public Type fieldType;
        public float width;
        public bool visible = true;
    }

    private const float SelectColWidth = 22f;
    private const float AssetColWidth = 240f;
    private const float OpenColWidth = 52f;
    private const float IdColWidth = 240f;
    private const float SummaryColWidth = 260f;

    private DefaultAsset _folder;
    private string _search = "";
    private bool _onlySelected;
    private bool _showColumns;
    private bool _showUtilityColumns = true;

    private Vector2 _scroll;
    private bool _loaded;

    private readonly List<Row> _rows = new();
    private readonly List<Column> _cols = new();

    // validation cache
    private Dictionary<string, int> _entityIdCounts = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Color WarnBg = new Color(1f, 0.90f, 0.45f, 1f);

    [MenuItem("Tools/Entities/Table Editor (Slots Summary)")]
    private static void Open() => GetWindow<EntityDefinitionsEditor>("Entities (Table)");

    private void OnEnable() => Undo.undoRedoPerformed += Repaint;
    private void OnDisable() => Undo.undoRedoPerformed -= Repaint;

    private void OnGUI()
    {
        DrawTopBar();

        if (!_loaded)
        {
            EditorGUILayout.HelpBox("Нажми Load, чтобы загрузить EntityDefinition в таблицу.", MessageType.Info);
            return;
        }

        DrawColumnsPanel();
        DrawHeader();
        DrawRows();
        DrawBottomBar();
    }

    private void DrawTopBar()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _folder = (DefaultAsset)EditorGUILayout.ObjectField("Folder", _folder, typeof(DefaultAsset), false);
                _search = EditorGUILayout.TextField("Search", _search);

                if (GUILayout.Button("Load", GUILayout.Width(80)))
                    Load();

                using (new EditorGUI.DisabledScope(_rows.Count == 0))
                {
                    if (GUILayout.Button("Select All", GUILayout.Width(90)))
                        SetSelection(true);
                    if (GUILayout.Button("None", GUILayout.Width(60)))
                        SetSelection(false);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _onlySelected = EditorGUILayout.ToggleLeft("Show only selected", _onlySelected, GUILayout.Width(160));
                _showColumns = EditorGUILayout.ToggleLeft("Columns", _showColumns, GUILayout.Width(90));
                _showUtilityColumns = EditorGUILayout.ToggleLeft("Utility", _showUtilityColumns, GUILayout.Width(90));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save Assets", GUILayout.Width(100)))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    private void DrawColumnsPanel()
    {
        if (!_showColumns) return;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Visible columns", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("All", GUILayout.Width(60)))
                    foreach (var c in _cols) c.visible = true;

                if (GUILayout.Button("None", GUILayout.Width(60)))
                    foreach (var c in _cols) c.visible = false;

                GUILayout.FlexibleSpace();
            }

            int perRow = Mathf.Max(1, Mathf.FloorToInt(position.width / 220f));
            int i = 0;

            while (i < _cols.Count)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int k = 0; k < perRow && i < _cols.Count; k++, i++)
                    {
                        var c = _cols[i];
                        c.visible = EditorGUILayout.ToggleLeft(c.label, c.visible, GUILayout.Width(210));
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("✓", GUILayout.Width(SelectColWidth));
            GUILayout.Label("Asset", GUILayout.Width(AssetColWidth));
            if (_showUtilityColumns)
            {
                GUILayout.Label("Open", GUILayout.Width(OpenColWidth));
                GUILayout.Label("Id", GUILayout.Width(IdColWidth));
                GUILayout.Label("Slots", GUILayout.Width(SummaryColWidth));
            }

            foreach (var c in _cols)
            {
                if (!c.visible) continue;
                GUILayout.Label(c.label, GUILayout.Width(GetColWidth(c)));
            }

            GUILayout.FlexibleSpace();
        }
    }

    private void DrawRows()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var r in _rows)
        {
            if (!PassFilter(r)) continue;

            if (r.so == null)
                r.so = new SerializedObject(r.entity);

            r.so.Update();

            // refresh props if needed
            if (r.idProp == null || r.idProp.serializedObject.targetObject != r.entity)
                r.idProp = r.so.FindProperty("<Id>k__BackingField");

            if (r.entityIdProp == null || r.entityIdProp.serializedObject.targetObject != r.entity)
                r.entityIdProp = FindStringProp(r.so, "<EntityId>k__BackingField", "EntityId");

            if (r.prefabProp == null || r.prefabProp.serializedObject.targetObject != r.entity)
                r.prefabProp = FindObjectProp(r.so, "<Prefab>k__BackingField", "Prefab");

            if (r.equipmentSlotsProp == null || r.equipmentSlotsProp.serializedObject.targetObject != r.entity)
                r.equipmentSlotsProp = FindEquipmentSlotsArray(r.so);

            // summary throttled
            if (EditorApplication.timeSinceStartup >= r.nextSummaryRefresh)
            {
                r.slotsSummary = BuildSlotsSummary(r.equipmentSlotsProp);
                r.nextSummaryRefresh = EditorApplication.timeSinceStartup + 0.25;
            }

            bool hasMissingPrefab = IsPrefabMissing(r.prefabProp);
            bool hasDuplicateEntityId = IsDuplicateEntityId(r.entityIdProp);

            // row highlight
            var prevBg = GUI.backgroundColor;
            if (hasMissingPrefab || hasDuplicateEntityId)
                GUI.backgroundColor = WarnBg;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.backgroundColor = prevBg;

                r.selected = EditorGUILayout.Toggle(r.selected, GUILayout.Width(SelectColWidth));

                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField(r.entity, typeof(EntityDefinition), false, GUILayout.Width(AssetColWidth));

                if (_showUtilityColumns)
                {
                    if (GUILayout.Button("Open", GUILayout.Width(OpenColWidth)))
                    {
                        Selection.activeObject = r.entity;
                        EditorGUIUtility.PingObject(r.entity);
                    }

                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(r.idProp, GUIContent.none, GUILayout.Width(IdColWidth));

                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.TextField(r.slotsSummary ?? "—", GUILayout.Width(SummaryColWidth));
                }

                // dynamic columns
                bool changed = false;

                foreach (var c in _cols)
                {
                    if (!c.visible) continue;

                    var p = r.so.FindProperty(c.path);
                    using (new EditorGUI.DisabledScope(p == null))
                    {
                        // per-cell highlight + tooltip for issues
                        var prev = GUI.backgroundColor;
                        if (p != null && p.propertyPath == r.entityIdProp?.propertyPath && hasDuplicateEntityId)
                            GUI.backgroundColor = WarnBg;
                        if (p != null && p.propertyPath == r.prefabProp?.propertyPath && hasMissingPrefab)
                            GUI.backgroundColor = WarnBg;

                        var label = GUIContent.none;
                        if (p != null && p.propertyPath == r.entityIdProp?.propertyPath && hasDuplicateEntityId)
                            label = new GUIContent("", "Duplicate EntityId");
                        if (p != null && p.propertyPath == r.prefabProp?.propertyPath && hasMissingPrefab)
                            label = new GUIContent("", "Prefab is missing");

                        EditorGUI.BeginChangeCheck();
                        if (p != null)
                            EditorGUILayout.PropertyField(p, label, true, GUILayout.Width(GetColWidth(c)));
                        else
                            EditorGUILayout.LabelField("-", GUILayout.Width(GetColWidth(c)));
                        if (EditorGUI.EndChangeCheck())
                            changed = true;

                        GUI.backgroundColor = prev;
                    }
                }

                if (changed)
                {
                    Undo.RecordObject(r.entity, "Edit EntityDefinition");
                    r.so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(r.entity);

                    // rebuild validation cache when ids/prefabs may change
                    RebuildValidationCache();

                    // refresh summary quickly
                    r.slotsSummary = BuildSlotsSummary(r.equipmentSlotsProp);
                    r.nextSummaryRefresh = EditorApplication.timeSinceStartup + 0.25;
                }

                GUILayout.FlexibleSpace();
            }

            // warnings under row (compact)
            if (hasMissingPrefab || hasDuplicateEntityId)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(SelectColWidth + AssetColWidth + OpenColWidth);
                    string msg = "";
                    if (hasMissingPrefab) msg += "Prefab missing. ";
                    if (hasDuplicateEntityId) msg += "Duplicate EntityId.";
                    EditorGUILayout.HelpBox(msg.Trim(), MessageType.Warning);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawBottomBar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            int total = _rows.Count;
            int shown = _rows.Count(PassFilter);
            int selected = _rows.Count(x => x.selected);

            int duplicates = _entityIdCounts.Values.Count(v => v > 1);
            int missingPrefabs = _rows.Count(r => IsPrefabMissing(r.prefabProp));

            GUILayout.Label($"Total: {total}   Shown: {shown}   Selected: {selected}");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Dup EntityId: {duplicates}   Missing Prefab: {missingPrefabs}");
        }
    }

    private bool PassFilter(Row r)
    {
        if (_onlySelected && !r.selected) return false;

        if (!string.IsNullOrWhiteSpace(_search))
        {
            var s = _search.Trim();
            string path = AssetDatabase.GetAssetPath(r.entity);

            bool ok =
                Contains(r.entity.name, s) ||
                Contains(path, s) ||
                Contains(r.slotsSummary ?? "", s) ||
                Contains(GetString(r.entityIdProp), s) ||
                Contains(GetString(r.idProp), s);

            if (!ok) return false;
        }

        return true;

        static bool Contains(string a, string b)
            => !string.IsNullOrEmpty(a) && a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void SetSelection(bool value)
    {
        foreach (var r in _rows)
            r.selected = value;
    }

    private void Load()
    {
        _rows.Clear();
        _cols.Clear();

        string[] guids;

        if (_folder != null)
        {
            string folderPath = AssetDatabase.GetAssetPath(_folder);
            guids = AssetDatabase.FindAssets("t:EntityDefinition", new[] { folderPath });
        }
        else
        {
            guids = AssetDatabase.FindAssets("t:EntityDefinition");
        }

        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var e = AssetDatabase.LoadAssetAtPath<EntityDefinition>(path);
            if (!e) continue;

            var so = new SerializedObject(e);

            var row = new Row
            {
                entity = e,
                selected = true,
                so = so,
                idProp = so.FindProperty("<Id>k__BackingField"),
                entityIdProp = FindStringProp(so, "<EntityId>k__BackingField", "EntityId"),
                prefabProp = FindObjectProp(so, "<Prefab>k__BackingField", "Prefab"),
                equipmentSlotsProp = FindEquipmentSlotsArray(so),
                slotsSummary = null,
                nextSummaryRefresh = 0
            };

            _rows.Add(row);
        }

        BuildColumnsFromEntityDefinition();
        RebuildValidationCache();

        _loaded = true;
        Repaint();
    }

    private void RebuildValidationCache()
    {
        _entityIdCounts.Clear();
        foreach (var r in _rows)
        {
            if (r.so == null) r.so = new SerializedObject(r.entity);
            r.so.Update();

            if (r.entityIdProp == null) r.entityIdProp = FindStringProp(r.so, "<EntityId>k__BackingField", "EntityId");

            var key = (GetString(r.entityIdProp) ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;

            _entityIdCounts.TryGetValue(key, out int c);
            _entityIdCounts[key] = c + 1;
        }
    }

    private bool IsDuplicateEntityId(SerializedProperty entityIdProp)
    {
        var key = (GetString(entityIdProp) ?? "").Trim();
        if (string.IsNullOrEmpty(key)) return false;
        return _entityIdCounts.TryGetValue(key, out var c) && c > 1;
    }

    private static bool IsPrefabMissing(SerializedProperty prefabProp)
    {
        if (prefabProp == null) return true;
        if (prefabProp.propertyType != SerializedPropertyType.ObjectReference) return true;
        return prefabProp.objectReferenceValue == null;
    }

    private void BuildColumnsFromEntityDefinition()
    {
        // колонки из сериализуемых полей:
        // исключаем: Id (показываем отдельной read-only колонкой), EquipmentSlots (summary), m_Script
        var t = typeof(EntityDefinition);
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        const string idBacking = "<Id>k__BackingField";
        const string equipBacking = "<EquipmentSlots>k__BackingField";

        var fields = t.GetFields(flags)
            .Where(f => !f.IsStatic)
            .Where(IsUnitySerializableField)
            .Where(f => f.Name != "m_Script")
            .Where(f => f.Name != idBacking)
            .Where(f => f.Name != equipBacking)
            .ToList();

        var probe = _rows.FirstOrDefault()?.so;
        if (probe == null) return;

        foreach (var f in fields)
        {
            var p = probe.FindProperty(f.Name);
            if (p == null) continue;

            _cols.Add(new Column
            {
                path = p.propertyPath,
                label = ObjectNames.NicifyVariableName(NiceNameFromBackingField(f.Name)),
                fieldType = f.FieldType,
                width = GuessWidth(f.FieldType),
                visible = true
            });
        }

        _cols.Sort((a, b) => string.CompareOrdinal(a.label, b.label));
    }

    // --------- helpers ---------

    private static string NiceNameFromBackingField(string name)
    {
        if (name.StartsWith("<", StringComparison.Ordinal))
        {
            int end = name.IndexOf('>');
            if (end > 1) return name.Substring(1, end - 1);
        }
        return name;
    }

    private static bool IsUnitySerializableField(FieldInfo f)
    {
        if (f.IsPublic) return true;
        return f.GetCustomAttributes(typeof(SerializeField), true).Length > 0;
    }

    private static float GuessWidth(Type t)
    {
        if (t == typeof(bool)) return 50;
        if (t == typeof(int) || t == typeof(float)) return 90;
        if (t != null && t.IsEnum) return 120;
        if (typeof(UnityEngine.Object).IsAssignableFrom(t)) return 170;
        return 170;
    }

    private static float GetColWidth(Column c) => Mathf.Clamp(c.width, 50, 320);

    private static SerializedProperty FindEquipmentSlotsArray(SerializedObject so)
    {
        var p = so.FindProperty("<EquipmentSlots>k__BackingField");
        if (p != null && p.isArray && p.propertyType != SerializedPropertyType.String)
            return p;

        // fallback scan
        var it = so.GetIterator();
        bool enterChildren = true;
        while (it.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (!it.isArray) continue;
            if (it.propertyType == SerializedPropertyType.String) continue;

            if (it.arraySize > 0)
            {
                var el = it.GetArrayElementAtIndex(0);
                if (el.propertyType == SerializedPropertyType.Enum)
                    return so.FindProperty(it.propertyPath);
            }
        }

        return null;
    }

    private static string BuildSlotsSummary(SerializedProperty slotsArray)
    {
        if (slotsArray == null) return "—";
        if (!slotsArray.isArray) return "—";
        if (slotsArray.arraySize == 0) return "—";

        var names = new List<string>(slotsArray.arraySize);
        for (int i = 0; i < slotsArray.arraySize; i++)
        {
            var el = slotsArray.GetArrayElementAtIndex(i);
            if (el.propertyType != SerializedPropertyType.Enum) continue;

            var display = el.enumDisplayNames != null && el.enumDisplayNames.Length > el.enumValueIndex
                ? el.enumDisplayNames[el.enumValueIndex]
                : el.enumNames != null && el.enumNames.Length > el.enumValueIndex
                    ? el.enumNames[el.enumValueIndex]
                    : el.enumValueIndex.ToString();

            names.Add(display);
        }

        return names.Count == 0 ? "—" : string.Join(", ", names);
    }

    private static SerializedProperty FindStringProp(SerializedObject so, string backing, string fallback)
    {
        var p = so.FindProperty(backing);
        if (p != null && p.propertyType == SerializedPropertyType.String) return p;

        p = so.FindProperty(fallback);
        if (p != null && p.propertyType == SerializedPropertyType.String) return p;

        return null;
    }

    private static SerializedProperty FindObjectProp(SerializedObject so, string backing, string fallback)
    {
        var p = so.FindProperty(backing);
        if (p != null && p.propertyType == SerializedPropertyType.ObjectReference) return p;

        p = so.FindProperty(fallback);
        if (p != null && p.propertyType == SerializedPropertyType.ObjectReference) return p;

        return null;
    }

    private static string GetString(SerializedProperty p)
    {
        if (p == null) return null;
        return p.propertyType == SerializedPropertyType.String ? p.stringValue : null;
    }
}
