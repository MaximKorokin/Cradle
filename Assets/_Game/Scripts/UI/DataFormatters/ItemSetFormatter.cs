using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public class ItemSetFormatter : IDataFormatter<(ItemDefinition, EquipmentModel), ItemSetDisplayData>
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

        public ItemSetDisplayData FormatData((ItemDefinition, EquipmentModel) data)
        {
            var (itemDefinition, equipmentModel) = data;

            var set = _itemSetCatalog.GetSetByItem(itemDefinition);
            if (set == null) return new();

            var setItems = set.SetItems.Select(item => new ItemSetItemDisplayData(item.Name, false)).ToArray();
            if (equipmentModel != null)
            {
                foreach (var (_, snapshot) in equipmentModel.Enumerate())
                {
                    if (!snapshot.HasValue) continue;
                    var index = Array.FindIndex(setItems, item => item.Name == snapshot.Value.Definition.Name);
                    if (index >= 0)
                    {
                        setItems[index] = new ItemSetItemDisplayData(snapshot.Value.Definition.Name, true);
                    }
                }
            }

            var equippedItemCount = setItems.Count(item => item.IsEquipped);
            var setBonuses = set.Bonuses
                .Select(bonus => new ItemSetBonusesDisplayData(
                    bonus.RequiredItemCount,
                    _statModifiersFormatter.FormatData(bonus.Modifiers),
                    equippedItemCount >= bonus.RequiredItemCount))
                .OrderBy(bonus => bonus.RequiredItemCount)
                .ToArray();

            return new(set.DisplayName, setItems, setBonuses);
        }
    }

    public readonly struct ItemSetDisplayData
    {
        public readonly bool HasData;

        public readonly string Name;
        public readonly ItemSetItemDisplayData[] Items;
        public readonly ItemSetBonusesDisplayData[] Bonuses;

        public ItemSetDisplayData(
            string name,
            ItemSetItemDisplayData[] items,
            ItemSetBonusesDisplayData[] bonuses)
        {
            HasData = true;

            Name = name;
            Items = items;
            Bonuses = bonuses;
        }
    }

    public readonly struct ItemSetBonusesDisplayData
    {
        public readonly int RequiredItemCount;
        public readonly string ModifiersText;
        public readonly bool IsEnabled;

        public ItemSetBonusesDisplayData(int requiredItemCount, string modifiersText, bool isEnabled)
        {
            RequiredItemCount = requiredItemCount;
            ModifiersText = modifiersText;
            IsEnabled = isEnabled;
        }
    }

    public readonly struct ItemSetItemDisplayData
    {
        public readonly string Name;
        public readonly bool IsEquipped;

        public ItemSetItemDisplayData(string name, bool isEquipped)
        {
            Name = name;
            IsEquipped = isEquipped;
        }
    }
}
