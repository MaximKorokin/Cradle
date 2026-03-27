using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class UIWindowBase : MonoBehaviour
    {
        public virtual bool IsModal => true;
        public virtual bool IsSingleton => true;

        public virtual void OnShow() { }
        public virtual void OnHide() { }
    }
}
