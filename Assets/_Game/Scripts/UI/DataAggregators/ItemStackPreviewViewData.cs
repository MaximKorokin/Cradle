using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class ItemStackPreviewViewData : ItemContainerDataAggregatorBase
    {
        private readonly ItemStackFormatter _itemStackFormatter;

        private ItemContainerPath _itemContainerPath;
        private long _slotIndex;
        private bool _provideFullData;

        public ItemStackPreviewViewData(
            ItemStackFormatter itemStackFormatter,
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(itemContainerResolver, entityRepository)
        {
            _itemStackFormatter = itemStackFormatter;
        }

        public ItemStackDisplayData GetDisplayData()
        {
            var itemStackSnapshot = GetItemStackSnapshot();

            if (!itemStackSnapshot.HasValue) return default;

            var equipmentPath = ItemContainerPath.Equipment(_itemContainerPath.EntityId);
            ItemContainerResolver.TryResolveContainer(equipmentPath, out EquipmentModel equipmentModel);

            return _itemStackFormatter.FormatData((itemStackSnapshot.Value, equipmentModel, _provideFullData));
        }

        public void SetData(ItemContainerPath itemContainerPath, long slotIndex, bool provideFullData)
        {
            if (_itemContainerPath != default)
            {
                var oldItemStackSnapshot = GetItemStackSnapshot();
                if (oldItemStackSnapshot.HasValue && oldItemStackSnapshot.Value.InstanceData != null)
                {
                    oldItemStackSnapshot.Value.InstanceData.Changed -= OnItemStackInstanceDataChanged;
                }
            }

            _itemContainerPath = itemContainerPath;
            _slotIndex = slotIndex;
            _provideFullData = provideFullData;

            var itemStackSnapshot = GetItemStackSnapshot();
            if (itemStackSnapshot.HasValue && itemStackSnapshot.Value.InstanceData != null)
            {
                itemStackSnapshot.Value.InstanceData.Changed -= OnItemStackInstanceDataChanged;
                itemStackSnapshot.Value.InstanceData.Changed += OnItemStackInstanceDataChanged;
            }

            NotifyChanged();
        }

        public override void Dispose()
        {
            base.Dispose();

            var itemStackSnapshot = GetItemStackSnapshot();
            if (itemStackSnapshot.HasValue && itemStackSnapshot.Value.InstanceData != null)
            {
                itemStackSnapshot.Value.InstanceData.Changed -= OnItemStackInstanceDataChanged;
            }
        }

        private void OnItemStackInstanceDataChanged(IItemInstanceData instanceData)
        {
            NotifyChanged();
        }

        protected override ItemContainerPath GetContainerPath(string entityId) => new(entityId, _itemContainerPath.ContainerId);

        private ItemStackSnapshot? GetItemStackSnapshot()
        {
            if (_itemContainerPath == default) return null;

            if (!ItemContainerResolver.TryResolveContainer(_itemContainerPath, out var container)) return null;
            return container.Get(_slotIndex);
        }
    }
}
