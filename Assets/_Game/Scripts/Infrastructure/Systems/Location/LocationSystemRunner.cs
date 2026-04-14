using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems.Location
{
    /// <summary>
    /// Runs in Location scope and manages the execution of location systems (start, tick, fixed tick).
    /// </summary>
    public sealed class LocationSystemRunner : SystemRunnerBase
    {
        public LocationSystemRunner(
            DispatcherService dispatcherService,
            IReadOnlyList<ILocationSystem> systems) : base(dispatcherService, systems) { }
    }
}
