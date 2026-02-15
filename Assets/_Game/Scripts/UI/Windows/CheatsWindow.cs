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
        private SimpleListView _itemsListView;
        [SerializeField]
        private SimpleListView _buffsListView;
        [SerializeField]
        private SimpleListView _debuffsListView;

        private Dictionary<object, ItemDefinition> _itemDefinitions;
        private Dictionary<object, StatusEffectDefinition> _statusEffectDefinitions;

        public event Action<ItemDefinition> ItemDefinitionClicked;
        public event Action<StatusEffectDefinition> StatusEffectDefinitionClicked;

        public void Render(CheatsHudData data)
        {
            _itemDefinitions = data.ItemDefinitions.ToDictionary(d => (object)d.Id, d => d);
            _itemsListView.Render(data.ItemDefinitions.Select(d => new SimpleListItemDefinition()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _itemsListView.ElementClicked += OnItemDefinitionClicked;

            _statusEffectDefinitions = data.StatusEffectDefinitions.ToDictionary(d => (object)d.Id, d => d);
            _buffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Buff).Select(d => new SimpleListItemDefinition()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _buffsListView.ElementClicked += OnStatusEffectDefinitionClicked;

            _debuffsListView.Render(data.StatusEffectDefinitions.Where(d => d.Category == StatusEffectCategory.Debuff).Select(d => new SimpleListItemDefinition()
            {
                Identifier = d.Id,
                Sprite = d.Icon,
                Text = d.Name
            }));
            _debuffsListView.ElementClicked += OnStatusEffectDefinitionClicked;
        }

        public void Clear()
        {
            _itemsListView.ElementClicked -= OnItemDefinitionClicked;
            _buffsListView.ElementClicked -= OnStatusEffectDefinitionClicked;
            _debuffsListView.ElementClicked -= OnStatusEffectDefinitionClicked;
            _itemsListView.Clear();
        }

        private void OnItemDefinitionClicked(object obj)
        {
            if (_itemDefinitions.TryGetValue(obj, out var itemDefinition))
            {
                ItemDefinitionClicked?.Invoke(itemDefinition);
            }
        }

        private void OnStatusEffectDefinitionClicked(object obj)
        {
            if (_statusEffectDefinitions.TryGetValue(obj, out var statusEffectDefinition))
            {
                StatusEffectDefinitionClicked?.Invoke(statusEffectDefinition);
            }
        }
    }
}
