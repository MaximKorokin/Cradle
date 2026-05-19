using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IEquipmentHudData
    {
        EquipmentModel EquipmentModel { get; }

        event Action Changed;

        void SetEquipmentEntity(string equipmentEntityId);
    }

    public class EquipmentHudData : DataAggregatorBase, IEquipmentHudData
    {
        private readonly EntityRepository _entityRepository;

        private EquipmentModule _equipmentModule;

        public EquipmentHudData(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public EquipmentModel EquipmentModel => _equipmentModule.Equipment;
        public ItemUseSettings ItemUseSettings => _equipmentModule.AutoItemUseSettings;

        public event Action Changed;

        public void SetEquipmentEntity(string equipmentEntityId)
        {
            var entity = _entityRepository.Get(equipmentEntityId);
            var equipmentModule = entity.GetModule<EquipmentModule>();
            if (_equipmentModule != equipmentModule)
            {
                if (_equipmentModule != null)
                {
                    _equipmentModule.Equipment.Changed -= OnEquipmentChanged;
                }
                _equipmentModule = equipmentModule;
                if (_equipmentModule != null)
                {
                    _equipmentModule.Equipment.Changed += OnEquipmentChanged;
                }
                OnEquipmentChanged();
            }
        }

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
