using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
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
            if (TryStartAction(entity, statModule, actionModule))
            {
                StartActionPreparation(actionModule);
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
        }

        private bool TryStartAction(Entity entity, StatModule statModule, ActionModule actionModule)
        {
            var intent = entity.GetModule<IntentModule>();

            if (!intent.TryConsumeAction(out var actionIntent)) return false;

            // If the intent doesn't have an action, clear the active state.
            if (actionIntent.ActionInstance == null)
            {
                actionModule.ActiveAction = default;
                actionModule.ActiveContext = default;
                actionModule.IsPreparing = false;
                actionModule.IsChanneling = false;
                return false;
            }

            // Prevent restarting the same action.
            if (actionModule.ActiveAction?.Definition.Id == actionIntent.ActionInstance?.Definition.Id) return false;

            if (!actionModule.GlobalCooldown.IsOver()) return false;

            var context = new InteractionContext(
                entity,
                actionIntent.Target,
                actionIntent.Point);
            var action = actionIntent.ActionInstance;

            if (!action.CanStartPreparation(context)) return false;

            actionModule.ActiveAction = action;
            actionModule.ActiveContext = context;

            // sync the action speed multiplier to the animator
            if (entity.TryGetModule<AppearanceModule>(out var appearanceModule))
            {
                var speedMultiplier = actionModule.ActiveAction.Definition.SpeedMultiplier.GetValue(statModule);
                appearanceModule.RequestSetAnimatorValue(EntityAnimatorParameterName.ActionSpeedMultiplier, speedMultiplier);
            }

            return true;
        }

        private void StartActionPreparation(ActionModule actionModule)
        {
            var action = actionModule.ActiveAction;
            var context = actionModule.ActiveContext;
            action.OnPreparationStart(context);

            actionModule.IsPreparing = true;
            actionModule.RemainingPreparationTime = action.Definition.PreparationTime;
        }

        private void UpdatePreparation(StatModule statModule, ActionModule actionModule, float delta)
        {
            var speedMultiplier = actionModule.ActiveAction.Definition.SpeedMultiplier.GetValue(statModule);
            actionModule.RemainingPreparationTime -= delta * speedMultiplier;

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

            var speedMultiplier = actionModule.ActiveAction.Definition.SpeedMultiplier.GetValue(statModule);
            actionModule.RemainingChannelTime -= delta * speedMultiplier;

            // The action can end channeling early if it returns true from OnChannelTick, or if the remaining channel time is 0 or less.
            if (action.OnChannelTick(actionModule.ActiveContext, delta * speedMultiplier) || actionModule.RemainingChannelTime <= 0)
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
