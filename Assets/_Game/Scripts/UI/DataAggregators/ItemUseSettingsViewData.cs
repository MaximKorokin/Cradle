using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public sealed class ItemUseSettingsViewData : EntityBoundDataAggregatorBase
    {
        private EquipmentModule _equipmentModule;
        public ItemUseSettings ItemUseSettings => _equipmentModule.AutoItemUseSettings;

        public ItemUseSettingsViewData(EntityRepository entityRepository) : base(entityRepository)
        {
        }

        protected override void OnBoundEntityChanged()
        {
            _equipmentModule = Entity.GetModule<EquipmentModule>();
        }
    }
}
