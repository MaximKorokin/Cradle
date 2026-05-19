using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Configs;
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

        bool ViewPneuma { get; }
        int Pneuma { get; }

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
        void SetInventoryEntity(string inventoryEntityId);
    }

    public abstract class InventoryHudDataBase : DataAggregatorBase, IInventoryHudData
    {
        private InventoryModel _inventoryModel;

        private readonly ItemsConfig _itemsConfig;
        private readonly ItemContainerResolver _itemContainerResolver;

        private Func<ItemStackSnapshot?, bool> _enumerationFilter;

        public InventoryHudDataBase(ItemsConfig itemsConfig, ItemContainerResolver itemContainerResolver)
        {
            _itemsConfig = itemsConfig;
            _itemContainerResolver = itemContainerResolver;
        }

        public InventoryModel InventoryModel => _inventoryModel;

        public abstract bool ViewPneuma { get; }
        public int Pneuma { get; private set; }

        public abstract bool ViewGold { get; }
        public int Gold { get; private set; }

        public abstract bool ViewWeight { get; }
        public abstract float WeightCurrent { get; }
        public abstract float WeightMax { get; }

        public abstract bool ViewSlotsAmount { get; }
        public int SlotsUsed { get; private set; }
        public int SlotsMax { get; private set; }

        public event Action Changed;

        protected abstract ItemContainerPath GetInventoryPath(string entityId);

        protected void NotifyChanged() => Changed?.Invoke();

        private void OnInventoryChanged()
        {
            Pneuma = InventoryHudDataUtils.CalculatePneuma(_itemsConfig, _inventoryModel);
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

        public virtual void SetInventoryEntity(string inventoryEntityId)
        {
            var inventoryModel = _itemContainerResolver.ResolveInventory(GetInventoryPath(inventoryEntityId));
            if (_inventoryModel != inventoryModel)
            {
                _inventoryModel = inventoryModel;
                _inventoryModel.Changed -= OnInventoryChanged;
                inventoryModel.Changed += OnInventoryChanged;
                OnInventoryChanged();
            }
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
        private readonly EntityRepository _entityRepository;

        private IStatsReadOnly _statsController;
        private float _weightCurrent;
        private float _weightMax;

        public InventoryHudData(EntityRepository entityRepository, ItemsConfig itemsConfig, ItemContainerResolver itemContainerResolver) : base(itemsConfig, itemContainerResolver)
        {
            _entityRepository = entityRepository;
        }

        public override void SetInventoryEntity(string inventoryEntityId)
        {
            base.SetInventoryEntity(inventoryEntityId);

            var newStats = _entityRepository.Get(inventoryEntityId).GetModule<StatModule>().Stats;
            if (_statsController != newStats)
            {
                if (_statsController != null)
                    _statsController.StatChanged -= OnStatsChanged;

                _statsController = newStats;
                _statsController.StatChanged += OnStatsChanged;
                OnStatsChanged(StatId.CarryWeight);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_statsController != null)
                _statsController.StatChanged -= OnStatsChanged;
        }

        protected override ItemContainerPath GetInventoryPath(string entityId) => ItemContainerPath.Inventory(entityId);

        public override bool ViewPneuma => true;

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
        public StorageHudData(ItemsConfig itemsConfig, ItemContainerResolver itemContainerResolver) : base(itemsConfig, itemContainerResolver)
        {
        }

        protected override ItemContainerPath GetInventoryPath(string entityId) => ItemContainerPath.Storage(entityId);

        public override bool ViewPneuma => false;

        public override bool ViewGold => false;

        public override bool ViewWeight => false;
        public override float WeightCurrent => 0;
        public override float WeightMax => 0;

        public override bool ViewSlotsAmount => true;
    }

    public static class InventoryHudDataUtils
    {
        public static int CalculatePneuma(ItemsConfig itemsConfig, InventoryModel inventoryModel)
        {
            var pneuma = 0;

            var pneumaDefinition = itemsConfig.GetSpecialItemDefinition(SpecialItemId.Pneuma);
            if (pneumaDefinition == null) return 0;

            foreach (var (_, snapshot) in inventoryModel.Enumerate())
            {
                if (snapshot == null) continue;

                var itemSnapshot = snapshot.Value;
                if (itemSnapshot.Definition.Id == pneumaDefinition.Id)
                {
                    pneuma += itemSnapshot.Amount;
                }
            }

            return pneuma;
        }
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
