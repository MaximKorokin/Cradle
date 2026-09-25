using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.Infrastructure.Game;
using System.Collections.Generic;
using VContainer;
using Assets._Game.Scripts.Infrastructure.Systems;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class UIViewRenderUISystem : UISystemBase, ILateTickSystem
    {
        private readonly HashSet<UIViewBase> _views = new();

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            BaseConstruct(globalEventBus);

            TrackGlobalEvent<UIViewCreatedEvent>(OnViewCreated);
            TrackGlobalEvent<UIViewDestroyedEvent>(OnViewDestroyed);

            foreach (var view in UnityEngine.Object.FindObjectsByType<UIViewBase>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None))
            {
                _views.Add(view);
            }
        }

        public void LateTick(float delta)
        {
            var views = new List<UIViewBase>(_views);
            foreach (var view in views)
            {
                //if (view != null && view.isActiveAndEnabled)
                if (view != null)
                {
                    view.TickRender();
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _views.Clear();
        }

        private void OnViewCreated(UIViewCreatedEvent e)
        {
            _views.Add(e.View);
        }

        private void OnViewDestroyed(UIViewDestroyedEvent e)
        {
            _views.Remove(e.View);
        }
    }

    public readonly struct UIViewCreatedEvent : IGlobalEvent
    {
        public readonly UIViewBase View;

        public UIViewCreatedEvent(UIViewBase view) => View = view;
    }

    public readonly struct UIViewDestroyedEvent : IGlobalEvent
    {
        public readonly UIViewBase View;

        public UIViewDestroyedEvent(UIViewBase view) => View = view;
    }
}
