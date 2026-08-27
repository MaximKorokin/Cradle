using Assets._Game.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public class ItemSetFormatter : IDataFormatter<ItemDefinition, ItemSetDisplayData>
    {
        private readonly StatModifiersFormatter _statModifiersFormatter;
        private readonly ItemSetDefinitionCatalog _itemSetCatalog;

        public ItemSetFormatter(
            StatModifiersFormatter statModifiersFormatter,
            ItemSetDefinitionCatalog itemSetCatalog)
        {
            _statModifiersFormatter = statModifiersFormatter;
            _itemSetCatalog = itemSetCatalog;
        }

        public ItemSetDisplayData FormatData(ItemDefinition data)
        {
            var set = _itemSetCatalog.GetSetByItem(data);
            if (set == null) return new();

            var setBonusEntries = new ItemSetBonusesEntry[set.Bonuses.Count];
            for (int i = 0; i < set.Bonuses.Count; i++)
            {
                var bonus = set.Bonuses[i];
                var modifiersText = _statModifiersFormatter.FormatData(bonus.Modifiers);
                setBonusEntries[i] = new ItemSetBonusesEntry(bonus.RequiredItemCount, modifiersText);
            }

            return new(set.DisplayName, set.SetItems.Select(item => item.Name).ToArray(), setBonusEntries);
        }
    }

    public readonly struct ItemSetDisplayData
    {
        public readonly bool HasData;

        public readonly string Name;
        public readonly string[] ItemNames;
        public readonly ItemSetBonusesEntry[] Bonuses;

        public ItemSetDisplayData(
            string name,
            string[] itemNames,
            ItemSetBonusesEntry[] bonuses)
        {
            HasData = true;

            Name = name;
            ItemNames = itemNames;
            Bonuses = bonuses;
        }
    }

    public readonly struct ItemSetBonusesEntry
    {
        public readonly int RequiredItemCount;
        public readonly string ModifiersText;

        public ItemSetBonusesEntry(int requiredItemCount, string modifiersText)
        {
            RequiredItemCount = requiredItemCount;
            ModifiersText = modifiersText;
        }
    }
}
