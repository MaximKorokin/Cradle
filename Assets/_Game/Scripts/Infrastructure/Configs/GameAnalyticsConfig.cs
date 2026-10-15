using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "GameAnalyticsConfig", menuName = "Configs/GameAnalyticsConfig")]
    public sealed class GameAnalyticsConfig : ScriptableObject
    {
        [field:Header("Logs")]
        [field: SerializeField]
        public bool ClearLogsOnStart { get; private set; }
        [field: SerializeField]
        public string LogsPath { get; private set; }

        [field: Header("Statistics")]
        [field: SerializeField]
        public bool SaveStatiscticsToFile { get; private set; }
        [field: SerializeField]
        public string StatisticsPath { get; private set; }
    }
}
