using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.UI.DataFormatters;
using System;
using System.IO;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class ItemEnchantingViewData : EntityBoundDataAggregatorBase
    {
        private readonly ItemContainerResolver _itemContainerResolver;
        private readonly EnchantableTraitFormatter _enchantableTraitFormatter;

        public ItemStackPath EnchantableItemPath { get; private set; }
        public ItemStackPath EnchantingItemPath { get; private set; }

        public ItemEnchantingViewData(
            EntityRepository entityRepository,
            ItemContainerResolver itemContainerResolver,
            EnchantableTraitFormatter enchantableTraitFormatter) : base(entityRepository)
        {
            _itemContainerResolver = itemContainerResolver;
            _enchantableTraitFormatter = enchantableTraitFormatter;
        }

        public ItemStackSnapshot? GetEnchantableItem()
        {
            GetItemData(EnchantableItemPath, out _, out var item);
            return item;
        }

        public ItemStackSnapshot? GetEnchantingItem()
        {
            GetItemData(EnchantingItemPath, out _, out var item);
            return item;
        }

        public EnchantDisplayData GetEnchantDisplayData()
        {
            var item = GetEnchantableItem();
            if (item == null) return default;

            return _enchantableTraitFormatter.FormatData((item.Value.Definition, item.Value.InstanceData));
        }

        public EnchantItem GetEnchantItemDisplayData(EnchantDisplayData displayData)
        {
            var enchantingItem = GetEnchantingItem();
            if (!displayData.HasData || enchantingItem == null) return default;
            return displayData.EligibleItems.FirstOrDefault(x => x.Name == enchantingItem.Value.Definition.Name);
        }

        public bool CanEnchant()
        {
            GetItemData(EnchantableItemPath, out _, out var enchantableItem);
            GetItemData(EnchantingItemPath, out _, out var enchantingItem);
            if (enchantableItem == null || enchantingItem == null) return false;
            if (!enchantableItem.Value.Definition.TryGetTrait<EnchantableTrait>(out var enchantableTrait)) return false;
            if (!enchantableItem.Value.InstanceData.TryGet<EnchantInstanceData>(out var enchantInstanceData)) return false;

            var method = enchantableTrait.ItemEnchantingDefinition.Methods.FirstOrDefault(x => x.Item == enchantingItem.Value.Definition);
            if (method ==  null) return false;

            return method.Rules.Count > enchantInstanceData.Level;
        }

        public void SetEnchantableItem(ItemStackPath path)
        {
            if (path == EnchantableItemPath) return;

            GetItemData(path, out var itemPath, out var item);

            // Item is not enchantable
            if (item == null ||
                !item.Value.Definition.TryGetTrait<EnchantableTrait>(out var enchantableTrait) ||
                !item.Value.InstanceData.TryGet<EnchantInstanceData>(out _))
            {
                EnchantableItemPath = default;
                EnchantingItemPath = default;
            }
            else
            {
                EnchantableItemPath = itemPath;

                var enchantingItem = GetEnchantingItem();
                if (enchantingItem != null && !enchantableTrait.ItemEnchantingDefinition.Methods.Any(x => x.Item == enchantingItem.Value.Definition))
                {
                    EnchantingItemPath = default;
                }
            }

            NotifyChanged();
        }

        public void SetEnchantingItem(ItemStackPath path)
        {
            if (path == EnchantingItemPath) return;

            GetItemData(path, out var itemPath, out var item);

            var enchantableItem = GetEnchantableItem();
            if (item == null ||
                enchantableItem == null ||
                !enchantableItem.Value.Definition.TryGetTrait<EnchantableTrait>(out var enchantableTrait) ||
                !enchantableItem.Value.InstanceData.TryGet<EnchantInstanceData>(out _) ||
                !enchantableTrait.ItemEnchantingDefinition.Methods.Any(x => x.Item == item.Value.Definition))
            {
                EnchantingItemPath = default;
            }
            else
            {
                EnchantingItemPath = itemPath;
            }

            NotifyChanged();
        }

        public void GetItemData(ItemStackPath path, out ItemStackPath itemPath, out ItemStackSnapshot? item)
        {
            itemPath = default;
            item = default;

            if (path == default)
            {
                itemPath = path;
                item = null;
            }
            else
            {
                if (!_itemContainerResolver.TryResolveContainer(path.ContainerPath, out var container))
                    throw new ArgumentException($"Could not resolve container with path {path.ContainerPath}");

                itemPath = path;
                item = container.Get(path.Slot);
            }
        }

        protected override void OnBoundEntityChanged()
        {
            NotifyChanged();
        }
    }
}
