using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Items.Crafting
{
    [CreateAssetMenu(menuName = "Definitions/CraftingRecipe")]
    public class CraftingRecipeDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public RecipeIngredient[] Ingredients { get; private set; } = Array.Empty<RecipeIngredient>();
        [field: SerializeField]
        public RecipeResult Result { get; private set; }
    }

    [Serializable]
    public struct RecipeIngredient
    {
        [field: SerializeField]
        public ItemDefinition Item { get; private set; }
        [field: SerializeField]
        public int Amount { get; private set; }
    }

    [Serializable]
    public struct RecipeResult
    {
        [field: SerializeField]
        public ItemDefinition Item { get; private set; }
        [field: SerializeField]
        public int Amount { get; private set; }
    }
}
