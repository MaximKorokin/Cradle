using Assets._Game.Scripts.UI.Windows;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "UIWindowPrefabsConfig", menuName = "Configs/UIWindowPrefabsConfig")]
    public sealed class UIWindowsConfig : ScriptableObject
    {
        [field: SerializeField]
        public UIWindowBase[] WindowPrefabs { get; private set; }
    }
}
