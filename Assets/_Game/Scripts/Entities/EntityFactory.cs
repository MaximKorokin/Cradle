using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public class EntityFactory
    {
        private readonly EntityRepository _entityRepository;
        private readonly InventoryEquipmentModuleFactory _inventoryEquipmentControllerAssembler;
        private readonly IReadOnlyList<IEntityModuleFactory> _moduleFactories;

        public EntityFactory(
            EntityRepository entityRepository,
            InventoryEquipmentModuleFactory inventoryEquipmentControllerAssembler,
            IReadOnlyList<IEntityModuleFactory> moduleFactories)
        {
            _entityRepository = entityRepository;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _moduleFactories = moduleFactories;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entity = new Entity(entityDefinition);

            for (int i = 0; i < _moduleFactories.Count; i++)
            {
                var module = _moduleFactories[i].Create(entityDefinition);
                if (module != null)
                {
                    entity.AddModule(module);
                }
            }

            _entityRepository.Add(entity);
            return entity;
        }

        public void Apply(Entity entity, EntitySave save)
        {
            if (entity.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                _inventoryEquipmentControllerAssembler.Apply(inventoryEquipmentModule, (save.InventorySave, save.EquipmentSave));
            }
        }

        public EntitySave Save(Entity entity)
        {
            var save = new EntitySave();
            if (entity.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                var inventoryEquipmentSave = _inventoryEquipmentControllerAssembler.Save(inventoryEquipmentModule);
                save.InventorySave = inventoryEquipmentSave.InventorySave;
                save.EquipmentSave = inventoryEquipmentSave.EquipmentSave;
            }
            return save;
        }
    }
}
