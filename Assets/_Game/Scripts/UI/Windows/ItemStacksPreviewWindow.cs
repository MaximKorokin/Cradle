using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindow : UIWindowBase
    {
        [SerializeField]
        private ItemStacksPreviewView _primaryItemPreviewView;
        [SerializeField]
        private ItemStacksPreviewView _secondaryItemPreviewView;
        [SerializeField]
        private RectTransform _actionButtonParent;
        [SerializeField]
        private Button _actionButtonTemplate;

        private readonly List<Button> _actionButtons = new();

        public override void OnShow()
        {
            base.OnShow();

            _actionButtonTemplate.gameObject.SetActive(false);
        }

        public override void OnHide()
        {
            base.OnHide();

            Clear();
        }

        public void Render(ItemStackDisplayData primaryItemStack) => Render(primaryItemStack, default);

        public void Render(ItemStackDisplayData primaryItemStack, ItemStackDisplayData secondaryItemStack)
        {
            Clear();
            if (primaryItemStack.HasData)
            {
                _primaryItemPreviewView.Render(primaryItemStack);
            }
            if (secondaryItemStack.HasData)
            {
                _secondaryItemPreviewView.Render(secondaryItemStack);
            }
        }

        public void Clear()
        {
            _primaryItemPreviewView.Clear();
            _secondaryItemPreviewView.Clear();

            foreach (var button in _actionButtons)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }
            _actionButtons.Clear();
        }
    }
}
