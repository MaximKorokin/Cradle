using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindow : UIWindow
    {
        [SerializeField]
        private ItemPreviewView _primaryItemPreviewView;
        [SerializeField]
        private ItemPreviewView _secondaryItemPreviewView;

        public override bool IsModal => true;

        public override void OnShow()
        {

        }

        public override void OnHide()
        {

        }

        public void Render(ItemStack firstItem, ItemStack secondStack)
        {
            _primaryItemPreviewView.Clear();
            _secondaryItemPreviewView.Clear();
            if (firstItem != null)
            {
                _primaryItemPreviewView.Render(firstItem);
            }
            if (secondStack != null)
            {
                _secondaryItemPreviewView.Render(secondStack);
            }
        }
    }
}
