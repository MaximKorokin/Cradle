using Assets._Game.Scripts.Items.Loot;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class KillRewardModule : EntityModuleBase
    {
        public long Experience { get; }
        public LootTable LootTable { get; }

        public KillRewardModule(int experience, LootTable lootTable)
        {
            Experience = experience;
            LootTable = lootTable;
        }
    }

    public sealed class KillRewardModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<KillRewardModuleDefinition>(out var moduleDefinition))
                return new KillRewardModule(moduleDefinition.Experience, moduleDefinition.LootTable);
            return null;
        }
    }
}
