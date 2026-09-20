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
        public ItemContainerDataAggregatorBase(ItemContainerResolver itemContainerResolver)
        {
            ItemContainerResolver = itemContainerResolver;
        }

        protected ItemContainerResolver ItemContainerResolver { get; private set; }

        public ItemContainerPath ContainerPath { get; protected set; }
        public IItemContainer ItemContainer { get; protected set; }

        public event Action Changed;

        protected void NotifyChanged()
        {
            Changed?.Invoke();
        }

        protected override void OnBoundEntityChanged(string entityId)
        {
            ContainerPath = GetContainerPath(entityId);
            var newItemContainer = ItemContainerResolver.ResolveContainer(ContainerPath);

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
