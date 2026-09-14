using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IEquipmentHudData : IItemContainerDataAggregator
    {
        EquipmentModel EquipmentModel { get; }
    }

    public class EquipmentHudData : ItemContainerDataAggregatorBase, IEquipmentHudData
    {
        private readonly EntityRepository _entityRepository;

        private EquipmentModule _equipmentModule;

        public EquipmentHudData(EntityRepository entityRepository, ItemContainerResolver itemContainerResolver) : base(itemContainerResolver)
        {
            _entityRepository = entityRepository;
        }

        public EquipmentModel EquipmentModel => _equipmentModule.Equipment;
        public ItemUseSettings ItemUseSettings => _equipmentModule.AutoItemUseSettings;
        
        public override void SetContainerEntity(string equipmentEntityId)
        {
            base.SetContainerEntity(equipmentEntityId);

            var entity = _entityRepository.Get(equipmentEntityId);
            _equipmentModule = entity.GetModule<EquipmentModule>();
        }

        protected override ItemContainerPath GetContainerPath(string entityId) => ItemContainerPath.Equipment(entityId);
    }
}
