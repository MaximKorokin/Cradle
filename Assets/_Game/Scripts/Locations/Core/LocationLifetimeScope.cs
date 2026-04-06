using Assets._Game.Scripts.Infrastructure.Systems.Location;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets._Game.Scripts.Locations.Core
{
    public class LocationLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private LocationMarkersContext _markersContext;
        [SerializeField]
        private LocationBordersContext _bordersContext;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LocationBootstrap>();
            builder.RegisterEntryPoint<LocationSystemRunner>();

            builder.RegisterComponent(_markersContext);
            builder.RegisterComponent(_bordersContext);

            builder.Register<LocationRuntimeSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
