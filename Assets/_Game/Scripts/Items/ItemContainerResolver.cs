using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;

namespace Assets._Game.Scripts.Items
{
    public sealed class ItemContainerResolver
    {
        private readonly EntityRepository _entityManager;

        public ItemContainerResolver(EntityRepository entityRepository)
        {
            _entityManager = entityRepository;
        }

        private bool TryResolveContainerInternal<T>(ItemContainerPath path, bool safeGetModule, out T container) where T : class, IItemContainer
        {
            var entityId = path.EntityId;
            container = (IItemContainer)(path.ContainerId switch
            {
                ItemContainerId.Equipment => GetEntityModule<EquipmentModule>(entityId, safeGetModule).Equipment,
                ItemContainerId.Inventory => GetEntityModule<InventoryModule>(entityId, safeGetModule).Inventory,
                ItemContainerId.Storage => GetEntityModule<StorageModule>(entityId, safeGetModule).Storage,
                ItemContainerId.Shop => GetEntityModule<ShopModule>(entityId, safeGetModule).Shop,
                _ => default
            }) as T;

            return container != null;
        }

        private T GetEntityModule<T>(string entityId, bool safe) where T : EntityModuleBase
        {
            var entity = _entityManager.Get(entityId);
            if (safe)
            {
                entity.TryGetModule<T>(out var module);
                return module;
            }
            else
            {
                return entity.GetModule<T>();
            }
        }

        public bool TryResolveContainer<T>(ItemContainerPath path, out T container) where T : class, IItemContainer
        {
            return TryResolveContainerInternal<T>(path, true, out container);
        }

        public bool TryResolveContainer(ItemContainerPath path, out IItemContainer container)
        {
            return TryResolveContainerInternal<IItemContainer>(path, true, out container);
        }

        public T ResolveContainer<T>(ItemContainerPath path) where T : class, IItemContainer
        {
            if (TryResolveContainerInternal<T>(path, false, out var container))
                return container;

            throw new ArgumentException($"Cannot resolve container {typeof(T)} from path: {path}");
        }

        public IItemContainer ResolveContainer(ItemContainerPath path)
        {
            return ResolveContainer<IItemContainer>(path);
        }
    }
}
