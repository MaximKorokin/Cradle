using Assets._Game.Scripts.Infrastructure.Persistence;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System;
using System.Linq;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class EquipmentModule : EntityModuleBase
    {
        public EquipmentModel Equipment { get; private set; }
        public ItemUseSettings AutoItemUseSettings { get; private set; }
        public ItemUseSettings ManualItemUseSettings { get; private set; }

        public EquipmentModule(EquipmentModel equipment, ItemUseSettings manualItemUseSettings)
        {
            Equipment = equipment;
            ManualItemUseSettings = manualItemUseSettings;

            if (Equipment != null) Equipment.EquipmentChanged += OnEquipmentSlotChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (Equipment != null) Equipment.EquipmentChanged -= OnEquipmentSlotChanged;
        }

        public void SetAutoItemUseSettings(ItemUseSettings settings)
        {
            AutoItemUseSettings = settings;
        }

        private void OnEquipmentSlotChanged(EquipmentChange equipmentChange)
        {
            Publish(new EquipmentChangedEvent(equipmentChange));
        }
    }

    [Serializable]
    public struct ItemUseSettings
    {
        public int HpPercent;
        public bool OverrideStatusEffects;

        public ItemUseSettings(int hpPercent, bool overrideStatusEffect)
        {
            HpPercent = hpPercent;
            OverrideStatusEffects = overrideStatusEffect;
        }
    }

    public readonly struct EquipmentChangedEvent : IEntityEvent
    {
        public readonly EquipmentSlotKey Slot;
        public readonly ItemStackSnapshot? Item;
        public readonly EquipmentChangeKind Kind;

        public EquipmentChangedEvent(EquipmentSlotKey slot, ItemStackSnapshot? item, EquipmentChangeKind kind)
        {
            Slot = slot;
            Item = item;
            Kind = kind;
        }

        public EquipmentChangedEvent(EquipmentChange equipmentChange)
        {
            Slot = equipmentChange.Slot;
            Item = equipmentChange.Item;
            Kind = equipmentChange.Kind;
        }
    }

    public class EquipmentModuleFactory : IEntityModuleFactory, IEntityModulePersistance
    {
        private readonly EquipmentModelFactory _equipmentModelAssembler;

        public EquipmentModuleFactory(EquipmentModelFactory equipmentModelAssembler)
        {
            _equipmentModelAssembler = equipmentModelAssembler;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<EquipmentModuleDefinition>(out var equipmentDefinitionModule))
                return null;

            var slots = equipmentDefinitionModule.EquipmentSlots.ToArray();
            var equipmentModel = _equipmentModelAssembler.Create(slots);
            return new EquipmentModule(equipmentModel, equipmentDefinitionModule.ManualItemUseSettings);
        }

        public void Apply(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;
            _equipmentModelAssembler.Apply(equipmentModule.Equipment, entitySave.EquipmentSave);
            equipmentModule.SetAutoItemUseSettings(entitySave.EquipmentSave.AutoItemUseSettings);
        }

        public void Save(Entity entity, EntitySave entitySave)
        {
            if (!entity.TryGetModule<EquipmentModule>(out var equipmentModule)) return;
            entitySave.EquipmentSave = _equipmentModelAssembler.Save(equipmentModule.Equipment);
            entitySave.EquipmentSave.AutoItemUseSettings = equipmentModule.AutoItemUseSettings;
        }
    }
}
