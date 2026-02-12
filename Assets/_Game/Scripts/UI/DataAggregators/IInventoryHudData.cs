using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IInventoryHudData : IDisposable
    {
        InventoryModel InventoryModel { get; }
        int Gold { get; }
        float WeightCurrent { get; }
        float WeightMax { get; }

        event Action Changed;
    }

    public class InventoryHudData : IInventoryHudData
    {
        private readonly InventoryModel _inventoryModel;
        private readonly IStatsReadOnly _statsController;
        private readonly ItemsConfig _itemsConfig;

        public InventoryHudData(PlayerContext playerContext, ItemsConfig itemsConfig)
        {
            _inventoryModel = playerContext.IEModule.Inventory;
            _statsController = playerContext.StatsModule.Stats;
            _itemsConfig = itemsConfig;

            _inventoryModel.Changed += OnInventoryChanged;
        }

        private void OnInventoryChanged()
        {
            Gold = 0;
            foreach (var (_, snapshot) in InventoryModel.Enumerate())
            {
                if (snapshot == null) continue;

                var itemSnapshot = snapshot.Value;
                var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
                if (snapshot != null && itemSnapshot.Definition.Id == goldDefinition.Id)
                {
                    Gold += itemSnapshot.Amount;
                }
            }

            Changed?.Invoke();
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public int Gold { get; private set; }

        public float WeightCurrent { get; private set; }

        public float WeightMax { get; private set; }

        public event Action Changed;

        public void Dispose()
        {
            _inventoryModel.Changed -= OnInventoryChanged;
        }
    }
}
