using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Crafting;

namespace Assets._Game.Scripts.Infrastructure.Services
{
    public sealed class CraftingService
    {
        private readonly ItemStackFactory _itemStackFactory;

        public CraftingService(ItemStackFactory itemStackFactory)
        {
            _itemStackFactory = itemStackFactory;
        }

        public bool CanCraft(CraftingRecipeDefinition recipe, int craftCount, InventoryModule inventoryModule)
        {
            if (recipe == null || inventoryModule == null || craftCount <= 0)
                return false;

            // Check if we have enough ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var totalRequired = ingredient.Amount * craftCount;
                if (inventoryModule.Inventory.Count(key) < totalRequired)
                {
                    return false;
                }
            }

            // Check if there's enough space for the result
            var totalResultAmount = recipe.Result.Amount * craftCount;
            var resultSnapshot = _itemStackFactory.Create(recipe.Result.ItemDefinition.Id, totalResultAmount).Snapshot;
            var canAddAmount = inventoryModule.Inventory.PreviewAdd(resultSnapshot);

            if (canAddAmount < totalResultAmount)
            {
                return false;
            }

            return true;
        }

        public bool CanCraftAny(CraftingRecipeDefinition recipe, InventoryModule inventoryModule)
        {
            return CanCraft(recipe, 1, inventoryModule);
        }

        public int CalculateMaxCraftable(CraftingRecipeDefinition recipe, InventoryModule inventoryModule)
        {
            if (recipe == null || inventoryModule == null)
                return 0;

            if (recipe.Ingredients.Length == 0)
                return int.MaxValue;

            int maxCraftable = int.MaxValue;

            // Check ingredient availability
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var available = inventoryModule.Inventory.Count(key);
                int possibleCrafts = available / ingredient.Amount;
                maxCraftable = System.Math.Min(maxCraftable, possibleCrafts);
            }

            // Check inventory space constraint
            // We need to check how many times we can add the result to the inventory
            var spaceBasedMax = CalculateMaxCraftableBySpace(recipe, inventoryModule);
            maxCraftable = System.Math.Min(maxCraftable, spaceBasedMax);

            return maxCraftable;
        }

        private int CalculateMaxCraftableBySpace(CraftingRecipeDefinition recipe, InventoryModule inventoryModule)
        {
            var resultItem = recipe.Result.ItemDefinition;
            var resultAmountPerCraft = recipe.Result.Amount;
            var maxStackSize = resultItem.MaxAmount;
            var resultKey = ItemKey.From(resultItem, null);

            // Calculate space in existing stacks of the same item
            int spaceInExistingStacks = 0;
            int emptySlots = 0;

            foreach (var (_, snapshot) in inventoryModule.Inventory.Enumerate())
            {
                if (snapshot == null)
                {
                    emptySlots++;
                }
                else if (snapshot.Value.Key.Equals(resultKey))
                {
                    // Calculate available space in this stack
                    int currentAmount = snapshot.Value.Amount;
                    int availableSpace = maxStackSize - currentAmount;
                    spaceInExistingStacks += availableSpace;
                }
            }

            // Total space = space in existing stacks + space in empty slots
            int totalAvailableSpace = spaceInExistingStacks + (emptySlots * maxStackSize);

            // Calculate how many times we can craft based on available space
            int maxCraftsBySpace = totalAvailableSpace / resultAmountPerCraft;

            return maxCraftsBySpace;
        }

        public void Craft(CraftingRecipeDefinition recipe, int craftCount, InventoryModule inventoryModule)
        {
            if (recipe == null || inventoryModule == null || craftCount <= 0)
                return;

            if (!CanCraft(recipe, craftCount, inventoryModule))
                return;

            // Remove ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                var key = ItemKey.From(ingredient.Item, null);
                var totalToRemove = ingredient.Amount * craftCount;
                inventoryModule.Inventory.Remove(key, totalToRemove);
            }

            // Add result
            var totalResultAmount = recipe.Result.Amount * craftCount;
            var resultSnapshot = _itemStackFactory.Create(recipe.Result.ItemDefinition.Id, totalResultAmount).Snapshot;
            inventoryModule.Inventory.Add(resultSnapshot);
        }
    }
}
