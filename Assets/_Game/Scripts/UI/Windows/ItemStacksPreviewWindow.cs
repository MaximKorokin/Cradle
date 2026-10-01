using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemStacksPreviewWindow : UIWindowBase
    {
        [field: SerializeField]
        public ItemStackPreviewView PrimaryItemPreviewView { get; private set; }
        [field: SerializeField]
        public ItemStackPreviewView SecondaryItemPreviewView { get; private set; }
    }
}
