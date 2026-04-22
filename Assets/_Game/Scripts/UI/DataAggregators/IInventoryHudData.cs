using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

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

        bool ViewSlotsAmount { get; }
        int SlotsUsed { get; }
        int SlotsMax { get; }

        event Action Changed;

        IEnumerable<(InventorySlot Slot, ItemStackSnapshot? Item)> Enumerate();
        void SetEnumerationFilter(Func<ItemStackSnapshot?, bool> filter);
    }

    public abstract class InventoryHudDataBase : DataAggregatorBase, IInventoryHudData
    {
        private readonly InventoryModel _inventoryModel;
        private readonly ItemsConfig _itemsConfig;

        private Func<ItemStackSnapshot?, bool> _enumerationFilter;

        public InventoryHudDataBase(InventoryModel inventoryModel, ItemsConfig itemsConfig)
        {
            _inventoryModel = inventoryModel;
            _itemsConfig = itemsConfig;

            _inventoryModel.Changed += OnInventoryChanged;
            OnInventoryChanged();
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public abstract bool ViewGold { get; }
        public int Gold { get; private set; }

        public abstract bool ViewWeight { get; }
        public abstract float WeightCurrent { get; }
        public abstract float WeightMax { get; }

        public abstract bool ViewSlotsAmount { get; }
        public int SlotsUsed { get; private set; }
        public int SlotsMax { get; private set; }

        public event Action Changed;

        protected void NotifyChanged() => Changed?.Invoke();

        private void OnInventoryChanged()
        {
            Gold = InventoryHudDataUtils.CalculateGold(_itemsConfig, _inventoryModel);
            SlotsUsed = InventoryHudDataUtils.CalculateSlotsUsed(_inventoryModel);
            SlotsMax = InventoryHudDataUtils.CalculateSlotsMax(_inventoryModel);

            NotifyChanged();
        }

        public override void Dispose()
        {
            base.Dispose();

            _inventoryModel.Changed -= OnInventoryChanged;
        }

        public IEnumerable<(InventorySlot Slot, ItemStackSnapshot? Item)> Enumerate()
        {
            var slotIndex = 0;
            foreach (var (_, snapshot) in _inventoryModel.Enumerate())
            {
                if (_enumerationFilter == null || _enumerationFilter(snapshot))
                {
                    yield return (InventorySlot.FromInt64(slotIndex), snapshot);
                    slotIndex++;
                }
            }
        }

        public void SetEnumerationFilter(Func<ItemStackSnapshot?, bool> filter)
        {
            _enumerationFilter = filter;
            NotifyChanged();
        }
    }

    public class InventoryHudData : InventoryHudDataBase
    {
        private readonly IStatsReadOnly _statsController;

        private float _weightCurrent;
        private float _weightMax;

        public InventoryHudData(PlayerContext playerContext, ItemsConfig itemsConfig) : base(playerContext.GetModule<InventoryModule>().Inventory, itemsConfig)
        {
            _statsController = playerContext.GetModule<StatModule>().Stats;

            _statsController.StatChanged += OnStatsChanged;

            OnStatsChanged(StatId.CarryWeight);
        }

        public override void Dispose()
        {
            base.Dispose();
            _statsController.StatChanged -= OnStatsChanged;
        }

        public override bool ViewGold => true;

        public override bool ViewWeight => true;
        public override float WeightCurrent => _weightCurrent;
        public override float WeightMax => _weightMax;

        public override bool ViewSlotsAmount => true;

        private void OnStatsChanged(StatId statId)
        {
            if (statId == StatId.CarryWeight || statId == StatId.CarryWeightMax)
            {
                _weightCurrent = _statsController.Get(StatId.CarryWeight);
                _weightMax = _statsController.Get(StatId.CarryWeightMax);
                NotifyChanged();
            }
        }
    }

    public class StorageHudData : InventoryHudDataBase
    {
        public StorageHudData(PlayerContext playerContext, ItemsConfig itemsConfig) : base(playerContext.GetModule<StorageModule>().Storage, itemsConfig)
        {
        }

        public override bool ViewGold => true;

        public override bool ViewWeight => false;
        public override float WeightCurrent => 0;
        public override float WeightMax => 0;

        public override bool ViewSlotsAmount => true;
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

        public static int CalculateSlotsUsed(InventoryModel inventoryModel)
        {
            return inventoryModel.Enumerate().Count(slot => slot.Snapshot != null);
        }

        public static int CalculateSlotsMax(InventoryModel inventoryModel)
        {
            return inventoryModel.Capacity;
        }
    }
}
