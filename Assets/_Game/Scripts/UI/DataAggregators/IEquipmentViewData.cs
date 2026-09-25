using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IEquipmentViewData : IItemContainerDataAggregator
    {
        EquipmentModel EquipmentModel { get; }
    }

    public class EquipmentViewData : ItemContainerDataAggregatorBase, IEquipmentViewData
    {
        private EquipmentModule _equipmentModule;

        public EquipmentViewData(
            ItemContainerResolver itemContainerResolver,
            EntityRepository entityRepository) : base(itemContainerResolver, entityRepository) { }

        public EquipmentModel EquipmentModel => _equipmentModule.Equipment;
        public ItemUseSettings ItemUseSettings => _equipmentModule.AutoItemUseSettings;

        protected override void OnBoundEntityChanged(string equipmentEntityId)
        {
            _equipmentModule = Entity.GetModule<EquipmentModule>();

            base.OnBoundEntityChanged(equipmentEntityId);
        }

        protected override ItemContainerPath GetContainerPath(string entityId) => ItemContainerPath.Equipment(entityId);
    }
}
