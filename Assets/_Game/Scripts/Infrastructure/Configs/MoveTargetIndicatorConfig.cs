using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "MoveTargetIndicatorConfig", menuName = "Configs/MoveTargetIndicatorConfig")]
    public sealed class MoveTargetIndicatorConfig : ScriptableObject
    {
        [field: SerializeField]
        public MoveTargetIndicatorView Prefab { get; private set; }
    }
}
