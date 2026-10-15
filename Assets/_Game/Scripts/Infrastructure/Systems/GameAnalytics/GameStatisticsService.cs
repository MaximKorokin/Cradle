using Assets._Game.Scripts.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics
{
    public sealed class GameStatisticsService : IDisposable
    {
        private readonly GameAnalyticsConfig _gameAnalyticsConfig;

        private readonly string _statisticsFilePath;
        private readonly Dictionary<int, StatisticsEntry> _statistics = new();

        public GameStatisticsService(
            GameAnalyticsConfig gameAnalyticsConfig)
        {
            _gameAnalyticsConfig = gameAnalyticsConfig;

            var dir = Path.Combine(Application.persistentDataPath, "Statistics");
            Directory.CreateDirectory(dir);
            _statisticsFilePath = Path.Combine(dir, _gameAnalyticsConfig.StatisticsPath);
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                if (!File.Exists(_statisticsFilePath)) return;
                var data = JsonUtility.FromJson<StatisticsContainer>(File.ReadAllText(_statisticsFilePath));
                if (data?.Entries == null) return;
                foreach (var entry in data.Entries)
                {
                    var key = GetEntryKey(entry);
                    _statistics[key] = entry;
                }
            }
            catch (Exception ex)
            {
                SLog.Exception(ex);
            }
        }

        public void SaveStatistics()
        {
            try
            {
                File.WriteAllText(_statisticsFilePath, JsonUtility.ToJson(new StatisticsContainer { Entries = new List<StatisticsEntry>(_statistics.Values) }, true));
            }
            catch (Exception ex)
            {
                SLog.Exception(ex);
            }
        }

        public void AddEntry(StatisticsEntryStep step)
        {
            var key = GetEntryStepKey(step);
            if (_statistics.TryGetValue(key, out var entry))
            {
                entry.Count++;
                entry.Total = step.Value;
            }
            else
            {
                entry = new(step)
                {
                    Count = 1,
                    Total = step.Value
                };
                _statistics[key] = entry;
            }
        }

        public void Dispose()
        {
            SaveStatistics();
        }

        private int GetEntryKey(StatisticsEntry entry) => GetKey(entry.Type, entry.Place, entry.Source, entry.Target);
        private int GetEntryStepKey(StatisticsEntryStep step) => GetKey(step.Type, step.Place, step.Source, step.Target);
        private int GetKey(AnalyticsEntryType type, string place, string source, string target) => HashCode.Combine(type, place, source, target);

        [Serializable]
        private sealed class StatisticsContainer
        {
            public List<StatisticsEntry> Entries;
        }

        [Serializable]
        public class StatisticsEntry
        {
            public AnalyticsEntryType Type;
            public string Place;
            public string Source;
            public string Target;
            // Amount of time action happened
            public long Count;
            // Total accumulated amount of actions value
            public double Total;

            public StatisticsEntry(StatisticsEntryStep step) : this(step.Type, step.Place, step.Source, step.Target) { }
            public StatisticsEntry(AnalyticsEntryType type, string place, string source, string target)
            {
                Type = type;
                Place = place;
                Source = source;
                Target = target;
            }
        }
    }

    public readonly struct StatisticsEntryStep
    {
        public readonly AnalyticsEntryType Type;
        public readonly string Place;
        public readonly string Source;
        public readonly string Target;
        public readonly float Value;

        public StatisticsEntryStep(AnalyticsEntryType type, string place, string source, string target, float value)
        {
            Type = type;
            Place = place;
            Source = source;
            Target = target;
            Value = value;
        }
    }
}
