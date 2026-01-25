using System;

namespace Assets.CoreScripts
{
    public class MonoBehaviourDispatcher : MonoBehaviourSingleton<MonoBehaviourDispatcher>
    {
        public static event Action OnUpdate;

        protected override void Update()
        {
            base.Update();
            OnUpdate?.Invoke();
        }
    }
}