using Assets._Game.Scripts.Entities.Control;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Loot;
using Assets._Game.Scripts.Items.Shop;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.Shared;
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
        public string DisplayName { get; private set; }

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

    public class PersistenceModuleDefinition : EntityModuleDefinition
    {
        public string PersistenceKey;
    }

    public class StorageModuleDefinition : EntityModuleDefinition
    {
        public int SlotsAmount;
        public float Radius;
    }

    public class InventoryModuleDefinition : EntityModuleDefinition
    {
        public int SlotsAmount;
    }

    public class EquipmentModuleDefinition : EntityModuleDefinition
    {
        public EquipmentSlotType[] EquipmentSlots;
        public ItemUseSettings ManualItemUseSettings;
        public ItemDefinition[] DefaultItems;
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

    public class KillRewardModuleDefinition : EntityModuleDefinition
    {
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
        public float WanderRadius;
    }

    public class LevelingModuleDefinition : EntityModuleDefinition
    {
        public int Level;
    }

    public sealed class ShopModuleDefinition : EntityModuleDefinition
    {
        [field: SerializeField]
        public ShopDefinition ShopDefinition { get; private set; }
        [field: SerializeField]
        public float Radius { get; private set; }
    }

    public sealed class QuestModuleDefinition : EntityModuleDefinition
    {
        [field: SerializeField]
        public QuestDefinition[] InitialQuests { get; private set; }
    }

    public sealed class QuestGiverModuleDefinition : EntityModuleDefinition
    {
        [field: SerializeField]
        public QuestDefinition[] OfferedQuests { get; private set; }
        [field: SerializeField]
        public float Radius { get; private set; }
    }

    public sealed class CraftingModuleDefinition : EntityModuleDefinition
    {
        [field: SerializeField]
        public string CrafterName { get; private set; }
        [field: SerializeField]
        public float Radius { get; private set; }
    }

    public sealed class CreatureModuleDefinition : EntityModuleDefinition
    {
    }
}
