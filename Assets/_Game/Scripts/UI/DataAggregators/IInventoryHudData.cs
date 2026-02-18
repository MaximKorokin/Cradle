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
            Gold = InventoryHudDataUtils.CalculateGold(_itemsConfig, _inventoryModel);

            Changed?.Invoke();
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
        private readonly ItemsConfig _itemsConfig;

        public StashHudData(PlayerContext playerContext, ItemsConfig itemsConfig)
        {
            _inventoryModel = playerContext.StashInventory;
            _itemsConfig = itemsConfig;

            _inventoryModel.Changed += OnInventoryChanged;
            OnInventoryChanged();
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public bool ViewGold => true;
        public int Gold { get; private set; }

        public bool ViewWeight => false;
        public float WeightCurrent => 0;
        public float WeightMax => 0;

        public event Action Changed;

        private void OnInventoryChanged()
        {
            Gold = InventoryHudDataUtils.CalculateGold(_itemsConfig, _inventoryModel);

            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
            _inventoryModel.Changed -= OnInventoryChanged;
        }
    }

    public static class InventoryHudDataUtils
    {
        public static int CalculateGold(ItemsConfig itemsConfig, InventoryModel inventoryModel)
        {
            var gold = 0;

            var goldDefinition = itemsConfig.GetSpecialItemDefinition(SpecialItemId.Gold);
            if (goldDefinition == null) return 0;

            foreach (var (_, snapshot) in inventoryModel.Enumerate())
            {
                if (snapshot == null) continue;

                var itemSnapshot = snapshot.Value;
                if (itemSnapshot.Definition.Id == goldDefinition.Id)
                {
                    gold += itemSnapshot.Amount;
                }
            }

            return gold;
        }
    }
}
