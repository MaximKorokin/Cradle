using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class CheatsWindow : UIWindowBase
    {
        private const string ControlTabId = "Control_id";
        private const string ItemsTabId = "Items_id";
        private const string BuffsTabId = "Buffs_id";
        private const string DebuffsTabId = "Debuffs_id";
        private const string QuestsTabId = "Quests_id";

        [SerializeField]
        private SelectableTabsController _cheatsTabsController;
        [SerializeField]
        private SimpleListView _cheatsTabContentTemplate;

        [field: SerializeField]
        public GameControlView GameControlTabContent { get; private set; }

        private SimpleListView _itemsListView;
        private SimpleListView _buffsListView;
        private SimpleListView _debuffsListView;
        private SimpleListView _questsListView;

        private Dictionary<string, ItemDefinition> _itemDefinitions;
        private Dictionary<string, StatusEffectDefinition> _statusEffectDefinitions;
        private Dictionary<string, QuestDefinition> _questDefinitions;

        public event Action<ItemDefinition> ItemDefinitionInfoClicked;
        public event Action<ItemDefinition> ItemDefinitionActionClicked;
        public event Action<StatusEffectDefinition> StatusEffectDefinitionClicked;
        public event Action<QuestDefinition> QuestDefinitionClicked;

        public override void OnShow()
        {
            base.OnShow();

            _cheatsTabContentTemplate.gameObject.SetActive(false);
        }

        public void Render(CheatsHudData data)
        {
            // Control tab
            _cheatsTabsController.AddTab(new TabData(ControlTabId, "Control", GameControlTabContent.transform as RectTransform));

            _itemDefinitions = data.ItemDefinitions.ToDictionary(d => d.Id, d => d);
            _statusEffectDefinitions = data.StatusEffectDefinitions.ToDictionary(d => d.Id, d => d);
            _questDefinitions = data.QuestDefinitions.ToDictionary(d => d.Id, d => d);

            // Items tab
            _itemsListView = Instantiate(_cheatsTabContentTemplate);
            _itemsListView.Render(data.ItemDefinitions.Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData(ItemsTabId, "Items", _itemsListView.transform as RectTransform));
            _itemsListView.ElementInfoClicked += OnItemDefinitionInfoClicked;
            _itemsListView.ElementActionClicked += OnItemDefinitionActionClicked;

            // Buffs tab
            _buffsListView = Instantiate(_cheatsTabContentTemplate);
            _buffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Buff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData(BuffsTabId, "Buffs", _buffsListView.transform as RectTransform));
            _buffsListView.ElementActionClicked += OnStatusEffectDefinitionClicked;

            // Debuffs tab
            _debuffsListView = Instantiate(_cheatsTabContentTemplate);
            _debuffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Debuff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _cheatsTabsController.AddTab(new TabData(DebuffsTabId, "Debuffs", _debuffsListView.transform as RectTransform));
            _debuffsListView.ElementActionClicked += OnStatusEffectDefinitionClicked;

            // Quests tab
            _questsListView = Instantiate(_cheatsTabContentTemplate);
            _questsListView.Render(data.QuestDefinitions.Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = null,
                Text = d.Title
            }));
            _cheatsTabsController.AddTab(new TabData(QuestsTabId, "Quests", _questsListView.transform as RectTransform));
            _questsListView.ElementActionClicked += OnQuestDefinitionClicked;

            _cheatsTabsController.SelectTab(0);
        }

        public void Clear()
        {
            _itemsListView.ElementActionClicked -= OnItemDefinitionActionClicked;
            _itemsListView.ElementInfoClicked -= OnItemDefinitionInfoClicked;
            _buffsListView.ElementActionClicked -= OnStatusEffectDefinitionClicked;
            _debuffsListView.ElementActionClicked -= OnStatusEffectDefinitionClicked;
            _questsListView.ElementActionClicked -= OnQuestDefinitionClicked;

            _itemsListView.Clear();
            _buffsListView.Clear();
            _debuffsListView.Clear();
            _questsListView.Clear();

            _cheatsTabsController.RemoveTab(ItemsTabId);
            _cheatsTabsController.RemoveTab(BuffsTabId);
            _cheatsTabsController.RemoveTab(DebuffsTabId);
            _cheatsTabsController.RemoveTab(QuestsTabId);
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

        private void OnQuestDefinitionClicked(string identifier)
        {
            if (_questDefinitions.TryGetValue(identifier, out var questDefinition))
            {
                QuestDefinitionClicked?.Invoke(questDefinition);
            }
        }
    }
}

