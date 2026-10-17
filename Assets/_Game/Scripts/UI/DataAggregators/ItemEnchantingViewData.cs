using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class ItemEnchantingViewData : EntityBoundDataAggregatorBase
    {
        private readonly ItemContainerResolver _itemContainerResolver;

        public ItemStackPath EnchantableItemPath { get; private set; }
        public ItemStackPath EnchantingItemPath { get; private set; }
        public ItemStackSnapshot? EnchantableItem { get; private set; }
        public ItemStackSnapshot? EnchantingItem { get; private set; }
        public string EnchantEffects {  get; private set; }

        public ItemEnchantingViewData(
            EntityRepository entityRepository,
            ItemContainerResolver itemContainerResolver) : base(entityRepository)
        {
            _itemContainerResolver = itemContainerResolver;
        }

        public void SetEnchantableItem(ItemStackPath path)
        {
            if (!_itemContainerResolver.TryResolveContainer(path.ContainerPath, out var container))
                throw new ArgumentException($"Could not resolve container with path {path.ContainerPath}");

            EnchantableItemPath = path;
            EnchantableItem = container.Get(path.Slot);

            NotifyChanged();
        }

        public void SetEnchantingItem(ItemStackPath path)
        {
            if (!_itemContainerResolver.TryResolveContainer(path.ContainerPath, out var container))
                throw new ArgumentException($"Could not resolve container with path {path.ContainerPath}");

            EnchantingItemPath = path;
            EnchantingItem = container.Get(path.Slot);

            NotifyChanged();
        }

        protected override void OnBoundEntityChanged()
        {
            NotifyChanged();
        }
    }
}
