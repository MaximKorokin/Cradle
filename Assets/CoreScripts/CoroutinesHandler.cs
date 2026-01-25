using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.CoreScripts
{
    public class CoroutinesHandler : MonoBehaviourSingleton<CoroutinesHandler>
    {
        private static readonly Dictionary<object, CoroutineWrapper> _uniqueCoroutines = new();

        public static CoroutineWrapper StartCoroutine(IEnumerator enumerator, Action finalAction = null)
        {
            var coroutine = Instance.StartCoroutineSafe(enumerator, finalAction);
            return coroutine;
        }

        public static CoroutineWrapper StartUniqueCoroutine(object obj, IEnumerator enumerator, Action finalAction = null)
        {
            if (_uniqueCoroutines.TryGetValue(obj, out var coroutine) && !coroutine.HasFinished)
            {
                Instance.StopCoroutine(coroutine);
            }

            coroutine = Instance.StartCoroutineSafe(enumerator, (() => _uniqueCoroutines.Remove(obj)) + finalAction);
            _uniqueCoroutines[obj] = coroutine;
            return coroutine;
        }
    }
}