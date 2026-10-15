using Assets._Game.Scripts.Infrastructure.Configs;
using Assets._Game.Scripts.Infrastructure.Persistence.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Systems.GameAnalytics
{
    public sealed class GameLoggingService : IDisposable
    {
        private readonly GameAnalyticsConfig _gameAnalyticsConfig;

        private readonly string _logFilePath;
        private readonly List<LogEntry> _logEntries = new();

        public GameLoggingService(
            GameAnalyticsConfig gameAnalyticsConfig)
        {
            _gameAnalyticsConfig = gameAnalyticsConfig;

            var dir = Path.Combine(Application.persistentDataPath, "Logs");
            if (_gameAnalyticsConfig.ClearLogsOnStart && Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            _logFilePath = Path.Combine(dir, _gameAnalyticsConfig.LogsPath);
        }

        public void SaveLogs()
        {
            try
            {
                var stringBuilder = new StringBuilder();
                foreach (var entry in _logEntries)
                {
                    var line = entry.ToString();
                    stringBuilder.AppendLine(line);
                }
                File.AppendAllText(_logFilePath, stringBuilder.ToString());
                _logEntries.Clear();
            }
            catch (Exception ex)
            {
                SLog.Exception(ex);
            }
        }

        public void AddEntry(LogEntry entry)
        {
            _logEntries.Add(entry);
        }

        public void Dispose()
        {
            SaveLogs();
        }
    }

    public readonly struct LogEntry
    {
        public readonly DateTime UtcTime;
        public readonly float GameTime;

        public readonly AnalyticsEntryType Type;
        public readonly string Place;
        public readonly SerializableVector2 Position;
        public readonly string Source;
        public readonly string Target;
        public readonly float Value;
        public readonly string Details;

        public LogEntry(AnalyticsEntryType type, string place, SerializableVector2 position, string source, string target, float value, string details)
        {
            UtcTime = DateTime.UtcNow;
            GameTime = Time.time;

            Type = type;
            Place = place;
            Position = position;
            Source = source;
            Target = target;
            Value = value;
            Details = details;
        }

        public override readonly string ToString()
        {
            var line = string.Join("\t", new[]
            {
                UtcTime.ToString("o", CultureInfo.InvariantCulture),
                GameTime.ToString("F2", CultureInfo.InvariantCulture),
                Type.ToString(),
                Place,
                Position.ToString(),
                Source,
                Target,
                Value.ToString(CultureInfo.InvariantCulture),
                Details ?? ""
            });

            return line;
        }
    }
}
