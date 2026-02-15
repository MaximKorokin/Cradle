using Assets._Game.Scripts.Shared.Attributes;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class GuidScriptableObject : ScriptableObject
{
    [SerializeField, ReadOnly]
    private string id;

    public string Id => id;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = System.Guid.NewGuid().ToString("N");
            EditorUtility.SetDirty(this);
        }

        var type = GetType();
        var guids = AssetDatabase.FindAssets($"t:{type.Name}");

        var duplicates = guids
            .Select(g => AssetDatabase.LoadAssetAtPath<GuidScriptableObject>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(so => so != null && so != this && so.Id == Id)
            .ToList();

        if (duplicates.Count > 0)
        {
            id = System.Guid.NewGuid().ToString("N");
            EditorUtility.SetDirty(this);
            Debug.LogWarning($"Duplicate GUID detected in {name}, regenerated.");
        }
    }
#endif
}
