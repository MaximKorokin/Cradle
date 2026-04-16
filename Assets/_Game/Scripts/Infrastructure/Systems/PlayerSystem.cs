using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public class PlayerSystem : SystemBase
    {
        private readonly PlayerContext _playerContext;

        public PlayerSystem(
            IGlobalEventBus globalEventBus,
            PlayerContext playerContext) : base(globalEventBus)
        {
            _playerContext = playerContext;

            TrackGlobalEvent<EntityDiedEvent>(OnEntityDiedEvent);
        }

        private void OnEntityDiedEvent(EntityDiedEvent e)
        {
            if (e.Victim == _playerContext.Player)
            {
                e.Victim.GetModule<HealthModule>().Heal(1000);
                e.Victim.GetModule<AppearanceModule>().RequestSetAnimatorValue(Entities.Units.EntityAnimatorParameterName.ToRevive, true);
                e.Victim.GetModule<RestrictionStateModule>().Remove(RestrictionState.Dead);
            }
        }
    }
}
