using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class UIViewBase : MonoBehaviour
    {
        private IGlobalEventBus _globalEventBus;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
            _globalEventBus.Publish(new UIViewCreatedEvent(this));
        }

        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            _globalEventBus?.Publish(new UIViewDestroyedEvent(this));
        }

        public abstract bool TickRender();
    }

    public abstract class UIViewBase<TData> : UIViewBase where TData : IDataAggregator
    {
        private TData _data;
        private bool _isDirty;

        public TData Data => _data;

        public void RequestRender(TData data)
        {
            _data = data;
            _isDirty = true;
        }

        public override bool TickRender()
        {
            if (!_isDirty) return false;

            _isDirty = false;
            Render(_data);

            return true;
        }

        protected abstract void Render(TData data);

        /// <summary>
        /// Called when the view's data is unbound. Override to release any cached
        /// references to the data so they are not held onto after unbinding.
        /// </summary>
        public virtual void Unbind()
        {
            _data = default;
        }
    }
}
