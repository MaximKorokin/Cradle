using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Items;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IItemContainerDataAggregator : IEntityBoundDataAggregatorBase, IDisposable
    {
        ItemContainerPath ContainerPath { get; }

        event Action Changed;
    }

    public abstract class ItemContainerDataAggregatorBase : EntityBoundDataAggregatorBase, IItemContainerDataAggregator
    {
        public ItemContainerPath ContainerPath { get; protected set; }
        public IItemContainer ItemContainer { get; protected set; }

        private ItemContainerResolver _itemContainerResolver;

        public event Action Changed;

        public ItemContainerDataAggregatorBase(
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(entityRepository)
        {
            _itemContainerResolver = itemContainerResolver;
        }

        protected void NotifyChanged()
        {
            Changed?.Invoke();
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
