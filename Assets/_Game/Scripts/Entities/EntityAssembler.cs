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
        private readonly ActionModuleAssembler _actionModuleAssembler;
        private readonly FactionModuleFactory _factionModuleFactory;
        private readonly RewardModuleFactory _rewardModuleFactory;
        private readonly DespawnModuleFactory _despawnModuleFactory;
        private readonly WanderModuleFactory _wanderModuleFactory;
        private readonly LevelingModuleAssembler _levelingModuleAssembler;

        public EntityAssembler(
            EntityRepository entityRepository,
            InventoryEquipmentModuleAssembler inventoryEquipmentControllerAssembler,
            AppearanceModuleFactory appearanceModuleFactory,
            StatsModuleAssembler statsModuleAssembler,
            StatusEffectModuleAssembler statusEffectModuleAssembler,
            EntityPositioningFactory entityPositioningFactory,
            ActionModuleAssembler actionModuleAssembler,
            FactionModuleFactory factionModuleFactory,
            RewardModuleFactory rewardModuleFactory,
            DespawnModuleFactory despawnModuleFactory,
            WanderModuleFactory wanderModuleFactory,
            LevelingModuleAssembler levelingModuleAssembler)
        {
            _entityRepository = entityRepository;
            _inventoryEquipmentControllerAssembler = inventoryEquipmentControllerAssembler;
            _appearanceModuleFactory = appearanceModuleFactory;
            _statsModuleAssembler = statsModuleAssembler;
            _statusEffectModuleAssembler = statusEffectModuleAssembler;
            _entityPositioningFactory = entityPositioningFactory;
            _actionModuleAssembler = actionModuleAssembler;
            _factionModuleFactory = factionModuleFactory;
            _rewardModuleFactory = rewardModuleFactory;
            _despawnModuleFactory = despawnModuleFactory;
            _wanderModuleFactory = wanderModuleFactory;
            _levelingModuleAssembler = levelingModuleAssembler;
        }

        public Entity Create(EntityDefinition entityDefinition)
        {
            var entity = new Entity(entityDefinition);

            entity.AddModule(new RestrictionStateModule());

            entity.AddModule(_statsModuleAssembler.Create(entityDefinition));

            entity.AddModule(_statusEffectModuleAssembler.Assemble());

            entity.AddModule(_appearanceModuleFactory.Create(entityDefinition.VisualModel));

            entity.AddModule(_inventoryEquipmentControllerAssembler.Create(entityDefinition));

            entity.AddModule(_actionModuleAssembler.Create(entityDefinition));

            entity.AddModule(_factionModuleFactory.Create(entityDefinition));

            entity.AddModule(_rewardModuleFactory.Create(entityDefinition));

            entity.AddModule(_despawnModuleFactory.Create(entityDefinition));

            entity.AddModule(_wanderModuleFactory.Create(entityDefinition));

            entity.AddModule(_levelingModuleAssembler.Create(entityDefinition));

            foreach (var module in _entityPositioningFactory.Create(entityDefinition))
            {
                entity.AddModule(module);
            }

            PostCreateActions(entity);

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

        private void PostCreateActions(Entity entity)
        {
            entity.Subscribe<EntityBoundEvent>(e =>
            {
                if (!entity.TryGetModule<WanderModule>(out var wanderModule)) return;
                wanderModule.AnchorPoint = entity.GetModule<SpatialModule>().Position;
            });
        }
    }
}
