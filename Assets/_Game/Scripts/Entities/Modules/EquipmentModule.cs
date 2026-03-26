using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EquipmentModule : EntityModuleBase
    {
        public EquipmentModel Equipment { get; private set; }

        public EquipmentModule(EquipmentModel equipment)
        {
            Equipment = equipment;

            if (Equipment != null) Equipment.EquipmentChanged += OnEquipmentSlotChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (Equipment != null) Equipment.EquipmentChanged -= OnEquipmentSlotChanged;
        }

        private void OnEquipmentSlotChanged(EquipmentChange equipmentChange)
        {
            Publish(new EquipmentChangedEvent(Entity, equipmentChange));
        }
    }

    public readonly struct EquipmentChangedEvent : IEntityEvent
    {
        public readonly EquipmentSlotKey Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly EquipmentChangeKind Kind;

        public Entity Entity { get; }

        public EquipmentChangedEvent(Entity entity, EquipmentSlotKey slot, ItemStackSnapshot? item, EquipmentChangeKind kind)
        {
            Entity = entity;
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public EquipmentChangedEvent(Entity entity, EquipmentChange equipmentChange)
        {
            Entity = entity;
            Slot = equipmentChange.Slot;
            Item = equipmentChange.Item;
            Kind = equipmentChange.Kind;
        }
    }

    public class EquipmentModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly EquipmentModelAssembler _equipmentModelAssembler;

        public EquipmentModuleFactory(EquipmentModelAssembler equipmentModelAssembler)
        {
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            EquipmentModel equipmentModel = null;
            if (entityDefinition.TryGetModuleDefinition<EquipmentModuleDefinition>(out var equipmentDefinitionModule))
            {
                var slots = equipmentDefinitionModule.EquipmentSlots.ToArray();
                equipmentModel = _equipmentModelAssembler.Create(slots);
            }

            return new EquipmentModule(equipmentModel);
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;
            _equipmentModelAssembler.Apply(equipmentModule.Equipment, entitySave.EquipmentSave);
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;
            entitySave.EquipmentSave = _equipmentModelAssembler.Save(equipmentModule.Equipment);
        }
    }
}
