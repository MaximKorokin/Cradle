using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public sealed class DragDropUISystem : UISystemBase
    {
        // todo
        private const int StartDragThreshold = 30;

        private DragDropHandler _dragDropHandler;
        private DragDropView _dragDropView;

        private IDragDropSource _currentDragDropSource;
        private IDragDropTarget _dragDropTargetCandidate;

        [Inject]
        private void Construct(
            IGlobalEventBus globalEventBus,
            DragDropHandler dragDropHandler,
            DragDropView dragDropView)
        {
            BaseConstruct(globalEventBus);

            _dragDropHandler = dragDropHandler;
            _dragDropView = dragDropView;

            TrackGlobalEvent<PointerDownEvent>(OnPointerDown);
            TrackGlobalEvent<PointerUpEvent>(OnPointerUp);
            TrackGlobalEvent<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (e.Context.IsOverUI && TryResolveDragDropSource(e.Context.UnderlyingElement, out var source))
            {
                var visual = source.CreateDragDropVisual();
                _dragDropView.AttachVisual(visual);
                _currentDragDropSource = source;
            }
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            if (_currentDragDropSource == null) return;

            _dragDropHandler.Handle(_currentDragDropSource, _dragDropTargetCandidate);

            _dragDropView.ClearVisual();
            _currentDragDropSource = null;
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (_dragDropView.IsActive)
            {
                _dragDropView.transform.position = e.Context.ScreenPosition;
            }

            if (e.Context.IsOverUI && e.Context.IsPressed && TryResolveDragDropTarget(e.Context.UnderlyingElement, out var target))
            {
                if (_dragDropTargetCandidate != null && _dragDropTargetCandidate != target)
                {
                    target.SetDragDropHighlight(false);
                }

                _dragDropTargetCandidate = target;
                _dragDropTargetCandidate?.SetDragDropHighlight(true);
                SLog.Log(_dragDropTargetCandidate);
            }
            else
            {
                _dragDropTargetCandidate?.SetDragDropHighlight(false);
                _dragDropTargetCandidate = null;
            }
        }

        private bool TryResolveDragDropSource(GameObject gameObject, out IDragDropSource dragDropSource)
        {
            dragDropSource = gameObject.GetComponentInParent<IDragDropSource>();
            return dragDropSource != null;
        }

        private bool TryResolveDragDropTarget(GameObject gameObject, out IDragDropTarget dragDropTarget)
        {
            dragDropTarget = gameObject.GetComponentInParent<IDragDropTarget>();
            return dragDropTarget != null;
        }
    }
}
