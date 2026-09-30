using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class ItemUseSettingsWindow : UIWindowBase
    {
        [field: SerializeField]
        public ItemUseSettingsView ItemUseSettingsView { get; private set; }
    }
}
