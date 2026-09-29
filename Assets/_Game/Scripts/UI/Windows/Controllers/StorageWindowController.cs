using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StorageWindowController : SingleViewWindowControllerBase<StorageWindow, StorageWindowControllerArguments, InventoryView, IInventoryViewData, InventoryViewController>
    {
        private readonly InventoryViewData _inventoryHudData;
        private readonly StorageViewData _storageHudData;
        private readonly EquipmentViewData _equipmentHudData;

        public StorageWindowController(
            InventoryViewController storageInventoryViewController,
            StorageViewData storageHudData,
            InventoryViewData inventoryHudData,
            EquipmentViewData equipmentHudData) : base(storageInventoryViewController, storageHudData)
        {
            _inventoryHudData = inventoryHudData;
            _storageHudData = storageHudData;
            _equipmentHudData = equipmentHudData;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _storageHudData.SetEntityId(Arguments.StorageEntityId);
            _inventoryHudData.SetEntityId(Arguments.InventoryEntityId);
            _equipmentHudData.SetEntityId(Arguments.EquipmentEntityId);
        }

        protected override InventoryView GetView() => Window.StorageInventoryView;

        public override void Dispose()
        {
            base.Dispose();

            _inventoryHudData.Dispose();
            _equipmentHudData.Dispose();
        }
    }

    public readonly struct StorageWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> StorageEntityId { get; }
        public IReadOnlyObservableData<string> InventoryEntityId { get; }
        public IReadOnlyObservableData<string> EquipmentEntityId { get; }

        public StorageWindowControllerArguments(
            IReadOnlyObservableData<string> storageEntityId,
            IReadOnlyObservableData<string> inventoryEntityId,
            IReadOnlyObservableData<string> equipmentEntityId)
        {
            InventoryEntityId = inventoryEntityId;
            StorageEntityId = storageEntityId;
            EquipmentEntityId = equipmentEntityId;
        }
    }
}
