using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AppearanceSystem : EntitySystemBase, ITickSystem
    {
        private readonly EquipmentVisualHandler _equipmentVisualHandler;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled,
                new[] { typeof(AppearanceModule), typeof(StatModule) }
            );

        public AppearanceSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository repository) : base(globalEventBus, repository)
        {
            _equipmentVisualHandler = new EquipmentVisualHandler();
            TrackGlobalEvent<EntityDiedEvent>(OnEntityDied);
            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
            TrackEntityEvent<StatChangedEvent>(OnStatChanged);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            // Set initial visuals when the entity view created
            entity.SubscribeOnce<EntityBoundEvent>(e =>
            {
                var appearance = entity.GetModule<AppearanceModule>();
                var stats = entity.GetModule<StatModule>();
                // Set initial walk speed multiplier
                appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.WalkSpeedMultiplier, stats.Stats.Get(StatId.MoveSpeed));
                // Set initial scale
                appearance.RequestSetScale(stats.Stats.Get(StatId.SizeScale));

                if (entity.TryGetModule<EquipmentModule>(out var equipmentModule))
                {
                    // Set initial equipment visuals
                    foreach (var (slot, item) in equipmentModule.Equipment.Enumerate())
                    {
                        if (item == null) continue;
                        var equipmentChangedEvent = new EquipmentChangedEvent(slot, item.Value, EquipmentChangeKind.Equipped);
                        OnEquipmentChanged(entity, equipmentChangedEvent);
                    }
                }
            });
        }

        private void OnEquipmentChanged(Entity entity, EquipmentChangedEvent e)
        {
            if (e.Item == null)
            {
                SLog.Warn($"Received {nameof(EquipmentChangedEvent)} for entity {entity.Definition.Id} with null item. Slot: {e.Slot.SlotType}, Kind: {e.Kind}");
                return;
            }

            // Handle equipment visuals
            _equipmentVisualHandler.RefreshEquipmentVisuals(entity);

            var appearance = entity.GetModule<AppearanceModule>();
            
            // Check for animation overrides
            var context = new ItemTriggerContext(entity, ItemTrigger.OnEquipmentChange, e.Item.Value);
            foreach (var animationOverrideTrait in e.Item.Value.GetFunctionalTraits<AnimationOverrideTrait>(ItemTrigger.OnEquipmentChange))
            {
                if (animationOverrideTrait == null || !animationOverrideTrait.CanTrigger(context)) continue;

                // Change animations
                foreach (var animationOverride in animationOverrideTrait.AnimationOverrideProfile.AnimationOverrides)
                {
                    var animationClip = e.Kind == EquipmentChangeKind.Unequipped ? null : animationOverride.AnimationClip;
                    appearance.RequestSetAnimation(animationOverride.AnimationKey, animationClip);
                }
            }
        }

        private void OnStatChanged(Entity entity, StatChangedEvent e)
        {
            var stats = entity.GetModule<StatModule>();
            var appearance = entity.GetModule<AppearanceModule>();

            // sync the walk speed stat to the animator
            if (e.StatId == StatId.MoveSpeed)
            {
                appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.WalkSpeedMultiplier, stats.Stats.Get(StatId.MoveSpeed));
            }
            // sync the scale stat to visual scale
            else if (e.StatId == StatId.SizeScale)
            {
                appearance.RequestSetScale(stats.Stats.Get(StatId.SizeScale));
            }
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            var appearance = e.Victim.GetModule<AppearanceModule>();
            appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.ToDie, true);
        }

        public void Tick(float delta)
        {
            IterateAllEntities(entity => TickEntity(entity, delta));
        }

        private void TickEntity(Entity entity, float delta)
        {
            if (entity.TryGetModule<AppearanceModule>(out var appearanceModule))
                appearanceModule.RequestUpdateOrderInLayer();
        }
    }
}
