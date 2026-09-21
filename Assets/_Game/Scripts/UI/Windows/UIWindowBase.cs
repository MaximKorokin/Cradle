using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class UIWindowBase : MonoBehaviour
    {
        private bool _isCleanedUp;

        public virtual void OnShow() => _isCleanedUp = false;
        public virtual void OnHide() => _isCleanedUp = true;

        protected virtual void OnDestroy()
        {
            if (!_isCleanedUp) OnHide();
        }
    }
}
