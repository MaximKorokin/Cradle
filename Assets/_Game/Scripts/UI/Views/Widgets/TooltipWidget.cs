using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class TooltipWidget : UIViewBase
    {
        [SerializeField]
        private RectTransform _contentParent;

        public void SetVisible(bool visible)
        {
            if (visible && !gameObject.activeSelf)
                gameObject.SetActive(visible);
            else if (!visible && gameObject.activeSelf)
                gameObject.SetActive(visible);
        }

        public void SetPosition(Vector2 position)
        {
            if (transform.position == (Vector3)position) return;

            transform.position = position;
        }

        public void SetContent(RectTransform content)
        {
            if (content == null || content.parent == _contentParent) return;

            content.transform.DisableRaycastTargets();
            content.transform.SetParent(_contentParent, false);
        }

        public override bool TickRender()
        {
            return true;
        }
    }
}
