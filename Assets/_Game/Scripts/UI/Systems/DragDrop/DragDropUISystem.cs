using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;
using VContainer;

namespace Assets._Game.Scripts.UI.Systems.DragDrop
{
    public sealed class DragDropUISystem : UISystemBase
    {
        private const int DragStartThreshold = 30;

        private DragDropHandler _dragDropHandler;
        private DragDropView _dragDropView;

        private Vector2 _dragStartPosition;
        private bool _isDragging;

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
            if (TryResolveDragDropSource(e.Context.UnderlyingElement, out var source) && source.CanStartDrag())
            {
                _currentDragDropSource = source;
                _dragStartPosition = e.Context.ScreenPosition;
            }
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            if (_isDragging)
            {
                _dragDropHandler.Handle(_currentDragDropSource, _dragDropTargetCandidate);
            }

            _currentDragDropSource = null;
            _dragDropView.ClearVisual();
            _isDragging = false;

            _dragDropTargetCandidate?.SetDragDropHighlight(false);
            _dragDropTargetCandidate = null;
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            // Start Drag
            if (!_isDragging &&
                _currentDragDropSource != null &&
                e.Context.IsPressed &&
                (e.Context.ScreenPosition - _dragStartPosition).magnitude >= DragStartThreshold)
            {
                var visual = _currentDragDropSource.CreateDragDropVisual();
                _dragDropView.AttachVisual(visual);
                _isDragging = true;
            }

            if (!_isDragging) return;

            _dragDropView.transform.position = e.Context.ScreenPosition;

            // Get Drop candidate and set highlight
            TryResolveDragDropTarget(e.Context.UnderlyingElement, out var target);
            if (_dragDropTargetCandidate != target)
            {
                _dragDropTargetCandidate?.SetDragDropHighlight(false);
                _dragDropTargetCandidate = target;
                _dragDropTargetCandidate?.SetDragDropHighlight(true);
            }
        }

        private bool TryResolveDragDropSource(GameObject gameObject, out IDragDropSource dragDropSource)
        {
            dragDropSource = gameObject == null ? null : gameObject.GetComponentInParent<IDragDropSource>();
            return dragDropSource != null;
        }

        private bool TryResolveDragDropTarget(GameObject gameObject, out IDragDropTarget dragDropTarget)
        {
            dragDropTarget = gameObject.GetComponentInParent<IDragDropTarget>();
            return dragDropTarget != null;
        }
    }
}
