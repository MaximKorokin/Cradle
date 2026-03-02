using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AppearanceSystem : ReactiveEntitySystemBase
    {
        public AppearanceSystem(EntityRepository repository) : base(repository)
        {
        }

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<AppearanceModule>();
        }

        protected override void OnTrack(Entity entity)
        {
            entity.Subscribe<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        protected override void OnUntrack(Entity entity)
        {
            entity.Unsubscribe<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        private void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            e.Entity.TryGetModule(out AppearanceModule appearance);

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
    }
}
