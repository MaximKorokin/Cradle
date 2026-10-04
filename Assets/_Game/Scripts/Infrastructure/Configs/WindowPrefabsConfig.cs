using Assets._Game.Scripts.UI.Windows;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "WindowPrefabsConfig", menuName = "Configs/WindowPrefabsConfig")]
    public sealed class WindowPrefabsConfig : ScriptableObject
    {
        [field: SerializeField]
        public UIWindowBase[] WindowPrefabs { get; private set; }
    }
}
