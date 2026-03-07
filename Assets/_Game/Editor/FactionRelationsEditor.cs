using Assets._Game.Scripts.Entities.Faction;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactionRelations))]
public class FactionRelationsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var data = (FactionRelations)target;

        DrawDefaultInspector();

        data.Resize();

        int n = data.Count;

        if (n == 0)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Relations", EditorStyles.boldLabel);

        DrawHeader(data, n);

        for (int y = 0; y < n; y++)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(data.Factions[y].name, GUILayout.Width(100));

            for (int x = 0; x < n; x++)
            {
                if (x <= y)
                {
                    GUILayout.Label("", GUILayout.Width(80));
                    continue;
                }

                var r = data.Get(x, y);

                var newR = (FactionRelation)EditorGUILayout.EnumPopup(
                    r,
                    GUILayout.Width(80));

                if (newR != r)
                {
                    data.Set(x, y, newR);
                    EditorUtility.SetDirty(data);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawHeader(FactionRelations data, int n)
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("", GUILayout.Width(100));

        for (int i = 0; i < n; i++)
            GUILayout.Label(data.Factions[i].name, GUILayout.Width(80));

        EditorGUILayout.EndHorizontal();
    }
}
