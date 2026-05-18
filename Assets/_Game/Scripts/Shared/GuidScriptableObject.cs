using System;
using Assets._Game.Scripts.Shared.Attributes;
using UnityEditor;
using UnityEngine;

namespace Assets._Game.Scripts.Shared
{
    public abstract class GuidScriptableObject : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private string _id;

        public string Id => _id;

        public void RegenerateId()
        {
            _id = Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                RegenerateId();
            }
        }

        protected virtual void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                RegenerateId();
            }
        }
    }
}
