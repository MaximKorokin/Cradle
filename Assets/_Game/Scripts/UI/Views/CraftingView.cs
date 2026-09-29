using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
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

        private SimpleListView _availableRecipesListView;
        private SimpleListView _unavailableRecipesListView;

        private Dictionary<string, CraftingRecipeDefinition> _recipeDefinitions;

        public event Action<CraftingRecipeDefinition> RecipeInfoClicked;
        public event Action<CraftingRecipeDefinition> RecipeActionClicked;

        protected override void Awake()
        {
            base.Awake();
            _craftingTabContentTemplate.gameObject.SetActive(false);
        }

        public override void OnShow()
        {
            base.OnShow();

            _craftingTabContentTemplate.gameObject.SetActive(false);
        }

        public override void OnHide()
        {
            base.OnHide();

            Clear();
        }

        protected override void Render(CraftingViewData data)
        {
            Clear();

            var currentTabIndex = _craftingTabsController.GetSelectedTabIndex();

            var availableRecipes = data.AvailableRecipes.ToArray();
            var unavailableRecipes = data.UnavailableRecipes.ToArray();

            _recipeDefinitions = availableRecipes.Concat(unavailableRecipes).ToDictionary(r => r.Id, r => r);

            // Available Recipes
            _availableRecipesListView = Instantiate(_craftingTabContentTemplate);
            _availableRecipesListView.Render(availableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Result.ItemDefinition.Icon,
                Text = FormatRecipeWithIngredients(r, true)
            }));
            _craftingTabsController.AddTab(new TabData("Available", _availableRecipesListView.transform as RectTransform));
            _availableRecipesListView.ElementInfoClicked += OnRecipeInfoClicked;
            _availableRecipesListView.ElementActionClicked += OnRecipeActionClicked;

            // Unavailable Recipes
            _unavailableRecipesListView = Instantiate(_craftingTabContentTemplate);
            _unavailableRecipesListView.Render(unavailableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Result.ItemDefinition.Icon,
                Text = FormatRecipeWithIngredients(r, false)
            }));
            _craftingTabsController.AddTab(new TabData("Unavailable", _unavailableRecipesListView.transform as RectTransform));
            _unavailableRecipesListView.ElementInfoClicked += OnRecipeInfoClicked;
            _unavailableRecipesListView.ElementActionClicked += OnRecipeActionClicked;

            _craftingTabsController.SelectTab(currentTabIndex);
        }

        private string FormatRecipeWithIngredients(CraftingRecipeDefinition recipe, bool isAvailable)
        {
            var sb = new StringBuilder();

            sb.Append($"<b>{recipe.Result.ItemDefinition.Name}</b> <size=80%>x{recipe.Result.Amount}</size>");
            if (!isAvailable)
            {
                sb.Append($" <color=#888888><size=80%>(Unavailable)</color>");
            }

            if (recipe.Ingredients.Length > 0)
            {
                sb.Append("\n<size=80%><color=#333333>");
                for (int i = 0; i < recipe.Ingredients.Length; i++)
                {
                    var ingredient = recipe.Ingredients[i];
                    if (i > 0)
                        sb.Append(", ");

                    sb.Append($"{ingredient.Item.Name} x{ingredient.Amount}");
                }
                sb.Append("</color></size>");
            }

            return sb.ToString();
        }

        private void Clear()
        {
            if (_availableRecipesListView != null)
            {
                _availableRecipesListView.ElementInfoClicked -= OnRecipeInfoClicked;
                _availableRecipesListView.ElementActionClicked -= OnRecipeActionClicked;
                _availableRecipesListView.Clear();
            }

            if (_unavailableRecipesListView != null)
            {
                _unavailableRecipesListView.ElementInfoClicked -= OnRecipeInfoClicked;
                _unavailableRecipesListView.ElementActionClicked -= OnRecipeActionClicked;
                _unavailableRecipesListView.Clear();
            }

            _craftingTabsController.ClearTabs();
        }

        private void OnRecipeInfoClicked(string recipeId)
        {
            if (_recipeDefinitions.TryGetValue(recipeId, out var recipe))
                RecipeInfoClicked?.Invoke(recipe);
        }

        private void OnRecipeActionClicked(string recipeId)
        {
            if (_recipeDefinitions.TryGetValue(recipeId, out var recipe))
                RecipeActionClicked?.Invoke(recipe);
        }
    }
}
