using Assets._Game.Scripts.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "LocationConfig", menuName = "Configs/LocationConfig")]
    public sealed class LocationConfig : ScriptableObject
    {
        [field: SerializeField]
        public LocationTransitionData[] Locations { get; private set; }
        [field: SerializeField]
        public LocationRequirement[] LocationRequirements { get; private set; }

        public IReadOnlyList<LocationTransitionData> GetAvailableLocations(int level)
        {
            var availableLocations = new List<LocationTransitionData>(Locations.Distinct());
            availableLocations.AddRange(LocationRequirements
                .Where(r => r.RequiredLevel <= level)
                .Select(r => r.Location)
                .Distinct());
            return availableLocations;
        }
    }

    [Serializable]
    public struct LocationRequirement
    {
        public LocationTransitionData Location;
        public int RequiredLevel;
    }
}
