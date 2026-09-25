using System;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ViewControllerBase<TView> : IDisposable
        where TView : UIViewBase
    {
        public TView View { get; private set; }

        public virtual void Initialize(TView view)
        {
            View = view;
        }

        public void Render() => OnRender();

        protected abstract void OnRender();

        public virtual void Dispose()
        {
            View = default;
        }
    }
}
