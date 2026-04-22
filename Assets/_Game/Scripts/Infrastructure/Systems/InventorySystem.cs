using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items.Inventory;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class InventorySystem : EntitySystemBase
    {
        protected override EntityQuery EntityQuery { get; } = new(RestrictionState.Disabled, new[] { typeof(InventoryModule), typeof(StorageModule) });

        public InventorySystem(IGlobalEventBus globalEventBus, EntityRepository repository) : base(globalEventBus, repository)
        {
            TrackGlobalEvent<InventorySortRequest>(OnInventorySortingRequested);
        }

        private void OnInventorySortingRequested(InventorySortRequest request)
        {
            switch (request.SortingType)
            {
                case InventorySortingType.ByName:
                    request.InventoryModel.SortByName();
                    break;
                case InventorySortingType.ByPurpose:
                    request.InventoryModel.SortByPurpose();
                    break;
            }
        }
    }

    public readonly struct InventorySortRequest : IGlobalEvent
    {
        public readonly InventoryModel InventoryModel;
        public readonly InventorySortingType SortingType;

        public InventorySortRequest(InventorySortingType sortingType, InventoryModel inventoryModel)
        {
            SortingType = sortingType;
            InventoryModel = inventoryModel;
        }
    }

    public enum InventorySortingType
    {
        ByName = 10,
        ByPurpose = 20,
    }
}
