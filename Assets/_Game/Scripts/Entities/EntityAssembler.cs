using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Modules.Positioning;
using Assets._Game.Scripts.Infrastructure.Persistence;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private readonly EntityRepository _entityRepository;
        private readonly InventoryEquipmentModuleAssembler _inventoryEquipmentControllerAssembler;
        private readonly AppearanceModuleFactory _appearanceModuleFactory;
        private readonly StatsModuleAssembler _statsModuleAssembler;
        private readonly StatusEffectModuleAssembler _statusEffectModuleAssembler;
        private readonly EntityPositioningFactory _entityPositioningFactory;

        public EntityAssembler(
            EntityRepository entityRepository,
            InventoryEquipmentModuleAssembler inventoryEquipmentControllerAssembler,
            AppearanceModuleFactory appearanceModuleFactory,
            StatsModuleAssembler statsModuleAssembler,
            StatusEffectModuleAssembler statusEffectModuleAssembler,
            EntityPositioningFactory entityPositioningFactory)
        {
            _entityRepository = entityRepository;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _appearanceModuleFactory = appearanceModuleFactory;
            _statsModuleAssembler = statsModuleAssembler;
            _statusEffectModuleAssembler = statusEffectModuleAssembler;
            _entityPositioningFactory = entityPositioningFactory;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entity = new Entity(entityDefinition);

            entity.AddModule(_statsModuleAssembler.Create(entityDefinition));

            entity.AddModule(_statusEffectModuleAssembler.Assemble());

            entity.AddModule(_appearanceModuleFactory.Create(entityDefinition.VisualModel));

            entity.AddModule(_inventoryEquipmentControllerAssembler.Create(entityDefinition));

            foreach (var module in _entityPositioningFactory.Create(entityDefinition))
            {
                entity.AddModule(module);
            }

            _entityRepository.Add(entity);
            return entity;
        }

        public void Apply(Entity entity, EntitySave save)
        {
            if (entity.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                _inventoryEquipmentControllerAssembler.Apply(inventoryEquipmentModule, save);
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
