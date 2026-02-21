using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Units;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    [CreateAssetMenu(fileName = "DefaultPrefabReferences", menuName = "ScriptableObjects/DefaultPrefabReferences")]
    public class DefaultPrefabReferences : ScriptableObject
    {
        [field: SerializeField]
        public EntityView EntityView { get; private set; }
        [field: SerializeField]
        public UnitView UnitView { get; private set; }
    }
}
