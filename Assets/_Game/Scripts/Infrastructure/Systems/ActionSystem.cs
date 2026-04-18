using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Control.Intents;
using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Entities.Stats;
using Assets._Game.Scripts.Entities.Units;
using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Querying;
using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Equipment;
using Assets._Game.Scripts.Items.Traits;
using Assets._Game.Scripts.Shared.Extensions;
using VContainer;

namespace Assets._Game.Scripts.Infrastructure.Systems
{
    /// <summary>
    /// Drives the action lifecycle for each entity: intent → preparation → channeling → completion.
    /// Also reacts to equipment changes that grant or revoke special actions.
    /// </summary>
    public sealed class ActionSystem : EntitySystemBase, ITickSystem
    {
        private readonly IObjectResolver _resolver;

        protected override EntityQuery EntityQuery { get; } =
            new EntityQuery(
                RestrictionState.Disabled | RestrictionState.Dead | RestrictionState.Stunned | RestrictionState.Feared,
                new[] { typeof(ActionModule), typeof(StatModule), typeof(IntentModule) }
            );

        public ActionSystem(
            IGlobalEventBus globalEventBus,
            EntityRepository entityRepository,
            IObjectResolver resolver) : base(globalEventBus, entityRepository)
        {
            _resolver = resolver;

            TrackEntityEvent<EquipmentChangedEvent>(OnEquipmentChanged);
        }

        protected override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);

            if (!EntityQuery.Match(entity)) return;

