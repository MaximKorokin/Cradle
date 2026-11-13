
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ProjectGuideWindow : EditorWindow
{
    private const string DefaultPath = "Assets/ProjectGuide.asset";

    private ProjectGuide _guide;

    [MenuItem("Tools/Project Guide")]
    private static void Open()
    {
        GetWindow<ProjectGuideWindow>("Project Guide");
    }

    private void OnEnable()
    {
        LoadGuide();
    }

    private void OnFocus()
    {
        if (_guide == null)
            LoadGuide();
    }

    private void OnProjectChange()
    {
        if (_guide == null)
            LoadGuide();

        Repaint();
    }

    private void LoadGuide()
    {
        var guids = AssetDatabase.FindAssets("t:ProjectGuide");

        if (guids.Length == 0)
        {
            _guide = null;
            return;
        }

        // Prefer the default asset location.
        var paths = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(path => path == DefaultPath ? 0 : 1)
            .ToArray();

        _guide = AssetDatabase.LoadAssetAtPath<ProjectGuide>(paths[0]);
    }

    private void CreateGuide()
    {
        // Never create another guide if one already exists.
        LoadGuide();

        if (_guide != null)
        {
            Selection.activeObject = _guide;
            return;
        }

        var path = AssetDatabase.GenerateUniqueAssetPath(DefaultPath);

        _guide = CreateInstance<ProjectGuide>();

        AssetDatabase.CreateAsset(_guide, path);
        AssetDatabase.SaveAssets();

        Selection.activeObject = _guide;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        if (_guide == null)
        {
            EditorGUILayout.HelpBox(
                "Project Guide not found.",
                MessageType.Info);

            if (GUILayout.Button("Create Guide"))
                CreateGuide();

            return;
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            "UI Colors",
            EditorStyles.boldLabel);

        if (GUILayout.Button("Select Asset", GUILayout.Width(90)))
            Selection.activeObject = _guide;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        for (int i = 0; i < _guide.Colors.Count; i++)
        {
            var entry = _guide.Colors[i];

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            var name = EditorGUILayout.TextField(
                entry.Name,
                GUILayout.MinWidth(80));

            var color = EditorGUILayout.ColorField(
                entry.Value,
                GUILayout.Width(55));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_guide, "Edit Color");

                entry.Name = name;
                entry.Value = color;

                EditorUtility.SetDirty(_guide);
            }

            EditorGUILayout.SelectableLabel(
                entry.Hex,
                GUILayout.Width(75),
                GUILayout.Height(EditorGUIUtility.singleLineHeight));

            if (GUILayout.Button("Copy", GUILayout.Width(45)))
                GUIUtility.systemCopyBuffer = entry.Hex;

            GUI.enabled = i > 0;

            if (GUILayout.Button("↑", GUILayout.Width(25)))
            {
                MoveColor(i, i - 1);
                GUIUtility.ExitGUI();
            }

            GUI.enabled = i < _guide.Colors.Count - 1;

            if (GUILayout.Button("↓", GUILayout.Width(25)))
            {
                MoveColor(i, i + 1);
                GUIUtility.ExitGUI();
            }

            GUI.enabled = true;

            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                Undo.RecordObject(_guide, "Remove Color");

                _guide.Colors.RemoveAt(i);

                EditorUtility.SetDirty(_guide);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Color"))
        {
            Undo.RecordObject(_guide, "Add Color");

            _guide.Colors.Add(
                new ProjectGuide.ColorEntry
                {
                    Name = "New Color"
                });

            EditorUtility.SetDirty(_guide);
        }
    }

    private void MoveColor(int from, int to)
    {
        Undo.RecordObject(_guide, "Move Color");

        var entry = _guide.Colors[from];

        _guide.Colors.RemoveAt(from);
        _guide.Colors.Insert(to, entry);

        EditorUtility.SetDirty(_guide);
    }
}
