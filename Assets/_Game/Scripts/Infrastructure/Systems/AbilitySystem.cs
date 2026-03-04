using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Ability;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class AbilitySystem : SystemBase
    {
        private readonly EntityRepository _entityRepository;
        private readonly DispatcherService _dispatcher;
        private readonly IObjectResolver _resolver;

        public AbilitySystem(EntityRepository entityRepository, DispatcherService dispatcher, IObjectResolver resolver)
        {
            _entityRepository = entityRepository;
            _dispatcher = dispatcher;
            _resolver = resolver;

            _dispatcher.OnTick += OnTick;
        }

        public override void Dispose()
        {
            base.Dispose();
            _dispatcher.OnTick -= OnTick;
        }

        public void OnTick(float delta)
        {
            foreach (var entity in _entityRepository.All)
            {
                if (!entity.HasModule<AbilityModule>() || !entity.HasModule<StatModule>())
                    continue;
                Tick(entity, delta);
            }
        }

        public void Tick(Entity entity, float delta)
        {
            var statModule = entity.GetModule<StatModule>();
            var abilityModule = entity.GetModule<AbilityModule>();

            if (abilityModule.IsCasting)
            {
                UpdateCasting(statModule, abilityModule, delta);
                return;
            }

            if (abilityModule.IsChanneling)
            {
                UpdateChanneling(statModule, abilityModule, delta);
                return;
            }

            TryStartAbility(statModule, entity, abilityModule);
        }

        private void TryStartAbility(StatModule statModule, Entity entity, AbilityModule abilityModule)
        {
            var intent = entity.GetModule<IntentModule>();

            if (!intent.TryConsumeUseAbility(out var abilityIntent))
                return;

            if (abilityModule.GlobalCooldown.IsOver())
                return;

            var context = new InteractionContext(
                entity,
                abilityIntent.Target,
                abilityIntent.Point);
            var ability = abilityIntent.AbilityInstance;

            if (!ability.CanStartCast(context))
                return;

            ability.OnCastStart(context);

            abilityModule.ActiveAbility = ability;
            abilityModule.ActiveContext = context;

            if (ability.Definition.CastTime > 0)
            {
                abilityModule.IsCasting = true;
                abilityModule.RemainingCastTime = ability.Definition.CastTime;
            }
            else
            {
                CompleteAbility(statModule, abilityModule, ability);
            }
        }

        private void UpdateCasting(StatModule statModule, AbilityModule abilityModule, float delta)
        {
            abilityModule.RemainingCastTime -= delta;

            if (abilityModule.RemainingCastTime > 0)
                return;

            var ability = abilityModule.ActiveAbility;

            abilityModule.IsCasting = false;

            if (ability.Definition.ChannelTime > 0)
            {
                abilityModule.IsChanneling = true;
                abilityModule.RemainingChannelTime = ability.Definition.ChannelTime;
            }
            else
            {
                CompleteAbility(statModule, abilityModule, ability);
            }
        }

        private void UpdateChanneling(StatModule statModule, AbilityModule abilityModule, float delta)
        {
            var ability = abilityModule.ActiveAbility;

            ability.OnChannelTick(abilityModule.ActiveContext, delta);

            abilityModule.RemainingChannelTime -= delta;

            if (abilityModule.RemainingChannelTime <= 0)
            {
                abilityModule.IsChanneling = false;
                CompleteAbility(statModule, abilityModule, ability);
            }
        }

        private void CompleteAbility(StatModule statModule, AbilityModule abilityModule, AbilityInstance ability)
        {
            ability.OnCastComplete(abilityModule.ActiveContext, _resolver);

            abilityModule.ActiveAbility.Cooldown.Reset();
            abilityModule.GlobalCooldown.Cooldown = statModule.Stats.Get(StatId.PhysicalAbilityDelay);
            abilityModule.GlobalCooldown.Reset();

            abilityModule.ActiveAbility = null;
        }
    }
}
