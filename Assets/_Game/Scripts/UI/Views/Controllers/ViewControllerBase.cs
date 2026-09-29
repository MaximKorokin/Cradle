using Assets._Game.Scripts.UI.DataAggregators;
using System;

namespace Assets._Game.Scripts.UI.Views.Controllers
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

    /// <summary>
    /// Base class for view controllers that bind to a single <typeparamref name="TData"/> data aggregator
    /// and re-render whenever it changes.
    /// </summary>
    public abstract class ViewControllerBase<TView, TData> : ViewControllerBase<TView>
        where TView : UIViewBase<TData>
        where TData : IDataAggregator
    {
        protected TData Data { get; private set; }

        public virtual void Bind(TData data)
        {
            Data = data;
            Data.Changed += OnDataChanged;
        }

        public virtual void Unbind()
        {
            if (Data == null) return;

            Data.Changed -= OnDataChanged;
            Data = default;

            View.Unbind();
        }

        protected override void OnRender()
        {
            View.RequestRender(Data);
        }

        private void OnDataChanged()
        {
            Render();
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }
    }
}
