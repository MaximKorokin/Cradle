using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryWindow : UIWindow
    {
        InventoryEquipmentController _controller;

        public void Init(InventoryEquipmentController controller)
        {
            _controller = controller;
        }

        public override void OnShow()
        {
            // _controller events +
        }

        public override void OnHide()
        {
            // _controller events -
        }
    }
}
