using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
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

            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        private void OnEquipmentChanged(EquipmentChangedEvent e)
        {
            var actionModule = e.Entity.GetModule<ActionModule>();
            if (!e.Item.HasValue) return;

            var specialActionTrait = e.Item.Value.GetTrait<SpecialActionTrait>();
            if (specialActionTrait == null) return;

            if (e.Kind == EquipmentChangeKind.Equipped || e.Kind == EquipmentChangeKind.Replaced)
            {
                actionModule.SetSpecialAction(specialActionTrait.Kind, new(specialActionTrait.Action));
            }
            else if (e.Kind == EquipmentChangeKind.Unequipped)
            {
                actionModule.SetSpecialAction(specialActionTrait.Kind, null);
            }
        }

        public void Tick(float delta)
        {
            IterateMatchingEntities(entity => TickEntity(entity, delta));
        }

        /// <summary>
        /// If there's no active action, try to start one.
        /// If there is an active action, but we're not in range, try to approach.
        /// If we're preparing, update the preparation time.
        /// If we're channeling, update the channel time.
        /// </summary>
        private void TickEntity(Entity entity, float delta)
        {
            var statModule = entity.GetModule<StatModule>();
            var actionModule = entity.GetModule<ActionModule>();

            // If there's no active action, try to start one.
            if (actionModule.ActiveAction == null)
            {
                TryStartAction(entity, statModule, actionModule);
                return;
            }

            if (actionModule.IsPreparing)
            {
                UpdatePreparation(statModule, actionModule, delta);
                return;
            }

            if (actionModule.IsChanneling)
            {
                UpdateChanneling(statModule, actionModule, delta);
                return;
            }
            
            // If there is an active action, but we're not in range, try to approach.
            if (ApproachPreparationRange(entity, actionModule))
            {
                TryStartActionPreparation(entity, statModule, actionModule);
                return;
            }
        }

        private void TryStartAction(Entity entity, StatModule statModule, ActionModule actionModule)
        {
            var intent = entity.GetModule<IntentModule>();

            if (!intent.TryConsumeAction(out var actionIntent))
                return;

            if (!actionModule.GlobalCooldown.IsOver())
                return;

            var context = new InteractionContext(
                entity,
                actionIntent.Target,
                actionIntent.Point);
            var action = actionIntent.ActionInstance;

            if (!action.CanStartPreparation(context))
                return;

            actionModule.ActiveAction = action;
            actionModule.ActiveContext = context;
        }

        /// <summary>
        /// Returns true if the entity is within preparation range, otherwise sets the move intent towards the target and returns false
        /// </summary>
        private bool ApproachPreparationRange(Entity entity, ActionModule actionModule)
        {
            var direction = actionModule.ActiveContext.Target.GetPosition() - entity.GetPosition();
            if (direction.sqrMagnitude <= actionModule.ActiveAction.Definition.Range * actionModule.ActiveAction.Definition.Range)
                return true;
            var intent = entity.GetModule<IntentModule>();
            intent.SetMove(new(direction));
            return false;
        }

        private void TryStartActionPreparation(Entity entity, StatModule statModule, ActionModule actionModule)
        {
            var action = actionModule.ActiveAction;
            var context = actionModule.ActiveContext;
            action.OnPreparationStart(context);

            actionModule.IsPreparing = true;
            actionModule.RemainingPreparationTime = action.Definition.PreparationTime;
        }

        private void UpdatePreparation(StatModule statModule, ActionModule actionModule, float delta)
        {
            actionModule.RemainingPreparationTime -= delta;

            if (actionModule.RemainingPreparationTime > 0)
                return;

            var action = actionModule.ActiveAction;

            actionModule.IsPreparing = false;

            action.OnPreparationComplete(actionModule.ActiveContext, _resolver);

            if (action.Definition.MaxChannelingTime > 0)
            {
                actionModule.IsChanneling = true;
                actionModule.RemainingChannelTime = action.Definition.MaxChannelingTime;
            }
            else
            {
                CompleteAction(statModule, actionModule, action);
            }
        }

        private void UpdateChanneling(StatModule statModule, ActionModule actionModule, float delta)
        {
            var action = actionModule.ActiveAction;

            actionModule.RemainingChannelTime -= delta;

            // The action can end channeling early if it returns true from OnChannelTick, or if the remaining channel time is 0 or less.
            if (action.OnChannelTick(actionModule.ActiveContext, delta) || actionModule.RemainingChannelTime <= 0)
            {
                actionModule.IsChanneling = false;
                CompleteAction(statModule, actionModule, action);
            }
        }

        private void CompleteAction(StatModule statModule, ActionModule actionModule, ActionInstance action)
        {
            actionModule.ActiveAction.Cooldown.Reset();
            actionModule.ActiveAction = null;
            actionModule.GlobalCooldown.Cooldown = statModule.Stats.Get(StatId.PhysicalActionDelay);
            actionModule.GlobalCooldown.Reset();
        }
    }
}
