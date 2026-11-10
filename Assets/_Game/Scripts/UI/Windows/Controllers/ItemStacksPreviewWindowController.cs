using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.ItemStacksPreview)]
    public sealed class ItemStacksPreviewWindowController : MultiViewWindowControllerBase<ItemStacksPreviewWindow, ItemStacksPreviewWindowControllerArguments>
    {
        private readonly ItemStackPreviewViewData _primaryItemStackPreviewViewData;
        private readonly EntityRepository _entityRepository;

        public ItemStacksPreviewWindowController(
            ItemStackPreviewViewData primaryItemStackPreviewViewData,
            ItemStackPreviewViewController primaryItemStackPreviewViewController,
            EntityRepository entityRepository)
        {
            _primaryItemStackPreviewViewData = primaryItemStackPreviewViewData;
            _entityRepository = entityRepository;

            RegisterView(primaryItemStackPreviewViewController, _primaryItemStackPreviewViewData, () => Window.PrimaryItemPreviewView);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (Arguments.ItemContainerPath.HasData)
            {
                _primaryItemStackPreviewViewData.SetData(Arguments.ItemContainerPath, Arguments.ItemContainerSlot, true);
                _primaryItemStackPreviewViewData.SetEntityId(_entityRepository.Observe(Arguments.ItemContainerPath.EntityId));
            }
            else if (Arguments.ItemStackSnapshot != null)
            {
                _primaryItemStackPreviewViewData.SetData(Arguments.ItemStackSnapshot, true);
            }
        }
    }

    public readonly struct ItemStacksPreviewWindowControllerArguments : IWindowControllerArguments
    {
        public readonly ItemContainerPath ItemContainerPath;
        public readonly long ItemContainerSlot;
        public readonly ItemStackSnapshot? ItemStackSnapshot;

        public ItemStacksPreviewWindowControllerArguments(ItemContainerPath itemContainerPath, long itemContainerSlot)
        {
            ItemContainerPath = itemContainerPath;
            ItemContainerSlot = itemContainerSlot;
            ItemStackSnapshot = null;
        }

        public ItemStacksPreviewWindowControllerArguments(ItemStackSnapshot? itemStackSnapshot)
        {
            ItemContainerPath = default;
            ItemContainerSlot = default;
            ItemStackSnapshot = itemStackSnapshot;
        }
    }
}
