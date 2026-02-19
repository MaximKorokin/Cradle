using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets.CoreScripts;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private readonly EntityRepository _entityRepository;
        private readonly InventoryEquipmentModuleAssembler _inventoryEquipmentControllerAssembler;
        private readonly AppearanceModuleFactory _appearanceModuleFactory;
        private readonly StatsModuleAssembler _statsModuleAssembler;
        private readonly StatusEffectModuleAssembler _statusEffectModuleAssembler;

        public EntityAssembler(
            EntityRepository entityRepository,
            InventoryEquipmentModuleAssembler inventoryEquipmentControllerAssembler,
            AppearanceModuleFactory appearanceModuleFactory,
            StatsModuleAssembler statsModuleAssembler,
            StatusEffectModuleAssembler statusEffectModuleAssembler)
        {
            _entityRepository = entityRepository;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _appearanceModuleFactory = appearanceModuleFactory;
            _statsModuleAssembler = statsModuleAssembler;
            _statusEffectModuleAssembler = statusEffectModuleAssembler;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entity = new Entity(entityDefinition);
            _entityRepository.Add(entity);

            var statsModule = _statsModuleAssembler.Create(entityDefinition);
            entity.AddModule(statsModule);
            entity.AddModule(new DerivedStatsApplierModule(statsModule));
            entity.AddModule(new EquipmentStatsApplierModule(statsModule));
            entity.AddModule(new InventoryStatsApplierModule(statsModule));
            entity.AddModule(new StatusEffectsStatsApplierModule(statsModule));

            entity.AddModule(_statusEffectModuleAssembler.Assemble());

            var appearanceModule = _appearanceModuleFactory.Create(entityDefinition.VisualModel);
            entity.AddModule(appearanceModule);
            entity.AddModule(new EquipmentAppearanceApplierModule(appearanceModule, entityDefinition.VisualModel));

            //entity.AddModule(new BehaviourController());
            entity.AddModule(_inventoryEquipmentControllerAssembler.Create(entityDefinition));

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
