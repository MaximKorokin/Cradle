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
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AppearanceSystem : EntitySystemBase, ITickSystem
    {
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled,
                new[] { typeof(AppearanceModule), typeof(StatModule) }
            );

        public AppearanceSystem(EntityRepository repository, IGlobalEventBus globalEventBus) : base(repository)
        {
            _globalEventBus = globalEventBus;
            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
            TrackEntityEvent<StatChangedEvent>(OnStatChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            entity.SubscribeOnce<EntityBoundEvent>(e =>
            {
                if (!EntityQuery.Match(entity)) return;

                var appearance = entity.GetModule<AppearanceModule>();
                var stats = entity.GetModule<StatModule>();
                // Set initial walk speed multiplier
                appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.WalkSpeedMultiplier, stats.Stats.Get(StatId.MoveSpeed));
                // Set initial scale
                appearance.RequestSetScale(stats.Stats.Get(StatId.SizeScale));
            });
        }

        private void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            var appearance = e.Entity.GetModule<AppearanceModule>();

            // Change units
            foreach (var entityUnitVisualModel in e.Entity.Definition.VisualModel.Units.Where(u => u.EquipmentSlots.Contains(e.Slot.SlotType)))
            {
                var path = $"{entityUnitVisualModel.Path}/{e.Slot.SlotType}";
                if (e.Kind == EquipmentChangeKind.Equipped)
                {
                    var relativeOrderInLayer = e.Slot.SlotType == EquipmentSlotType.Weapon ? -1 : 1;
                    appearance.RequestEnsureUnit(path, relativeOrderInLayer);
                    appearance.RequestSetUnitSprite(path, e.Item.Value.Definition.Sprite);
                }
                else if (e.Kind == EquipmentChangeKind.Unequipped)
                {
                    appearance.RequestRemoveUnit(path);
                }
            }

            // Check for animation overrides
            var animationOverrideTrait = e.Item.Value.GetFunctionalTrait<AnimationOverrideTrait>(ItemTrigger.OnEquipmentChange);
            if (animationOverrideTrait == null || !animationOverrideTrait.CanTrigger(new(e.Entity, ItemTrigger.OnEquipmentChange, e.Item.Value)))
                return;

            // Change animations
            foreach (var animationOverride in animationOverrideTrait.AnimationOverrideProfile.AnimationOverrides)
            {
                var animationClip = e.Kind == EquipmentChangeKind.Unequipped ? null : animationOverride.AnimationClip;
                appearance.RequestSetAnimation(animationOverride.AnimationKey, animationClip);
            }
        }

        private void OnStatChanged(StatChangedEvent e)
        {
            var stats = e.Entity.GetModule<StatModule>();
            var appearance = e.Entity.GetModule<AppearanceModule>();

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
            IterateMatchingEntities(entity => TickEntity(entity, delta));
        }

        private void TickEntity(Entity entity, float delta)
        {
            var appearance = entity.GetModule<AppearanceModule>();
            appearance.RequestUpdateOrderInLayer();
        }
    }
}
