using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Persistence;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities
{
    public class EntityFactory
    {
        private readonly EntityRepository _entityRepository;
        private readonly InventoryModuleFactory _inventoryModuleAssembler;
        private readonly EquipmentModuleFactory _equipmentModuleAssembler;
        private readonly IReadOnlyList<IEntityModuleFactory> _moduleFactories;

        public EntityFactory(
            EntityRepository entityRepository,
            InventoryModuleFactory inventoryModuleAssembler,
            EquipmentModuleFactory equipmentModuleAssembler,
            IReadOnlyList<IEntityModuleFactory> moduleFactories)
        {
            _entityRepository = entityRepository;
            _inventoryModuleAssembler = inventoryModuleAssembler;
            _equipmentModuleAssembler = equipmentModuleAssembler;
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
            if (entity.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                _inventoryModuleAssembler.Apply(inventoryModule, save.InventorySave);
            }
            if (entity.TryGetModule<EquipmentModule>(out var equipmentModule))
            {
                _equipmentModuleAssembler.Apply(equipmentModule, save.EquipmentSave);
            }
        }

        public EntitySave Save(Entity entity)
        {
            var save = new EntitySave();
            if (entity.TryGetModule<InventoryModule>(out var inventoryModule))
            {
                var inventorySave = _inventoryModuleAssembler.Save(inventoryModule);
                save.InventorySave = inventorySave;
            }
            if (entity.TryGetModule<EquipmentModule>(out var equipmentModule))
            {
                var equipmentSave = _equipmentModuleAssembler.Save(equipmentModule);
                save.EquipmentSave = equipmentSave;
            }
            return save;
        }
    }
}
