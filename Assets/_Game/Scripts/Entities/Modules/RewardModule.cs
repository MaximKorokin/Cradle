using Assets._Game.Scripts.Items.Loot;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RewardModule : EntityModuleBase
    {
        public int PreferredLevel { get; }
        public long Experience { get; }
        public LootTable LootTable { get; }

        public RewardModule(int preferredLevel, int experience, LootTable lootTable)
        {
            PreferredLevel = preferredLevel;
            Experience = experience;
            LootTable = lootTable;
        }
    }

    public sealed class RewardModuleFactory
    {
        public RewardModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<RewardModuleDefinition>(out var moduleDefinition))
                return new RewardModule(moduleDefinition.PreferredLevel, moduleDefinition.Experience, moduleDefinition.LootTable);
            return null;
        }
    }
}
