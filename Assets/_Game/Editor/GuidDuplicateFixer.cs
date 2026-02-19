#if UNITY_EDITOR
using Assets._Game.Scripts.Infrastructure.Storage;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GuidDuplicateFixer
{
    [MenuItem("Tools/GUID/Fix Duplicate IDs")]
    public static void FixDuplicates()
    {
        var guids = AssetDatabase.FindAssets($"t:{nameof(GuidScriptableObject)}");

        var assets = guids
            .Select(guid =>
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GuidScriptableObject>(path);
                return new
                {
                    Asset = asset,
                    Path = path,
                    CreationTime = File.GetCreationTimeUtc(path)
                };
            })
            .Where(x => x.Asset != null)
            .ToList();

        var groups = assets
            .GroupBy(x => x.Asset.Id)
            .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1);

        int fixedCount = 0;

        foreach (var group in groups)
        {
            // сортируем по дате создания (старые сверху)
            var ordered = group.OrderBy(x => x.CreationTime).ToList();

            // первый — оставляем
            var keeper = ordered.First();

            Debug.Log($"Duplicate GUID '{group.Key}' found. Keeping: {keeper.Path}");

            // остальные — перегенерируем
            foreach (var duplicate in ordered.Skip(1))
            {
                Undo.RecordObject(duplicate.Asset, "Regenerate Duplicate GUID");
                duplicate.Asset.RegenerateId();
                EditorUtility.SetDirty(duplicate.Asset);

                Debug.LogWarning(
                    $"Regenerated GUID for: {duplicate.Path}"
                );

                fixedCount++;
            }
        }

        if (fixedCount > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Duplicate GUID fix complete. Regenerated: {fixedCount}");
    }
}
#endif
