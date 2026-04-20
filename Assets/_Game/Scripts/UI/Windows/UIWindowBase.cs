using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class UIWindowBase : MonoBehaviour
    {
        private bool _isCleanedUp;

        public virtual bool IsModal => true;
        public virtual bool IsSingleton => true;

        public virtual void OnShow() { }
        public virtual void OnHide() => _isCleanedUp = true;

        protected virtual void OnDestroy()
        {
            if (!_isCleanedUp) OnHide();
        }
    }
}
