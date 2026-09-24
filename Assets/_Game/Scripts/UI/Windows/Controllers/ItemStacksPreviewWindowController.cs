using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemStacksPreviewWindowController : WindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly ItemStackFormatter _itemStackFormatter;

        public ItemStacksPreviewWindowController(
            ItemContainerResolver itemContainerResolver,
            ItemStackFormatter itemStackFormatter)
        {
            _itemContainerResolver = itemContainerResolver;
            _itemStackFormatter = itemStackFormatter;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void Redraw()
        {
            var equipmentModel = _itemContainerResolver.ResolveEquipment(Arguments.EquipmentPath);
            Window.Render(_itemStackFormatter.FormatData((Arguments.ItemStackSnapshot, equipmentModel)));
        }
    }

    public readonly struct ItemStacksPreviewWindowControllerArguments : IWindowControllerArguments
    {
        public readonly ItemContainerPath EquipmentPath;
        public readonly ItemStackSnapshot ItemStackSnapshot;

        public ItemStacksPreviewWindowControllerArguments(ItemContainerPath equipmentPath, ItemStackSnapshot itemStackSnapshot)
        {
            EquipmentPath = equipmentPath;
            ItemStackSnapshot = itemStackSnapshot;
        }
    }
}
