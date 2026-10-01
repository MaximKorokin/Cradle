using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemStacksPreviewWindowController : MultiViewWindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly ItemStackPreviewViewData _primaryItemStackPreviewViewData;

        public ItemStacksPreviewWindowController(
            ItemStackPreviewViewData primaryItemStackPreviewViewData,
            ItemStackPreviewViewController primaryItemStackPreviewViewController)
        {
            _primaryItemStackPreviewViewData = primaryItemStackPreviewViewData;

            RegisterView(primaryItemStackPreviewViewController, _primaryItemStackPreviewViewData, () => Window.PrimaryItemPreviewView);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _primaryItemStackPreviewViewData.SetData(Arguments.ItemContainerPath, Arguments.ItemContainerSlot);
            _primaryItemStackPreviewViewData.SetEntityId(new ObservableData<string>(Arguments.ItemContainerPath.EntityId));
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
