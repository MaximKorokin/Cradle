using Assets._Game.Scripts.UI.DataFormatters;
using Assets._Game.Scripts.UI.Views.Widgets;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindow : UIWindowBase
    {
        [SerializeField]
        private ItemStacksPreviewWidget _primaryItemPreviewView;
        [SerializeField]
        private ItemStacksPreviewWidget _secondaryItemPreviewView;

        public override void OnHide()
        {
            base.OnHide();

            Clear();
        }

        public void Render(ItemStackDisplayData primaryItemStack) => Render(primaryItemStack, default);

        public void Render(ItemStackDisplayData primaryItemStack, ItemStackDisplayData secondaryItemStack)
        {
            Clear();
            if (primaryItemStack.HasData && _primaryItemPreviewView != null)
            {
                _primaryItemPreviewView.Render(primaryItemStack);
            }
            if (secondaryItemStack.HasData && _secondaryItemPreviewView != null)
            {
                _secondaryItemPreviewView.Render(secondaryItemStack);
            }
        }

        public void Clear()
        {
            if (_primaryItemPreviewView != null)
            {
                _primaryItemPreviewView.Clear();
            }
            if (_secondaryItemPreviewView != null)
            {
                _secondaryItemPreviewView.Clear();
            }
        }
    }
}
