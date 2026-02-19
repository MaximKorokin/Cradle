using Assets._Game.Scripts.Shared.Attributes;
using UnityEditor;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Storage
{
    public abstract class GuidScriptableObject : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private string _id;

        public string Id => _id;

        public void RegenerateId()
        {
            _id = System.Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);

            // save only if real asset
            var path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.SaveAssets();
            }
#endif
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                RegenerateId();
            }
        }
    }
}