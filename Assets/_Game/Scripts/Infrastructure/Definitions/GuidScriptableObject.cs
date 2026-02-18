using Assets._Game.Scripts.Shared.Attributes;
using UnityEditor;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Definitions
{
    public abstract class GuidScriptableObject : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private string id;

        public string Id => id;

        public void RegenerateId()
        {
            id = System.Guid.NewGuid().ToString("N");
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(id))
            {
                id = System.Guid.NewGuid().ToString("N");
                EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}