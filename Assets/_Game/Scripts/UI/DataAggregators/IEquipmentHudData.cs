using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IEquipmentHudData
    {
        EquipmentModel EquipmentModel { get; }

        event Action Changed;
    }

    public class EquipmentHudData : DataAggregatorBase, IEquipmentHudData
    {
        private readonly EquipmentModule _equipmentModule;

        public EquipmentHudData(PlayerContext playerContext)
        {
            _equipmentModule = playerContext.GetModule<EquipmentModule>();
            _equipmentModule.Equipment.Changed += OnEquipmentChanged;
            OnEquipmentChanged();
        }

        public EquipmentModel EquipmentModel => _equipmentModule.Equipment;
        public ItemUseSettings ItemUseSettings => _equipmentModule.AutoItemUseSettings;

        public event Action Changed;

        private void OnEquipmentChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
            _equipmentModule.Equipment.Changed -= OnEquipmentChanged;
        }
    }
}
