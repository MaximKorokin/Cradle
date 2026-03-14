using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AppearanceSystem : EntitySystemBase
    {
        private readonly IGlobalEventBus _globalEventBus;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled,
                new[] { typeof(AppearanceModule) }
            );

        public AppearanceSystem(EntityRepository repository, IGlobalEventBus globalEventBus) : base(repository)
        {
            _globalEventBus = globalEventBus;
            _globalEventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            _globalEventBus.Unsubscribe<EntityDiedEvent>(OnEntityDied);
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
            appearance.RequestUpdateOrderInLayer();

            // Change animations
            foreach (var animationTrait in e.Item.Value.GetTraits<AnimationOverrideTrait>())
            {
                var animationClip = e.Kind == EquipmentChangeKind.Unequipped ? null : animationTrait.AnimationClip;
                appearance.RequestSetAnimation(animationTrait.AnimationKey, animationClip);
            }
        }

        private void OnEntityDied(EntityDiedEvent e)
        {
            var appearance = e.Victim.GetModule<AppearanceModule>();
            appearance.RequestSetAnimatorValue(EntityAnimatorParameterName.ToDie, true);
        }
    }
}
