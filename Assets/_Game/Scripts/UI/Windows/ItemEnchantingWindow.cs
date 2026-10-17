using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemEnchantingWindow : UIWindowBase
    {
        [field: SerializeField]
        public ItemEnchantingView EnchantingView { get; private set; }
    }
}
