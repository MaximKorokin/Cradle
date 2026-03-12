using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(menuName = "Configs/SaveConfig")]
    public sealed class SaveConfig : ScriptableObject
    {
        [field: SerializeField]
        public float AutosaveTime { get; private set; }
    }
}
