using Assets._Game.Scripts.Items.Loot;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RewardModule : EntityModuleBase
    {
        public int Experience { get; }
        public LootTable LootTable { get; }

        public RewardModule(int experience, LootTable lootTable)
        {
            Experience = experience;
            LootTable = lootTable;
        }
    }
}
