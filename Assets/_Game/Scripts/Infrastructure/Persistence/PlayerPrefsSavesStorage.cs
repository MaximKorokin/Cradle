using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public class PlayerPrefsSavesStorage : ISaveStorage
    {
        public string LoadText(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void SaveText(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}
