using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets.CoreScripts;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Entities
{
    public class EntityAssembler
    {
        private static int _entitiesCounter = 0;

        private readonly IObjectResolver _resolver;
        private readonly InventoryEquipmentModuleAssembler _inventoryEquipmentControllerAssembler;
        private readonly AppearanceModuleFactory _appearanceModuleFactory;
        private readonly StatsModuleAssembler _statsModuleAssembler;
        private readonly StatusEffectModuleAssembler _statusEffectModuleAssembler;

        public EntityAssembler(
            IObjectResolver resolver,
            InventoryEquipmentModuleAssembler inventoryEquipmentControllerAssembler,
            AppearanceModuleFactory appearanceModuleFactory,
            StatsModuleAssembler statsModuleAssembler,
            StatusEffectModuleAssembler statusEffectModuleAssembler)
        {
            _resolver = resolver;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _appearanceModuleFactory = appearanceModuleFactory;
            _statsModuleAssembler = statsModuleAssembler;
            _statusEffectModuleAssembler = statusEffectModuleAssembler;
        }

        public Entity Assemble(EntityDefinition entityDefinition, EntitySave save)
        {
            var entity = Create(entityDefinition);
            if (entity == null)
            {
                SLog.Error($"Cannot create entity with id: {entityDefinition.VisualModel}");
                return null;
            }
            if (entity.TryGetModule<InventoryEquipmentModule>(out var inventoryEquipmentModule))
            {
                _inventoryEquipmentControllerAssembler.Apply(inventoryEquipmentModule, save);
            }
            return entity;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entityView = _resolver.Instantiate(entityDefinition.VisualModel.BasePrefab);
            entityView.name = $"{entityDefinition.VisualModel} ({++_entitiesCounter})";

            var entity = new Entity("");

            var statsModule = _statsModuleAssembler.Create(entityDefinition);
            entity.AddModule(statsModule);
            entity.AddModule(new DerivedStatsApplierModule(statsModule));
            entity.AddModule(new EquipmentStatsApplierModule(statsModule));
            entity.AddModule(new InventoryStatsApplierModule(statsModule));
            entity.AddModule(new StatusEffectsStatsApplierModule(statsModule));

            entity.AddModule(_statusEffectModuleAssembler.Assemble());

            var appearanceModule = _appearanceModuleFactory.Create(entityView, entityDefinition);
            entity.AddModule(appearanceModule);
            entity.AddModule(new EquipmentAppearanceApplierModule(appearanceModule, entityDefinition.VisualModel));

            //entity.AddModule(new BehaviourController());
            entity.AddModule(_inventoryEquipmentControllerAssembler.Create(entityDefinition));

            return entity;
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
