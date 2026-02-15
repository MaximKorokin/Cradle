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

    public class EquipmentHudData : IEquipmentHudData, IDisposable
    {
        private readonly EquipmentModel _equipmentModel;

        public EquipmentHudData(PlayerContext playerContext)
        {
            _equipmentModel = playerContext.IEModule.Equipment;
            _equipmentModel.Changed += OnEquipmentChanged;
            OnEquipmentChanged();
        }

        public EquipmentModel EquipmentModel => _equipmentModel;

        public event Action Changed;

        private void OnEquipmentChanged()
        {
            Changed?.Invoke();
        }

        public void Dispose()
        {
            _equipmentModel.Changed -= OnEquipmentChanged;
        }
    }
}
