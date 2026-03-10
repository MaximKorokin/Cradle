using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Units;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(menuName = "Configs/DefaultPrefabReferences")]
    public class DefaultPrefabReferences : ScriptableObject
    {
        [field: SerializeField]
        public EntityView EntityView { get; private set; }
        [field: SerializeField]
        public UnitView UnitView { get; private set; }
    }
}
