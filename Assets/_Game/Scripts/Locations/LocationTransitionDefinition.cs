using UnityEngine;

namespace Assets._Game.Scripts.Locations
{
    [CreateAssetMenu(fileName = "LocationTransitionDefinition", menuName = "Game/LocationTransitionDefinition")]
    public sealed class LocationTransitionDefinition : ScriptableObject
    {
        [field: SerializeField]
        public LocationTransitionData TransitionData { get; private set; }
    }
}
