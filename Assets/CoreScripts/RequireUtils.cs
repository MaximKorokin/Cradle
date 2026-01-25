using UnityEngine;

namespace Assets.CoreScripts
{
    public static class RequireUtils
    {
        public static T GetRequiredComponent<T>(this GameObject behaviour) where T : Component
        {
            if (!behaviour.TryGetComponent<T>(out var component))
            {
                SLog.Error($"GameObject {behaviour.name} doesn't contain required component {typeof(T)}");
            }
            return component;
        }

        public static T GetRequiredComponent<T>(this Component behaviour) where T : Component
        {
            if (!behaviour.TryGetComponent<T>(out var component))
            {
                SLog.Error($"Component {behaviour.name} doesn't contain required component {typeof(T)}");
            }
            return component;
        }

        public static T GetRequiredComponentInChildren<T>(this GameObject behaviour) where T : Component
        {
            var component = behaviour.GetComponentInChildren<T>();
            if (component == null)
            {
                SLog.Error($"GameObject {behaviour.name} doesn't contain required component {typeof(T)} in children");
            }
            return component;
        }

        public static T GetRequiredComponentInParent<T>(this GameObject behaviour) where T : Component
        {
            var component = behaviour.GetComponentInParent<T>();
            if (component == null)
            {
                SLog.Error($"GameObject {behaviour.name} doesn't contain required component {typeof(T)} in parents");
            }
            return component;
        }

        public static T GetRequiredComponentOrInChildren<T>(this GameObject behaviour) where T : Component
        {
            var component = behaviour.GetComponentInChildren<T>();
            if (component == null && !behaviour.TryGetComponent(out component))
            {
                SLog.Error($"GameObject {behaviour.name} doesn't contain required component {typeof(T)} on itself or in children");
            }
            return component;
        }

        public static TTarget CastRequired<TSource, TTarget>(TSource sourceOject)
        {
            if (sourceOject is TTarget targetObject)
            {
                return targetObject;
            }
            SLog.Error($"Object {sourceOject} of type {typeof(TSource)} is not of type {typeof(TTarget)}");
            return default;
        }
    }
}