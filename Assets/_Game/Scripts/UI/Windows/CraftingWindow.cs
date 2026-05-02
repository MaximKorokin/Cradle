using Assets._Game.Scripts.Items.Crafting;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CraftingWindow : UIWindowBase
    {
        [SerializeField]
        private SelectableTabsController _craftingTabsController;
        [SerializeField]
        private SimpleListView _craftingTabContentTemplate;

        private SimpleListView _availableRecipesListView;
        private SimpleListView _unavailableRecipesListView;

        private Dictionary<string, CraftingRecipeDefinition> _recipeDefinitions;

        public event Action<CraftingRecipeDefinition> RecipeClicked;

        public override void OnShow()
        {
            base.OnShow();

            _craftingTabContentTemplate.gameObject.SetActive(false);
        }

        public void Render(CraftingHudData data)
        {
            var availableRecipes = data.AvailableRecipes.ToArray();
            var unavailableRecipes = data.UnavailableRecipes.ToArray();

            _recipeDefinitions = availableRecipes.Concat(unavailableRecipes).ToDictionary(r => r.Id, r => r);

            // Available Recipes
            _availableRecipesListView = Instantiate(_craftingTabContentTemplate);
            _availableRecipesListView.Render(availableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Icon,
                Text = r.Name
            }));
            _craftingTabsController.AddTab(new TabData("Available", _availableRecipesListView.transform as RectTransform));
            _availableRecipesListView.ElementClicked += OnRecipeClicked;

            // Unavailable Recipes
            _unavailableRecipesListView = Instantiate(_craftingTabContentTemplate);
            _unavailableRecipesListView.Render(unavailableRecipes.Select(r => new SimpleListItemData()
            {
                Identifier = r.Id,
                Sprite = r.Icon,
                Text = $"{r.Name} (Unavailable)"
            }));
            _craftingTabsController.AddTab(new TabData("Unavailable", _unavailableRecipesListView.transform as RectTransform));
            _unavailableRecipesListView.ElementClicked += OnRecipeClicked;

            _craftingTabsController.SelectTab(0);
        }

        public void Clear()
        {
            _availableRecipesListView.ElementClicked -= OnRecipeClicked;
            _unavailableRecipesListView.ElementClicked -= OnRecipeClicked;

            _availableRecipesListView.Clear();
            _unavailableRecipesListView.Clear();

            _craftingTabsController.ClearTabs();
        }

        private void OnRecipeClicked(string identifier)
        {
            if (_recipeDefinitions.TryGetValue(identifier, out var recipeDefinition))
            {
                RecipeClicked?.Invoke(recipeDefinition);
            }
        }
    }
}
