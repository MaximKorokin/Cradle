using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.UI.Systems;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public abstract class UIViewBase : MonoBehaviour
    {
        private bool _isCleanedUp;
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

        public virtual void OnShow() => _isCleanedUp = false;

        public virtual void OnHide() => _isCleanedUp = true;

        protected virtual void OnDestroy()
        {
            _globalEventBus?.Publish(new UIViewDestroyedEvent(this));
            if (!_isCleanedUp) OnHide();
        }

        internal abstract void TickRender();
    }

    public abstract class UIViewBase<TData> : UIViewBase
    {
        private TData _data;
        private bool _isDirty;

        public void RequestRender(TData data)
        {
            _data = data;
            _isDirty = true;
        }

        internal override void TickRender()
        {
            if (!_isDirty) return;

            _isDirty = false;
            Render(_data);
        }

        public abstract void Render(TData data);
    }
}
