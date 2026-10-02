using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class UIViewRenderUISystem : UISystemBase, ITickSystem, ILateTickSystem, IUIRenderTickSystem
    {
        private readonly HashSet<UIViewBase> _views = new();

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            BaseConstruct(globalEventBus);

            TrackGlobalEvent<UIViewCreatedEvent>(OnViewCreated);
            TrackGlobalEvent<UIViewDestroyedEvent>(OnViewDestroyed);

            foreach (var view in FindObjectsByType<UIViewBase>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None))
            {
                _views.Add(view);
            }
        }

        public void Tick(float delta)
        {
            TickRenderViews();
        }

        public void LateTick(float delta)
        {
            TickRenderViews();
        }

        public void UIRenderTick(float delta)
        {
            TickRenderViews();
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

        private void TickRenderViews()
        {
            var views = new List<UIViewBase>(_views);
            foreach (var view in views)
            {
                // Render the view and rebuild the layout after so that view has the correct size and position
                if (view != null &&
                    view.isActiveAndEnabled &&
                    view.gameObject.transform.parent != null && // prefab
                    view.TickRender())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(view.transform as RectTransform);
                }
            }
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
