using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class EquipmentWindow : UIWindowBase
    {
        [field: SerializeField]
        public EquipmentView EquipmentView { get; private set; }
    }
}
