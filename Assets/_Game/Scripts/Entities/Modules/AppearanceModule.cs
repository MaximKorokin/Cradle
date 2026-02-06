using Assets._Game.Scripts.Entities.Units;
using Assets.CoreScripts;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModule : EntityModuleBase
    {
        public UnitsController Units { get; }

        public AppearanceModule(UnitsController units)
        {
            Units = units;
        }

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

            // todo:  use entity visual model
            // todo:  use entity unit factory
            // todo:  set animations
            // todo:  remove unequipped item units
            var obj = new GameObject("Sword");
            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            var item = ieModule.Equipment.Get(e.Slot);
            spriteRenderer.sprite = item?.Definition.Sprite;
            Units.AddUnit(new(obj, $"HandRight/Sword", 1));
            // read equipment module, update visuals via Units (set sprites, enable/disable units, etc.)
        }

        //private void OnDirectionChanged(DirectionChanged e)
        //{
        //    Units.SetDirection(e.Right);
        //}
    }
}
