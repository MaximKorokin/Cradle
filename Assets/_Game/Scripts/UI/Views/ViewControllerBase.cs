using System;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class ViewControllerBase<T> : IDisposable
    {
        public T View { get; private set; }

        public virtual void Initialize(T view)
        {
            View = view;
        }

        public virtual void Dispose()
        {
            View = default;
        }
    }
}
