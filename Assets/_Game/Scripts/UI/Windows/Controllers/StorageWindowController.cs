using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public class StorageWindowController : WindowControllerBase<StorageWindow, StorageWindowControllerArguments>
    {
        private readonly InventoryViewController _storageInventoryViewController;
        private readonly InventoryHudData _inventoryHudData;
        private readonly StorageHudData _storageHudData;
        private readonly EquipmentHudData _equipmentHudData;

        public StorageWindowController(
            InventoryViewController stoargeInventoryViewController,
            InventoryHudData inventoryHudData,
            StorageHudData storageHudData,
            EquipmentHudData equipmentHudData)
        {
            _storageInventoryViewController = stoargeInventoryViewController;
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

        protected override void OnBind()
        {
            base.OnBind();

            _storageInventoryViewController.Initialize(Window.StorageInventoryView);
            _storageInventoryViewController.Bind(_storageHudData);
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _storageInventoryViewController.Unbind();
        }

        protected override void Redraw()
        {
            _storageInventoryViewController.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            _storageInventoryViewController.Dispose();
            _inventoryHudData.Dispose();
            _storageHudData.Dispose();
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
