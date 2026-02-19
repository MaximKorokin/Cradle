using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EquipmentAppearanceApplierModule : EntityModuleBase
    {
        private readonly AppearanceModule _appearance;
        private readonly EntityVisualModel _entityVisualModel;

        public EquipmentAppearanceApplierModule(AppearanceModule appearance, EntityVisualModel entityVisualModel)
        {
            _appearance = appearance;
            _entityVisualModel = entityVisualModel;
        }

        protected override void OnAttach()
        {
            Subscribe<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        private void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            // Change units
            foreach (var entityUnitVisualModel in _entityVisualModel.Units.Where(u => u.EquipmentSlots.Contains(e.Slot.SlotType)))
            {
                var path = $"{entityUnitVisualModel.Path}/{e.Slot.SlotType}";
                if (e.Kind == EquipmentChangeKind.Equipped)
                {
                    var relativeOrderInLayer = e.Slot.SlotType == EquipmentSlotType.Weapon ? -1 : 1;
                    _appearance.RequestEnsureUnit(path, relativeOrderInLayer);
                    _appearance.RequestSetUnitSprite(path, e.Item.Value.Definition.Sprite);
                }
                else if (e.Kind == EquipmentChangeKind.Unequipped)
                {
                    _appearance.RequestRemoveUnit(path);
                }
            }
            _appearance.RequestUpdateOrderInLayer();

            // Change animations
            foreach (var animationTrait in e.Item.Value.GetTraits<AnimationOverrideTrait>())
            {
                var animationClip = e.Kind == EquipmentChangeKind.Unequipped ? null : animationTrait.AnimationClip;
                _appearance.RequestSetAnimation(animationTrait.AnimationKey, animationClip);
            }
        }
    }
}
