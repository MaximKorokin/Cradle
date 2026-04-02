using Assets._Game.Scripts.Locations.Markers;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Locations.Core
{
    public class LocationLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LocationMarkersContext>();
        }
    }
}
