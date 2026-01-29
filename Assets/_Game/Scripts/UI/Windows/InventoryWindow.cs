using Assets._Game.Scripts.Entities.Items.Inventory;

namespace Assets._Game.Scripts.UI.Windows
{
    public class InventoryWindow : UIWindow
    {
        InventoryController _controller;

        public void Init(InventoryController controller)
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
