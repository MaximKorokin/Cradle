using System.Linq;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets.CoreScripts;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EntityAppearanceModule : EntityModuleBase
    {
        private readonly EntityUnitFactory _entityUnitFactory;
        private readonly EntityVisualModel _entityVisualModel;

        public EntityAppearanceModule(EntityVisualModel entityVisualModel, EntityUnitsController units, EntityUnitFactory entityUnitFactory)
        {
            Units = units;
            _entityVisualModel = entityVisualModel;
            _entityUnitFactory = entityUnitFactory;
        }

        public EntityUnitsController Units { get; }

        protected override void OnAttach()
        {
            base.OnAttach();

            Subscribe<EquipmentChanged>(OnEquipmentChanged);
            //Subscribe<DirectionChanged>(OnDirectionChanged);
            //Subscribe<PositionChanged>(...) UpdateOrderInLayer
        }

        private void OnEquipmentChanged(EquipmentChanged e)
        {
            // Change units
            foreach (var entityUnitVisualModel in _entityVisualModel.Units.Where(u => u.EquipmentSlots.Contains(e.Slot.SlotType)))
            {
                var path = $"{entityUnitVisualModel.Path}/{e.Slot.SlotType}";
                if (e.Kind == EquipmentChangeKind.Equipped)
                {
                    // todo: check that unit exists
                    var relativeOrderInLayer = e.Slot.SlotType == EquipmentSlotType.Weapon ? -1 : 1;
                    var unit = _entityUnitFactory.Create(path, e.Item.Value.Definition.Sprite, relativeOrderInLayer);
                    Units.AddUnit(unit);
                }
                else if (e.Kind == EquipmentChangeKind.Unequipped)
                {
                    Units.RemoveUnit(path);
                }
            }
            Units.UpdateOrderInLayer();

            // Change animations
            foreach (var animationTrait in e.Item.Value.Definition.Traits.Select(t => t as AnimationOverrideTrait).Where(t => t != null))
            {
                var animationClip = e.Kind == EquipmentChangeKind.Unequipped ? null : animationTrait.AnimationClip;
                Units.AnimatorController.SetAnimation(animationTrait.AnimationKey, animationClip);
            }
        }

        //private void OnDirectionChanged(DirectionChanged e)
        //{
        //    Units.SetDirection(e.Right);
        //}
    }
}
