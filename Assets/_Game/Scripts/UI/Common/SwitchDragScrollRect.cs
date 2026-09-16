using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public class SwitchDragScrollRect : ScrollRect
    {
        // Not actually visible; at least in current Unity version
        // Custom inspector might be a solution if actually needed
        // Can be seen in Debug inspector mode
        [SerializeField]
        private bool _dragEnabled;

        public bool DragEnabled
        {
            get => _dragEnabled;
            set => _dragEnabled = value;
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (_dragEnabled)
            {
                base.OnInitializePotentialDrag(eventData);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (_dragEnabled)
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_dragEnabled)
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            // No condition to correctly finalize drag if it was disabled during drag
            base.OnEndDrag(eventData);
        }
    }
}
