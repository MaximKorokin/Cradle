using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    [CreateAssetMenu(menuName = "Definitions/ItemSet")]
    public class ItemSetDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        private ItemDefinition[] _setItems;
        public IReadOnlyList<ItemDefinition> SetItems => _setItems;

        [field: SerializeField]
        private ItemSetBonus[] _bonuses;
        public IReadOnlyList<ItemSetBonus> Bonuses => _bonuses;

        public IEnumerable<ItemSetBonus> GetItemsBonuses(IEnumerable<ItemStackSnapshot> items)
        {
            var count = 0;

            foreach (var item in items)
            {
                for (int i = 0; i < _setItems.Length; i++)
                {
                    if (_setItems[i].Id == item.Definition.Id)
                    {
                        count += item.Amount;
                        break;
                    }
                }
            }

            return GetBonuses(count);
        }

        public IEnumerable<ItemSetBonus> GetBonuses(int itemCount)
        {
            for (int i = 0; i < _bonuses.Length; i++)
            {
                if (itemCount >= _bonuses[i].RequiredItemCount)
                {
                    yield return _bonuses[i];
                }
            }
        }
    }

    [Serializable]
    public struct ItemSetBonus
    {
        [field: SerializeField]
        public int RequiredItemCount { get; private set; }

        [field: SerializeField]
        private StatModifier[] _modifiers;
        public readonly IReadOnlyList<StatModifier> Modifiers => _modifiers;

        public ItemSetBonus(int requiredItemCount, StatModifier[] modifiers = null)
        {
            RequiredItemCount = requiredItemCount;
            _modifiers = modifiers ?? Array.Empty<StatModifier>();
        }
    }
}
