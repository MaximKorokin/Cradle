namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class RewardModuleFactory
    {
        public RewardModule Create(EntityDefinition entityDefinition)
        {
            if (entityDefinition.TryGetModuleDefinition<RewardModuleDefinition>(out var moduleDefinition))
                return new RewardModule(moduleDefinition.Experience, moduleDefinition.LootTable);
            return null;
        }
    }
}
