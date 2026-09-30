using Assets._Game.Scripts.Shared.Extensions;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class TooltipWidget : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _contentParent;

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetContent(RectTransform content)
        {
            content.transform.DisableRaycastTargets();
            content.transform.SetParent(_contentParent, false);
        }
    }
}
