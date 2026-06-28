namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class CraftingModule : EntityModuleBase
    {
        public CraftingModuleDefinition Definition { get; }

        public CraftingModule(CraftingModuleDefinition definition)
        {
            Definition = definition;
        }
    }

    public sealed class CraftingModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<CraftingModuleDefinition>(out var craftingModuleDefinition))
                return null;

            return new CraftingModule(craftingModuleDefinition);
        }
    }
}
