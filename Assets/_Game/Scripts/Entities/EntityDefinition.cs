using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Storage;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Loot;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    [CreateAssetMenu(menuName = "Definitions/Entity")]
    public class EntityDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public EntityVisualModel VisualModel { get; private set; }
        [field: SerializeField]
        public string VariantName { get; private set; }

        [SerializeReference]
        [SerializeField]
        private EntityModuleDefinition[] _modules = Array.Empty<EntityModuleDefinition>();

        public EntityModuleDefinition[] Modules => _modules;

        public bool TryGetModuleDefinition<T>(out T module) where T : EntityModuleDefinition
        {
            module = _modules.FirstOrDefault(m => m is T) as T;
            return module != null;
        }
    }

    [Serializable]
    public abstract class EntityModuleDefinition
    {
        public bool Enabled = true;
    }

    public class InventoryModuleDefinition : EntityModuleDefinition
    {
        public int SlotsAmount;
    }

    public class EquipmentModuleDefinition : EntityModuleDefinition
    {
        public EquipmentSlotType[] EquipmentSlots;
    }

    public class StatsModuleDefinition : EntityModuleDefinition
    {
        public StatsDefinition Stats;
    }

    public class StatusEffectModuleDefinition : EntityModuleDefinition
    {

    }

    public class ControlModuleDefinition : EntityModuleDefinition
    {
        [SerializeReference]
        public ControlProviderData ControlProvider;
    }

    public class ActionModuleDefinition : EntityModuleDefinition
    {
        public SpecialActionDefinition[] SpecialActions;
        public ActionDefinition[] Actions;
    }

    [Serializable]
    public struct SpecialActionDefinition
    {
        public SpecialActionKind Kind;
        public ActionDefinition Action;
    }

    public class FactionModuleDefinition : EntityModuleDefinition
    {
        public Faction.Faction Faction;
    }

    public class RewardModuleDefinition : EntityModuleDefinition
    {
        public int PreferredLevel;
        public int Experience;
        public LootTable LootTable;
    }

    public class DespawnModuleDefinition : EntityModuleDefinition
    {
        public float DespawnDelay;
        public DespawnCounterStartTrigger Trigger;
    }

    public enum DespawnCounterStartTrigger
    {
        OnSpawn, OnDeath
    }

    public class WanderBehaviourModuleDefinition : EntityModuleDefinition
    {
        public float MinIdleTime;
        public float MaxIdleTime;
    }

    public class LevelingModuleDefinition : EntityModuleDefinition
    {
    }
}
