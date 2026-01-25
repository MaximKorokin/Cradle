using UnityEngine;

namespace Assets.CoreScripts
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviourBase where T : MonoBehaviourSingleton<T>
    {
        private static T _instance;
        public static T Instance => GetOrCreateInstance();

        protected override void Awake()
        {
            base.Awake();
            _instance = (T)this;
        }

        private static T GetOrCreateInstance()
        {
            if (_instance == null)
            {
                var obj = new GameObject();
                _instance = obj.AddComponent<T>();
            }
            return _instance;
        }
    }
}