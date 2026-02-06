using System.Linq;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Items.Equipment;
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
            if (!Entity.TryGetModule<EntityInventoryEquipmentModule>(out var ieModule))
            {
                SLog.Warn($"{nameof(EquipmentChanged)} event triggered but no {nameof(EntityInventoryEquipmentModule)} is attached");
                return;
            }

            if (ieModule.Equipment == null) return;

            // todo:  set animations

            foreach (var entityUnitVisualModel in _entityVisualModel.Units.Where(u => u.EquipmentSlots.Contains(e.Slot.SlotType)))
            {
                var path = $"{entityUnitVisualModel.Path}/{e.Slot.SlotType}";
                var item = ieModule.Equipment.Get(e.Slot);
                // item equipped
                if (item != null)
                {
                    // todo: check that unit exists
                    var relativeOrderInLayer = e.Slot.SlotType == EquipmentSlotType.Weapon ? -1 : 1;
                    var unit = _entityUnitFactory.Create(path, item.Value.Definition.Sprite, relativeOrderInLayer);
                    Units.AddUnit(unit);
                }
                // item unequipped
                else
                {
                    Units.RemoveUnit(path);
                }
            }

            Units.UpdateOrderInLayer();

            // read equipment module, update visuals via Units (set sprites, enable/disable units, etc.)
        }

        //private void OnDirectionChanged(DirectionChanged e)
        //{
        //    Units.SetDirection(e.Right);
        //}
    }
}
