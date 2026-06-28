using Assets._Game.Scripts.Items.Crafting;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class CraftingModule : EntityModuleBase
    {
        public IEnumerable<CraftingRecipeDefinition> Recipes { get; }

        public CraftingModule(CraftingRecipeDefinition[] recipes)
        {
            Recipes = recipes ?? Array.Empty<CraftingRecipeDefinition>();
        }
    }

    public sealed class CraftingModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<CraftingModuleDefinition>(out var craftingModuleDefinition))
                return null;

            return new CraftingModule(craftingModuleDefinition.Recipes);
        }
    }
}
