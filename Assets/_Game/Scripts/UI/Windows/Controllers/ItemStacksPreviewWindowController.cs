using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class ItemStacksPreviewWindowController : MultiViewWindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly ItemStackPreviewViewData _primaryItemStackPreviewViewData;
        private readonly ItemStackPreviewViewData _secondaryItemStackPreviewViewData;

        public ItemStacksPreviewWindowController(
            ItemStackPreviewViewData primaryItemStackPreviewViewData,
            ItemStackPreviewViewData secondaryItemStackPreviewViewData,
            ItemStackPreviewViewController primaryItemStackPreviewViewController
            //ItemStackPreviewViewController secondaryItemStackPreviewViewController
            )
        {
            _primaryItemStackPreviewViewData = primaryItemStackPreviewViewData;
            _secondaryItemStackPreviewViewData = secondaryItemStackPreviewViewData;

            RegisterView(primaryItemStackPreviewViewController, _primaryItemStackPreviewViewData, () => Window.PrimaryItemPreviewView);

            //RegisterView(secondaryItemStackPreviewViewController, _secondaryItemStackPreviewViewData, () => Window.SecondaryItemPreviewView);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _primaryItemStackPreviewViewData.SetData(Arguments.ItemContainerPath, Arguments.ItemContainerSlot);
            _primaryItemStackPreviewViewData.SetEntityId(new ObservableData<string>(Arguments.ItemContainerPath.EntityId));

            //_secondaryItemStackPreviewViewData.SetData();
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
