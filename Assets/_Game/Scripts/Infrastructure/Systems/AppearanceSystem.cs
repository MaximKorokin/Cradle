using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AppearanceSystem : ReactiveEntitySystemBase
    {
        public AppearanceSystem(EntityRepository repository) : base(repository)
        {
        }

        protected override bool Filter(Entity entity)
        {
            return entity.HasModule<InventoryEquipmentModule>();
        }

        protected override void OnTrack(Entity entity)
        {
            entity.Subscribe<EquipmentChangedEvent>(OnEquipmentChnaged);
        }

        protected override void OnUntrack(Entity entity)
        {
            entity.Unsubscribe<EquipmentChangedEvent>(OnEquipmentChnaged);
        }

        private void OnEquipmentChnaged(EquipmentChangedEvent equipEvent)
        {
        }
    }
}
