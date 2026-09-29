using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
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

        protected override void Redraw()
        {
            var itemSnapshot = _itemContainerResolver.ResolveContainer(Arguments.ItemContainerPath).Get(Arguments.ItemContainerSlot);

            if (itemSnapshot == null) return;

            var equipmentPath = ItemContainerPath.Equipment(Arguments.ItemContainerPath.EntityId);
            _itemContainerResolver.TryResolveContainer<EquipmentModel>(equipmentPath, out var equipmentModel);

            Window.Render(_itemStackFormatter.FormatData((itemSnapshot.Value, equipmentModel)));
        }
    }

    public readonly struct ItemStacksPreviewWindowControllerArguments : IWindowControllerArguments
    {
        public readonly ItemContainerPath ItemContainerPath;
        public readonly long ItemContainerSlot;

        public ItemStacksPreviewWindowControllerArguments(ItemContainerPath itemContainerPath, long itemContainerSlot)
        {
            ItemContainerPath = itemContainerPath;
            ItemContainerSlot = itemContainerSlot;
        }
    }
}
