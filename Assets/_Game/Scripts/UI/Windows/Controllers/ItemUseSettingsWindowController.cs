using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.ItemUseSettings)]
    public sealed class ItemUseSettingsWindowController : SingleViewWindowControllerBase<ItemUseSettingsWindow, ItemUseSettingsWindowControllerArguments, ItemUseSettingsView, ItemUseSettingsViewData, ItemUseSettingsViewController>
    {
        public ItemUseSettingsWindowController(
            ItemUseSettingsViewController itemUseSettingsViewController,
            ItemUseSettingsViewData itemUseSettingsViewData) : base(itemUseSettingsViewController, itemUseSettingsViewData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override ItemUseSettingsView GetView() => Window.ItemUseSettingsView;
    }

    public readonly struct ItemUseSettingsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> EquipmentEntityId { get; }

        public ItemUseSettingsWindowControllerArguments(IReadOnlyObservableData<EntryRef> equipmentEntityId)
        {
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