            if (entity.TryGetModule<EquipmentModule>(out var equipmentModule))
            {
                foreach (var (slot, item) in equipmentModule.Equipment.Enumerate())
                {
                    if (item == null) continue;
                    var equipmentChangedEvent = new EquipmentChangedEvent(slot, item.Value, EquipmentChangeKind.Equipped);
                    OnEquipmentChanged(entity, equipmentChangedEvent);
                }
            }
        }

        // ───────────────────────── Equipment Events ─────────────────────────

        private void OnEquipmentChanged(Entity entity, EquipmentChangedEvent e)
        {
            if (!e.Item.HasValue) return;

            var actionModule = entity.GetModule<ActionModule>();
            var isEquipping = e.Kind == EquipmentChangeKind.Equipped || e.Kind == EquipmentChangeKind.Replaced;
            var context = new ItemTriggerContext(entity, ItemTrigger.OnEquipmentChange, e.Item.Value);

            foreach (var specialActionTrait in e.Item.Value.GetFunctionalTraits<SpecialActionTrait>(ItemTrigger.OnEquipmentChange))
            {
                if (specialActionTrait == null || !specialActionTrait.CanTrigger(context)) continue;

                actionModule.SetSpecialAction(
                    specialActionTrait.Kind,
                    isEquipping ? new ActionInstance(specialActionTrait.Action) : null
                );
            }
        }

        // ───────────────────────── Tick Loop ─────────────────────────

        public void Tick(float delta)
        {
            IterateMatchingEntities(entity => TickEntity(entity, delta));
        }

        private void TickEntity(Entity entity, float delta)
        {
            var statModule = entity.GetModule<StatModule>();
            var actionModule = entity.GetModule<ActionModule>();

            if (actionModule.IsPreparing)
            {
                TickPreparation(statModule, actionModule, delta);
            }
            else if (actionModule.IsChanneling)
            {
                TickChanneling(statModule, actionModule, delta);
            }
            else
            {
                TryBeginNewAction(entity, statModule, actionModule);
            }
        }

        // ───────────────────────── Starting a New Action ─────────────────────────

        private void TryBeginNewAction(Entity entity, StatModule statModule, ActionModule actionModule)
        {
            var intent = entity.GetModule<IntentModule>();

            if (!intent.TryConsumeAction(out var actionIntent))
                return;

            var action = actionIntent.ActionInstance;

            // A null action signals that the current action should be cancelled.
            if (action == null)
            {
                actionModule.ResetActiveState();
                return;
            }

            if (!CanStartAction(actionModule, action, entity, actionIntent))
                return;

            ActivateAction(entity, statModule, actionModule, action, actionIntent);
            BeginPreparation(actionModule);
        }

        private bool CanStartAction(ActionModule actionModule, ActionInstance action, Entity entity, ActionIntent actionIntent)
        {
            // Prevent restarting the same action.
            if (actionModule.ActiveAction?.Definition.Id == action.Definition.Id)
                return false;

            if (!actionModule.GlobalCooldown.IsOver())
                return false;

            var context = new InteractionContext(entity, actionIntent.Target, actionIntent.Point);
            return action.CanStartPreparation(context);
        }

        private void ActivateAction(Entity entity, StatModule statModule, ActionModule actionModule, ActionInstance action, ActionIntent actionIntent)
        {
            actionModule.ResetActiveState();

            actionModule.ActiveAction = action;
            actionModule.ActiveContext = new InteractionContext(entity, actionIntent.Target, actionIntent.Point);

            SyncAnimatorSpeed(entity, statModule, actionModule);
        }

        private void SyncAnimatorSpeed(Entity entity, StatModule statModule, ActionModule actionModule)
        {
            if (!entity.TryGetModule<AppearanceModule>(out var appearanceModule))
                return;

            var speedMultiplier = actionModule.ActiveAction.Definition.SpeedMultiplier.GetValue(statModule);
            appearanceModule.RequestSetAnimatorValue(EntityAnimatorParameterName.ActionSpeedMultiplier, speedMultiplier);
        }

        // ───────────────────────── Preparation Phase ─────────────────────────

        private void BeginPreparation(ActionModule actionModule)
        {
            var action = actionModule.ActiveAction;

            action.OnPreparationStart(actionModule.ActiveContext);

            actionModule.IsPreparing = true;
            actionModule.RemainingPreparationTime = action.Definition.PreparationTime;
        }

        private void TickPreparation(StatModule statModule, ActionModule actionModule, float delta)
        {
            var speedMultiplier = actionModule.ActiveAction.Definition.SpeedMultiplier.GetValue(statModule);
            actionModule.RemainingPreparationTime -= delta * speedMultiplier;

            if (actionModule.RemainingPreparationTime > 0)
                return;

            actionModule.IsPreparing = false;

            var action = actionModule.ActiveAction;
            action.OnPreparationComplete(actionModule.ActiveContext, _resolver);

            if (action.Definition.MaxChannelingTime > 0)
            {
                BeginChanneling(actionModule, action);
            }
            else
            {
                CompleteAction(statModule, actionModule, action);
            }
        }

        // ───────────────────────── Channeling Phase ─────────────────────────

        private void BeginChanneling(ActionModule actionModule, ActionInstance action)
        {
            actionModule.IsChanneling = true;
            actionModule.RemainingChannelTime = action.Definition.MaxChannelingTime;
        }

        private void TickChanneling(StatModule statModule, ActionModule actionModule, float delta)
        {
            var action = actionModule.ActiveAction;
            var speedMultiplier = action.Definition.SpeedMultiplier.GetValue(statModule);
            var scaledDelta = delta * speedMultiplier;

            actionModule.RemainingChannelTime -= scaledDelta;

            var channelingFinished = action.OnChannelTick(actionModule.ActiveContext, scaledDelta);

            if (channelingFinished || actionModule.RemainingChannelTime <= 0)
            {
                actionModule.IsChanneling = false;
                CompleteAction(statModule, actionModule, action);
            }
        }

        // ───────────────────────── Completion ─────────────────────────

        private void CompleteAction(StatModule statModule, ActionModule actionModule, ActionInstance action)
        {
            action.Cooldown.Reset();
            actionModule.ResetActiveState();

            actionModule.GlobalCooldown.Cooldown = statModule.Stats.Get(StatId.PhysicalActionDelay);
            actionModule.GlobalCooldown.Reset();

            actionModule.Entity.Publish(new ActionCompletedEvent(action));
        }
    }

    public readonly struct ActionCompletedEvent : IEntityEvent
    {
        public readonly ActionInstance ActionInstance;

        public ActionCompletedEvent(ActionInstance actionInstance)
        {
            ActionInstance = actionInstance;
        }
    }
}
