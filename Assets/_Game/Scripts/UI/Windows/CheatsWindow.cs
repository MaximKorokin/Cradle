using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CheatsWindow : UIWindowBase
    {
        [SerializeField]
        private SelectableTabsController _cheatsTabsController;
        [SerializeField]
        private SimpleListView _cheatsTabContentTemplate;

        private SimpleListView _itemsListView;
        private SimpleListView _buffsListView;
        private SimpleListView _debuffsListView;

        private Dictionary<string, ItemDefinition> _itemDefinitions;
        private Dictionary<string, StatusEffectDefinition> _statusEffectDefinitions;

        public event Action<ItemDefinition> ItemDefinitionInfoClicked;
        public event Action<ItemDefinition> ItemDefinitionActionClicked;
        public event Action<StatusEffectDefinition> StatusEffectDefinitionClicked;

        public override void OnShow()
        {
            base.OnShow();

            _cheatsTabContentTemplate.gameObject.SetActive(false);
        }

        public void Render(CheatsHudData data)
        {
            _itemDefinitions = data.ItemDefinitions.ToDictionary(d => d.Id, d => d);
            _statusEffectDefinitions = data.StatusEffectDefinitions.ToDictionary(d => d.Id, d => d);

            // Items
            _itemsListView = Instantiate(_cheatsTabContentTemplate);
            _itemsListView.Render(data.ItemDefinitions.Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData("Items", _itemsListView.transform as RectTransform));
            _itemsListView.ElementInfoClicked += OnItemDefinitionInfoClicked;
            _itemsListView.ElementActionClicked += OnItemDefinitionActionClicked;

            // Buffs
            _buffsListView = Instantiate(_cheatsTabContentTemplate);
            _buffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Buff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData("Buffs", _buffsListView.transform as RectTransform));
            _buffsListView.ElementActionClicked += OnStatusEffectDefinitionClicked;

            // Debuffs
            _debuffsListView = Instantiate(_cheatsTabContentTemplate);
            _debuffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Debuff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData("Debuffs", _debuffsListView.transform as RectTransform));
            _debuffsListView.ElementActionClicked += OnStatusEffectDefinitionClicked;

            _cheatsTabsController.SelectTab(0);
        }

        public void Clear()
        {
            _itemsListView.ElementActionClicked -= OnItemDefinitionActionClicked;
            _itemsListView.ElementInfoClicked -= OnItemDefinitionInfoClicked;
            _buffsListView.ElementActionClicked -= OnStatusEffectDefinitionClicked;
            _debuffsListView.ElementActionClicked -= OnStatusEffectDefinitionClicked;

            _itemsListView.Clear();
            _buffsListView.Clear();
            _debuffsListView.Clear();

            _cheatsTabsController.ClearTabs();
        }

        private void OnItemDefinitionInfoClicked(string identifier)
        {
            if (_itemDefinitions.TryGetValue(identifier, out var itemDefinition))
            {
                ItemDefinitionInfoClicked?.Invoke(itemDefinition);
            }
        }

        private void OnItemDefinitionActionClicked(string identifier)
        {
            if (_itemDefinitions.TryGetValue(identifier, out var itemDefinition))
            {
                ItemDefinitionActionClicked?.Invoke(itemDefinition);
            }
        }

        private void OnStatusEffectDefinitionClicked(string identifier)
        {
            if (_statusEffectDefinitions.TryGetValue(identifier, out var statusEffectDefinition))
            {
                StatusEffectDefinitionClicked?.Invoke(statusEffectDefinition);
            }
        }
    }
}
