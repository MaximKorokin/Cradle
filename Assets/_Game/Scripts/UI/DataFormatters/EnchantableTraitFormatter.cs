using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using System;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataFormatters
{
    public sealed class EnchantableTraitFormatter : IDataFormatter<(ItemDefinition, IItemInstanceData), EnchantDisplayData>
    {
        private readonly StatModifiersFormatter _statModifiersFormatter;

        public EnchantableTraitFormatter(StatModifiersFormatter statModifiersFormatter)
        {
            _statModifiersFormatter = statModifiersFormatter;
        }

        public EnchantDisplayData FormatData((ItemDefinition, IItemInstanceData) data)
        {
            var (itemDefinition, instanceData) = data;
            if (itemDefinition == null)
                return new EnchantDisplayData();

            if (!itemDefinition.TryGetTrait<EnchantableTrait>(out var enchantableTrait))
                return new EnchantDisplayData();

            var itemEnchantingDefinition = enchantableTrait.ItemEnchantingDefinition;
            if (itemEnchantingDefinition == null)
                return new EnchantDisplayData();

            var eligibleItems = itemEnchantingDefinition.Methods?
                .Select(m => m.Item != null ? m.Item.Name : "Unknown")
                .ToArray() ?? Array.Empty<string>();

            EnchantInstanceData enchantInstanceData = null;
            instanceData?.TryGet(out enchantInstanceData);

            var levelBonuses = itemEnchantingDefinition.Levels?
                .Select((level, i) => new EnchantLevelBonusDisplayData(
                    i + 1,
                    _statModifiersFormatter.FormatData(level.Bonuses), 
                    enchantInstanceData != null && enchantInstanceData.Level >= i + 1))
                .ToArray() ?? Array.Empty<EnchantLevelBonusDisplayData>();

            var overallStatModifiers = itemEnchantingDefinition.Levels
                .Take(enchantInstanceData?.Level ?? 0)
                .SelectMany(l => l.Bonuses)
                .GroupBy(m => (m.Stat, m.Stage, m.Operation, m.Priority))
                .Select(x => x.Aggregate((m1, m2) => m1 + m2))
                .ToArray();
            var overallStatModifiersText = _statModifiersFormatter.FormatData(overallStatModifiers);

            return new EnchantDisplayData(eligibleItems, overallStatModifiersText, levelBonuses);
        }
    }

    public readonly struct EnchantDisplayData
    {
        public readonly bool HasData;

        public readonly string[] EligibleItems;
        public readonly string OverallStatModifiersText;
        public readonly EnchantLevelBonusDisplayData[] LevelBonuses;

        public EnchantDisplayData(string[] eligibleItems, string overallStatModifiersText, EnchantLevelBonusDisplayData[] levelBonuses)
        {
            HasData = true;
            EligibleItems = eligibleItems;
            OverallStatModifiersText = overallStatModifiersText;
            LevelBonuses = levelBonuses;
        }
    }

    public readonly struct EnchantLevelBonusDisplayData
    {
        public readonly int Level;
        public readonly string BonusDescription;
        public readonly bool IsEnabled;

        public EnchantLevelBonusDisplayData(int level, string bonusDescription, bool isEnabled)
        {
            Level = level;
            BonusDescription = bonusDescription;
            IsEnabled = isEnabled;
        }
    }
}
