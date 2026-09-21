using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Shared;
using System.Linq;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public sealed class StatsWindowController : WindowControllerBase<StatsWindow, StatsWindowControllerArguments>
    {
        private StatModule _statModule;
        private readonly EntityRepository _entityRepository;

        public StatsWindowController(EntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        protected override void OnBind()
        {
            base.OnBind();

            _statModule = _entityRepository.Get(Arguments.EntityId.Value).GetModule<StatModule>();
            _statModule.Stats.Changed += Redraw;
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            _statModule.Stats.Changed -= Redraw;
            _statModule = null;
        }

        protected override void Redraw()
        {
            Window.Render(_statModule.Stats.Enumerate().Select(s => (s.Id.ToString(), s.Final.ToString())));
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_statModule != null)
            {
                _statModule.Stats.Changed -= Redraw;
            }
        }
    }

    public readonly struct StatsWindowControllerArguments : IWindowControllerArguments
    {
        public IReadOnlyObservableData<string> EntityId { get; }
        public StatsWindowControllerArguments(IReadOnlyObservableData<string> entityId)
        {
            EntityId = entityId;
        }
    }
}
