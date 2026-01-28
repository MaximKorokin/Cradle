using Assets._Game.Scripts.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemTableEditor : EditorWindow
{
    [Serializable]
    class Row
    {
        public ItemDefinition item;
        public bool selected = true;   // только для фильтра "show only selected"
        public SerializedObject so;
        public SerializedProperty traits; // managed ref array prop (если найдём)
        public string traitsSummary;
        public double nextSummaryRefresh;
    }

    class Column
    {
        public string path;
        public string label;
        public Type fieldType;
        public float width;
        public bool visible = true;
    }

    Vector2 _scroll;
    DefaultAsset _folder;
    string _search = "";
    bool _onlySelected;
    bool _showColumns;

    readonly List<Row> _rows = new();
    readonly List<Column> _cols = new();

    bool _loaded;

    const float AssetColWidth = 220f;
    const float OpenColWidth = 52f;
    const float TraitsColWidth = 260f;

    [MenuItem("Tools/Items/Table Editor")]
    static void Open() => GetWindow<ItemTableEditor>("Items (Table)");

    void OnEnable() => Undo.undoRedoPerformed += Repaint;

    void OnDisable()
    {
        Undo.undoRedoPerformed -= Repaint;
        foreach (var r in _rows) r.so = null;
    }

    void OnGUI()
    {
        DrawTopBar();

        if (!_loaded)
        {
            EditorGUILayout.HelpBox("Нажми Load чтобы загрузить ItemDefinition в таблицу.", MessageType.Info);
            return;
        }

        if (_cols.Count == 0 && _rows.Count > 0)
            EditorGUILayout.HelpBox("Не нашёл сериализуемых полей в ItemDefinition (кроме Traits).", MessageType.None);

        DrawColumnsPanel();
        DrawHeader();
        DrawRows();
        DrawBottomBar();
    }

    void DrawTopBar()
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
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save Assets", GUILayout.Width(100)))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    void DrawColumnsPanel()
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

            int perRow = Mathf.Max(1, Mathf.FloorToInt(position.width / 120f));
            int i = 0;

            while (i < _cols.Count)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int k = 0; k < perRow && i < _cols.Count; k++, i++)
                    {
                        var c = _cols[i];
                        c.visible = EditorGUILayout.ToggleLeft(c.label, c.visible, GUILayout.Width(110));
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }

    void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("✓", GUILayout.Width(22));
            GUILayout.Label("Asset", GUILayout.Width(AssetColWidth));
            GUILayout.Label("Open", GUILayout.Width(OpenColWidth));
            GUILayout.Label("Traits", GUILayout.Width(TraitsColWidth));

            foreach (var c in _cols)
            {
                if (!c.visible) continue;
                GUILayout.Label(c.label, GUILayout.Width(GetColWidth(c)));
            }

            GUILayout.FlexibleSpace();
        }
    }

    void DrawRows()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var r in _rows)
        {
            if (!PassFilter(r)) continue;

            if (r.so == null)
                r.so = new SerializedObject(r.item);

            r.so.Update();

            // Обновляем ссылку на Traits prop (если её ещё нет или сломалась)
            if (r.traits == null || r.traits.serializedObject.targetObject != r.item)
                r.traits = FindTraitsManagedRefArray(r.so);

            // Обновляем summary раз в ~0.25 сек, чтобы не считать каждый OnGUI
            if (EditorApplication.timeSinceStartup >= r.nextSummaryRefresh)
            {
                r.traitsSummary = BuildTraitsSummary(r.traits);
                r.nextSummaryRefresh = EditorApplication.timeSinceStartup + 0.25;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                r.selected = EditorGUILayout.Toggle(r.selected, GUILayout.Width(22));

                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField(r.item, typeof(ItemDefinition), false, GUILayout.Width(AssetColWidth));

                if (GUILayout.Button("Open", GUILayout.Width(OpenColWidth)))
                {
                    Selection.activeObject = r.item;
                    EditorGUIUtility.PingObject(r.item);
                }

                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.TextField(r.traitsSummary ?? "—", GUILayout.Width(TraitsColWidth));

                bool rowChanged = false;

                foreach (var c in _cols)
                {
                    if (!c.visible) continue;

                    var p = r.so.FindProperty(c.path);
                    using (new EditorGUI.DisabledScope(p == null))
                    {
                        EditorGUI.BeginChangeCheck();
                        if (p != null)
                            EditorGUILayout.PropertyField(p, GUIContent.none, GUILayout.Width(GetColWidth(c)));
                        else
                            EditorGUILayout.LabelField("-", GUILayout.Width(GetColWidth(c)));
                        if (EditorGUI.EndChangeCheck())
                            rowChanged = true;
                    }
                }

                if (rowChanged)
                {
                    // Пишем сразу в ассет (как Inspector), Undo откатит.
                    Undo.RecordObject(r.item, "Edit ItemDefinition");
                    r.so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(r.item);

                    // traits summary тоже мог измениться (если например поменяли структуру)
                    r.traitsSummary = BuildTraitsSummary(r.traits);
                    r.nextSummaryRefresh = EditorApplication.timeSinceStartup + 0.25;
                }

                GUILayout.FlexibleSpace();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawBottomBar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            int total = _rows.Count;
            int shown = _rows.Count(PassFilter);
            int selected = _rows.Count(r => r.selected);

            GUILayout.Label($"Total: {total}   Shown: {shown}   Selected: {selected}");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Undo to revert changes (Ctrl+Z)");
        }
    }

    bool PassFilter(Row r)
    {
        if (_onlySelected && !r.selected) return false;

        if (!string.IsNullOrWhiteSpace(_search))
        {
            var s = _search.Trim();
            if (!Contains(r.item.name, s) && !Contains(AssetDatabase.GetAssetPath(r.item), s))
                return false;
        }

        return true;

        static bool Contains(string a, string b)
            => !string.IsNullOrEmpty(a) && a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    void Load()
    {
        foreach (var r in _rows) r.so = null;
        _rows.Clear();
        _cols.Clear();

        string[] guids;
        if (_folder != null)
        {
            var folderPath = AssetDatabase.GetAssetPath(_folder);
            guids = AssetDatabase.FindAssets("t:ItemDefinition", new[] { folderPath });
        }
        else
        {
            guids = AssetDatabase.FindAssets("t:ItemDefinition");
        }

        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path);
            if (!item) continue;

            var so = new SerializedObject(item);

            _rows.Add(new Row
            {
                item = item,
                selected = true,
                so = so,
                traits = FindTraitsManagedRefArray(so),
                traitsSummary = null,
                nextSummaryRefresh = 0
            });
        }

        BuildColumnsFromItemDefinition();
        _loaded = true;
        Repaint();
    }

    void BuildColumnsFromItemDefinition()
    {
        // генерируем колонки из сериализуемых полей (как раньше),
        // но исключаем Traits, чтобы не рисовать его как массив в таблице (у нас Summary)
        var t = typeof(ItemDefinition);
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var fields = t.GetFields(flags)
            .Where(f => !f.IsStatic)
            .Where(IsUnitySerializableField)
            .Where(f => f.Name != "m_Script")
            .ToList();

        SerializedObject probe = _rows.FirstOrDefault()?.so;
        if (probe == null) return;

        foreach (var f in fields)
        {
            var p = probe.FindProperty(f.Name);
            if (p == null) continue;

            // Если это managed reference array (Traits) — пропускаем
            if (IsManagedRefArray(p)) continue;

            _cols.Add(new Column
            {
                path = p.propertyPath,
                label = ObjectNames.NicifyVariableName(f.Name),
                fieldType = f.FieldType,
                width = GuessWidth(f.FieldType),
                visible = true
            });
        }

        _cols.Sort((a, b) => string.CompareOrdinal(a.label, b.label));
    }

    static bool IsUnitySerializableField(FieldInfo f)
    {
        if (f.IsPublic) return true;
        return f.GetCustomAttributes(typeof(SerializeField), true).Length > 0;
    }

    static float GuessWidth(Type t)
    {
        if (t == typeof(bool)) return 45;
        if (t == typeof(int) || t == typeof(float)) return 80;
        if (t != null && t.IsEnum) return 110;
        if (typeof(UnityEngine.Object).IsAssignableFrom(t)) return 160;
        return 160;
    }

    float GetColWidth(Column c) => Mathf.Clamp(c.width, 45, 260);

    void SetSelection(bool value)
    {
        foreach (var r in _rows)
            r.selected = value;
    }

    // ---- Traits summary helpers ----

    static SerializedProperty FindTraitsManagedRefArray(SerializedObject so)
    {
        // Ищем первый массив, элементы которого ManagedReference.
        // Если Traits пустой — Unity не даёт надёжно определить тип элементов без рефлексии по backing field.
        // Но обычно Traits не пустой в контенте. Если пустой — summary будет "—".
        var it = so.GetIterator();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (!it.isArray) continue;
            if (it.propertyType == SerializedPropertyType.String) continue;

            if (it.arraySize <= 0) continue;

            var el = it.GetArrayElementAtIndex(0);
            if (el.propertyType == SerializedPropertyType.ManagedReference)
                return so.FindProperty(it.propertyPath);
        }

        return null;
    }

    static bool IsManagedRefArray(SerializedProperty p)
    {
        if (!p.isArray) return false;
        if (p.propertyType == SerializedPropertyType.String) return false;
        if (p.arraySize <= 0) return false;
        var el = p.GetArrayElementAtIndex(0);
        return el.propertyType == SerializedPropertyType.ManagedReference;
    }

    static string BuildTraitsSummary(SerializedProperty traitsArray)
    {
        if (traitsArray == null) return "—";
        if (!traitsArray.isArray) return "—";
        if (traitsArray.arraySize == 0) return "—";

        // Убираем дубликаты, сохраняем порядок появления
        var set = new HashSet<string>();
        var list = new List<string>();

        for (int i = 0; i < traitsArray.arraySize; i++)
        {
            var el = traitsArray.GetArrayElementAtIndex(i);
            string name = GetManagedTypeShortName(el);
            if (string.IsNullOrWhiteSpace(name) || name == "(null)") continue;

            if (set.Add(name))
                list.Add(name);
        }

        return list.Count == 0 ? "—" : string.Join(", ", list);
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
}
