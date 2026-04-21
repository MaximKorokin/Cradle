using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryInventoryWindow : UIWindowBase
    {
        [SerializeField]
        private InventoryView _firstInventoryView;
        [SerializeField]
        private InventoryView _secondInventoryView;

        public InventoryView FirstInventoryView => _firstInventoryView;
        public InventoryView SecondInventoryView => _secondInventoryView;

        public override void OnHide()
        {
            base.OnHide();

            _firstInventoryView.Unbind();
            _secondInventoryView.Unbind();
        }

        public void Render(IInventoryHudData firstInventoryHudData, IInventoryHudData secondInventoryHudData)
        {
            _firstInventoryView.Render(firstInventoryHudData);
            _secondInventoryView.Render(secondInventoryHudData);
        }
    }
}
