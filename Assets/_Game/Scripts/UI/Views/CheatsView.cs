using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CheatsView : UIViewBase<CheatsViewData>
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
        public GameControlWidget GameControlTabContent { get; private set; }

        private TabbedListViewCollection _tabbedListViews;

        private Dictionary<string, ItemDefinition> _itemDefinitions;
        private Dictionary<string, StatusEffectDefinition> _statusEffectDefinitions;
        private Dictionary<string, QuestDefinition> _questDefinitions;

        public event Action<ItemDefinition> ItemDefinitionInfoClicked;
        public event Action<ItemDefinition> ItemDefinitionActionClicked;
        public event Action<StatusEffectDefinition> StatusEffectDefinitionClicked;
        public event Action<QuestDefinition> QuestDefinitionClicked;

        protected override void Awake()
        {
            base.Awake();
            _cheatsTabContentTemplate.gameObject.SetActive(false);
            _tabbedListViews = new TabbedListViewCollection(_cheatsTabsController, _cheatsTabContentTemplate);
        }

        protected override void Render(CheatsViewData data)
        {
            Clear();

            _itemDefinitions = data.ItemDefinitions.ToDictionary(d => d.Id, d => d);
            _statusEffectDefinitions = data.StatusEffectDefinitions.ToDictionary(d => d.Id, d => d);
            _questDefinitions = data.QuestDefinitions.ToDictionary(d => d.Id, d => d);

            // Control tab
            _cheatsTabsController.AddTab(new TabData(ControlTabId, "Control", GameControlTabContent.transform as RectTransform));

            // Items tab
            _tabbedListViews.AddTab(ItemsTabId, "Items", data.ItemDefinitions.Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }), OnItemDefinitionInfoClicked, OnItemDefinitionActionClicked);

            // Buffs tab
            _tabbedListViews.AddTab(BuffsTabId, "Buffs", data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Buff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }), actionClicked: OnStatusEffectDefinitionClicked);

            // Debuffs tab
            _tabbedListViews.AddTab(DebuffsTabId, "Debuffs", data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Debuff).Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }), actionClicked: OnStatusEffectDefinitionClicked);

            // Quests tab
            _tabbedListViews.AddTab(QuestsTabId, "Quests", data.QuestDefinitions.Select(d => new SimpleListItemData()
            {
                Identifier = d.Id,
                Sprite = null,
                Text = d.Title
            }), actionClicked: OnQuestDefinitionClicked);
        }

        private void Clear()
        {
            _tabbedListViews.Clear();
        }

        private void OnItemDefinitionInfoClicked(string itemId)
        {
            if (_itemDefinitions.TryGetValue(itemId, out var itemDef))
                ItemDefinitionInfoClicked?.Invoke(itemDef);
        }

        private void OnItemDefinitionActionClicked(string itemId)
        {
            if (_itemDefinitions.TryGetValue(itemId, out var itemDef))
                ItemDefinitionActionClicked?.Invoke(itemDef);
        }

        private void OnStatusEffectDefinitionClicked(string statusEffectId)
        {
            if (_statusEffectDefinitions.TryGetValue(statusEffectId, out var statusEffectDef))
                StatusEffectDefinitionClicked?.Invoke(statusEffectDef);
        }

        private void OnQuestDefinitionClicked(string questId)
        {
            if (_questDefinitions.TryGetValue(questId, out var questDef))
                QuestDefinitionClicked?.Invoke(questDef);
        }
    }
}
