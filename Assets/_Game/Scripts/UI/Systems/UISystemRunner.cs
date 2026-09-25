using Assets._Game.Scripts.Infrastructure;
using Assets._Game.Scripts.Infrastructure.Systems;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Systems
{
    public sealed class UISystemRunner : SystemRunnerBase
    {
        public UISystemRunner(
            DispatcherService dispatcherService,
            IReadOnlyList<IUISystem> systems) : base(dispatcherService, systems) { }
    }
}
