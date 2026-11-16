using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CraftingView : UIViewBase<CraftingViewData>
    {
        [SerializeField]
        private SelectableTabsController _craftingTabsController;
        [SerializeField]
        private SimpleListView _craftingTabContentTemplate;
        [SerializeField]
        private SimpleListItemView _availableTemplate;
        [SerializeField]
        private SimpleListItemView _unavailableTemplate;

        private TabbedListViewCollection _tabbedListViews;

        private Dictionary<string, CraftingRecipeDefinition> _recipeDefinitions;

        public event Action<CraftingRecipeDefinition> RecipeActionClicked;

        protected override void Awake()
        {
            base.Awake();
            _availableTemplate.gameObject.SetActive(false);
            _unavailableTemplate.gameObject.SetActive(false);
            _craftingTabContentTemplate.gameObject.SetActive(false);
            _tabbedListViews = new TabbedListViewCollection(_craftingTabsController, _craftingTabContentTemplate);
        }

        protected override void Render(CraftingViewData data)
        {
            var currentTabIndex = _craftingTabsController.GetSelectedTabIndex();
            Clear();

            var availableRecipes = data.AvailableRecipes.ToArray();
            var unavailableRecipes = data.UnavailableRecipes.ToArray();

            _recipeDefinitions = availableRecipes.Concat(unavailableRecipes).ToDictionary(r => r.Id, r => r);

            // Available Recipes
            _tabbedListViews.AddTab("Available", "Available", availableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Result.ItemDefinition.Icon,
                TextFactory = templateText => FormatRecipe(r, templateText),
                ItemCreationCallback = v => OnCraftingListItemCreated(v, data, r)
            }),
            null,
            OnRecipeActionClicked,
            _ => _availableTemplate);

            // Unavailable Recipes
            _tabbedListViews.AddTab("Unavailable", "Unavailable", unavailableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Result.ItemDefinition.Icon,
                TextFactory = templateText => FormatRecipe(r, templateText),
                ItemCreationCallback = v => OnCraftingListItemCreated(v, data, r)
            }),
            null,
            OnRecipeActionClicked,
            _ => _unavailableTemplate);

            _craftingTabsController.SelectTab(currentTabIndex);
        }

        private string FormatRecipe(CraftingRecipeDefinition recipe, string templateText)
        {
            var itemName = recipe.Result.ItemDefinition.Name;
            var itemAmount = recipe.Result.Amount;
            var ingredients = string.Join(", ", recipe.Ingredients.Select(i => $"{i.Item.Name} x{i.Amount}"));

            return templateText.FormatSafe(new string[] { itemName, itemAmount.ToString(), ingredients });
        }

        private void OnCraftingListItemCreated(SimpleListItemView view, CraftingViewData viewData, CraftingRecipeDefinition recipe)
        {
            var itemWidget = view.GetComponentInChildren<ItemStackWidget>();
            itemWidget.Render(viewData.GetItemForRecipe(recipe));
        }

        private void Clear()
        {
            _tabbedListViews.Clear();
        }

        private void OnRecipeActionClicked(string recipeId)
        {
            if (_recipeDefinitions.TryGetValue(recipeId, out var recipe))
                RecipeActionClicked?.Invoke(recipe);
        }
    }
}
