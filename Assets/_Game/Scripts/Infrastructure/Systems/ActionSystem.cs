using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Shared.Extensions;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    public sealed class ActionSystem : EntitySystemBase, ITickSystem
    {
        private readonly IObjectResolver _resolver;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead | RestrictionState.Stunned | RestrictionState.Feared,
                new[] { typeof(ActionModule), typeof(StatModule), typeof(IntentModule) }
            );

        public ActionSystem(EntityRepository entityRepository, IObjectResolver resolver) : base(entityRepository)
        {
            _resolver = resolver;
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(entity => TickEntity(entity, delta));
        }

        private void TickEntity(Entity entity, float delta)
        {
            var statModule = entity.GetModule<StatModule>();
            var actionModule = entity.GetModule<ActionModule>();

            if (actionModule.IsCasting)
            {
                UpdateCasting(statModule, actionModule, delta);
                return;
            }

            if (actionModule.IsChanneling)
            {
                UpdateChanneling(statModule, actionModule, delta);
                return;
            }

            TryStartAction(statModule, entity, actionModule);
        }

        private void TryStartAction(StatModule statModule, Entity entity, ActionModule actionModule)
        {
            var intent = entity.GetModule<IntentModule>();

            if (!intent.TryConsumeAct(out var actionIntent))
                return;

            if (!actionModule.GlobalCooldown.IsOver())
                return;

            var context = new InteractionContext(
                entity,
                actionIntent.Target,
                actionIntent.Point);
            var action = actionIntent.ActionInstance;

            if (!action.CanStartCast(context))
                return;

            action.OnCastStart(context);

            actionModule.ActiveAction = action;
            actionModule.ActiveContext = context;

            if (action.Definition.CastTime > 0)
            {
                actionModule.IsCasting = true;
                actionModule.RemainingCastTime = action.Definition.CastTime;
            }
            else
            {
                CompleteAction(statModule, actionModule, action);
            }
        }

        private void UpdateCasting(StatModule statModule, ActionModule actionModule, float delta)
        {
            actionModule.RemainingCastTime -= delta;

            if (actionModule.RemainingCastTime > 0)
                return;

            var action = actionModule.ActiveAction;

            actionModule.IsCasting = false;

            if (action.Definition.ChannelTime > 0)
            {
                actionModule.IsChanneling = true;
                actionModule.RemainingChannelTime = action.Definition.ChannelTime;
            }
            else
            {
                CompleteAction(statModule, actionModule, action);
            }
        }

        private void UpdateChanneling(StatModule statModule, ActionModule actionModule, float delta)
        {
            var action = actionModule.ActiveAction;

            action.OnChannelTick(actionModule.ActiveContext, delta);

            actionModule.RemainingChannelTime -= delta;

            if (actionModule.RemainingChannelTime <= 0)
            {
                actionModule.IsChanneling = false;
                CompleteAction(statModule, actionModule, action);
            }
        }

        private void CompleteAction(StatModule statModule, ActionModule actionModule, ActionInstance action)
        {
            action.OnCastComplete(actionModule.ActiveContext, _resolver);

            actionModule.ActiveAction.Cooldown.Reset();
            actionModule.GlobalCooldown.Cooldown = statModule.Stats.Get(StatId.PhysicalActionDelay);
            actionModule.GlobalCooldown.Reset();
        }
    }
}
