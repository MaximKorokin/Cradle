using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IItemContainerDataAggregator : IEntityBoundDataAggregatorBase, IDisposable
    {
        ItemContainerPath ContainerPath { get; }
    }

    public abstract class ItemContainerDataAggregatorBase : EntityBoundDataAggregatorBase, IItemContainerDataAggregator
    {
        public ItemContainerPath ContainerPath { get; private set; }
        public IItemContainer ItemContainer { get; private set; }

        private readonly ItemContainerResolver _itemContainerResolver;

        public ItemContainerDataAggregatorBase(
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(entityRepository)
        {
            _itemContainerResolver = itemContainerResolver;
        }

        protected override void OnBoundEntityChanged(string entityId)
        {
            if (entityId == null) return;

            ContainerPath = GetContainerPath(entityId);
            var newItemContainer = _itemContainerResolver.ResolveContainer(ContainerPath);

            if (ItemContainer != newItemContainer)
            {
                if (ItemContainer != null)
                {
                    ItemContainer.Changed -= OnContainerChanged;
                }
                ItemContainer = newItemContainer;
                if (ItemContainer != null)
                {
                    ItemContainer.Changed += OnContainerChanged;
                }
                OnContainerChanged();
            }
        }

        protected virtual void OnContainerChanged()
        {
            NotifyChanged();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (ItemContainer != null)
            {
                ItemContainer.Changed -= OnContainerChanged;
            }
        }

        protected abstract ItemContainerPath GetContainerPath(string entityId);
    }
}
