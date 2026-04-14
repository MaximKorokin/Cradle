using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class SystemRunner : SystemRunnerBase
    {
        public SystemRunner(
            DispatcherService dispatcherService,
            IReadOnlyList<ISystem> systems) : base(dispatcherService, systems) { }
    }
}
