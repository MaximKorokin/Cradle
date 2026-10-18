using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Enchanting;
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
                .Select(m => new EnchantItem(
                    m.Item != null ? m.Item.Name : "Unknown",
                    m.Rules.Select(x => x.SuccessChance).ToArray(),
                    m.Rules.Select(x => x.Failure.Type).ToArray()))
                .ToArray() ?? Array.Empty<EnchantItem>();

            EnchantInstanceData enchantInstanceData = null;
            instanceData?.TryGet(out enchantInstanceData);

            var currentLevel = enchantInstanceData != null ? enchantInstanceData.Level : 0;
            var levelBonuses = itemEnchantingDefinition.Levels?
                .Select((level, i) => new EnchantLevelBonusDisplayData(
                    i + 1,
                    _statModifiersFormatter.FormatData(level.Bonuses),
                    currentLevel >= i + 1))
                .ToArray() ?? Array.Empty<EnchantLevelBonusDisplayData>();

            var overallStatModifiers = itemEnchantingDefinition.Levels
                .Take(enchantInstanceData?.Level ?? 0)
                .SelectMany(l => l.Bonuses)
                .GroupBy(m => (m.Stat, m.Stage, m.Operation, m.Priority))
                .Select(x => x.Aggregate((m1, m2) => m1 + m2))
                .ToArray();
            var overallStatModifiersText = _statModifiersFormatter.FormatData(overallStatModifiers);

            return new EnchantDisplayData(currentLevel, eligibleItems, overallStatModifiersText, levelBonuses);
        }
    }

    public readonly struct EnchantDisplayData
    {
        public readonly bool HasData;

        public readonly int CurrentLevel;
        public readonly EnchantItem[] EligibleItems;
        public readonly string OverallStatModifiersText;
        public readonly EnchantLevelBonusDisplayData[] LevelBonuses;

        public EnchantDisplayData(int currentLevel, EnchantItem[] eligibleItems, string overallStatModifiersText, EnchantLevelBonusDisplayData[] levelBonuses)
        {
            HasData = true;

            CurrentLevel = currentLevel;
            EligibleItems = eligibleItems;
            OverallStatModifiersText = overallStatModifiersText;
            LevelBonuses = levelBonuses;
        }
    }

    public readonly struct EnchantItem
    {
        public readonly bool HasData;

        public readonly string Name;
        public readonly float[] ProbabilityPerlevel;
        public readonly EnchantFailureType[] FailurePerLevel;

        public EnchantItem(string name, float[] probabilityPerlevel, EnchantFailureType[] failurePerLevel)
        {
            HasData = true;

            Name = name;
            ProbabilityPerlevel = probabilityPerlevel;
            FailurePerLevel = failurePerLevel;
        }
    }

    public readonly struct EnchantLevelBonusDisplayData
    {
        public readonly bool HasData;

        public readonly int Level;
        public readonly string BonusDescription;
        public readonly bool IsEnabled;

        public EnchantLevelBonusDisplayData(int level, string bonusDescription, bool isEnabled)
        {
            HasData = true;

            Level = level;
            BonusDescription = bonusDescription;
            IsEnabled = isEnabled;
        }
    }
}
