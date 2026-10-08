using Assets._Game.Scripts.Shared;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    [Window(WindowId.Storage)]
    public sealed class StorageWindowController : SingleViewWindowControllerBase<StorageWindow, StorageWindowControllerArguments, InventoryView, IInventoryViewData, InventoryViewController>
    {
        public StorageWindowController(
            InventoryViewController storageInventoryViewController,
            StorageViewData storageHudData) : base(storageInventoryViewController, storageHudData)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ViewData.SetEntityId(Arguments.StorageEntityId);
        }

        protected override InventoryView GetView() => Window.StorageInventoryView;
    }

    public readonly struct StorageWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<EntryRef> StorageEntityId { get; }

        public StorageWindowControllerArguments(IReadOnlyObservableData<EntryRef> storageEntityId)
        {
            StorageEntityId = storageEntityId;
        }
    }
}
