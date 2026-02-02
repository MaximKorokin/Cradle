using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public abstract class UIWindow : MonoBehaviour
    {
        public virtual void OnShow() { }
        public virtual void OnHide() { }
        public virtual void Render() { }
    }
}
