using Assets._Game.Scripts.Items.Loot;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RewardModule : EntityModuleBase
    {
        public long Experience { get; }
        public LootTable LootTable { get; }

        public RewardModule(int experience, LootTable lootTable)
        {
            Experience = experience;
            LootTable = lootTable;
        }
    }

    public sealed class RewardModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<RewardModuleDefinition>(out var moduleDefinition))
                return new RewardModule(moduleDefinition.Experience, moduleDefinition.LootTable);
            return null;
        }
    }
}
