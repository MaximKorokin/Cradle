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

        bool ViewGold { get; }
        int Gold { get; }

        bool ViewWeight { get; }
        float WeightCurrent { get; }
        float WeightMax { get; }

        event Action Changed;
    }

    public class InventoryHudData : DataAggregatorBase, IInventoryHudData
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
            _statsController.StatChanged += OnStatsChanged;

            OnInventoryChanged();
            OnStatsChanged(StatId.CarryWeight);
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public bool ViewGold => true;
        public int Gold { get; private set; }

        public bool ViewWeight => true;
        public float WeightCurrent { get; private set; }
        public float WeightMax { get; private set; }

        public event Action Changed;

        private void OnInventoryChanged()
        {
            var newGold = 0;

            var goldDefinition = _itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            if (goldDefinition == null) return;

            foreach (var (_, snapshot) in _inventoryModel.Enumerate())
            {
                if (snapshot == null) continue;

                var itemSnapshot = snapshot.Value;
                if (itemSnapshot.Definition.Id == goldDefinition.Id)
                {
                    newGold += itemSnapshot.Amount;
                }
            }

            if (newGold != Gold)
            {
                Gold = newGold;
                Changed?.Invoke();
            }
        }

        private void OnStatsChanged(StatId statId)
        {
            if (statId == StatId.CarryWeight || statId == StatId.CarryWeightMax)
            {
                WeightCurrent = _statsController.Get(StatId.CarryWeight);
                WeightMax = _statsController.Get(StatId.CarryWeightMax);
                Changed?.Invoke();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _inventoryModel.Changed -= OnInventoryChanged;
            _statsController.StatChanged -= OnStatsChanged;
        }
    }

    public class StashHudData : DataAggregatorBase, IInventoryHudData
    {
        private readonly InventoryModel _inventoryModel;

        public StashHudData(PlayerContext playerContext)
        {
            _inventoryModel = playerContext.StashInventory;
            _inventoryModel.Changed += OnInventoryChanged;
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public bool ViewGold => true;
        public int Gold => 0;

        public bool ViewWeight => false;
        public float WeightCurrent => 0;
        public float WeightMax => 0;

        public event Action Changed;

        private void OnInventoryChanged()
        {
            Changed?.Invoke();
        }
        public override void Dispose()
        {
            base.Dispose();
            _inventoryModel.Changed -= OnInventoryChanged;
        }
    }
}
