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

        public ItemStackPreviewViewData(
            ItemStackFormatter itemStackFormatter,
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(itemContainerResolver, entityRepository)
        {
            _itemStackFormatter = itemStackFormatter;
        }

        public ItemStackDisplayData GetDisplayData()
        {
            var itemStackSnapshot = ItemContainerResolver.ResolveContainer(_itemContainerPath).Get(_slotIndex);

            if (!itemStackSnapshot.HasValue) return default;

            var equipmentPath = ItemContainerPath.Equipment(_itemContainerPath.EntityId);
            ItemContainerResolver.TryResolveContainer(equipmentPath, out EquipmentModel equipmentModel);

            return _itemStackFormatter.FormatData((itemStackSnapshot.Value, equipmentModel));
        }

        public void SetData(ItemContainerPath itemContainerPath, long slotIndex)
        {
            if (_itemContainerPath != default)
            {
                var oldItemStackSnapshot = ItemContainerResolver.ResolveContainer(_itemContainerPath).Get(slotIndex);
                if (oldItemStackSnapshot.HasValue && oldItemStackSnapshot.Value.InstanceData != null)
                {
                    oldItemStackSnapshot.Value.InstanceData.Changed -= OnItemStackInstanceDataChanged;
                }
            }

            _itemContainerPath = itemContainerPath;
            _slotIndex = slotIndex;

            var itemStackSnapshot = ItemContainerResolver.ResolveContainer(_itemContainerPath).Get(slotIndex).Value;
            if (itemStackSnapshot.InstanceData != null)
            {
                itemStackSnapshot.InstanceData.Changed -= OnItemStackInstanceDataChanged;
                itemStackSnapshot.InstanceData.Changed += OnItemStackInstanceDataChanged;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            var itemStackSnapshot = ItemContainerResolver.ResolveContainer(_itemContainerPath).Get(_slotIndex).Value;
            if (itemStackSnapshot.InstanceData != null)
            {
                itemStackSnapshot.InstanceData.Changed -= OnItemStackInstanceDataChanged;
            }
        }

        private void OnItemStackInstanceDataChanged(IItemInstanceData instanceData)
        {
            NotifyChanged();
        }

        protected override ItemContainerPath GetContainerPath(string entityId) => new(entityId, _itemContainerPath.ContainerId);
    }
}
