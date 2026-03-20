using Assets._Game.Scripts.Entities.Interactions;
using Assets._Game.Scripts.Entities.Interactions.Action;
using Assets._Game.Scripts.Shared.Extensions;
using Assets.CoreScripts;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class ActionModule : EntityModuleBase
    {
        public bool IsPreparing;
        public float RemainingPreparationTime;

        public bool IsChanneling;
        public float RemainingChannelTime;

        public ActionInstance ActiveAction;
        public InteractionContext ActiveContext;

        private readonly IReadOnlyList<ActionInstance> _actions;
        private readonly Dictionary<SpecialActionKind, ActionInstance> _initialSpecialActions;
        private readonly Dictionary<SpecialActionKind, ActionInstance> _specialActionOverrides;

        public IReadOnlyList<ActionInstance> Actions;

        public CooldownCounter GlobalCooldown = new();

        public ActionModule(IReadOnlyList<ActionInstance> actions, Dictionary<SpecialActionKind, ActionInstance> specialActions)
        {
            _actions = actions;
            _initialSpecialActions = specialActions;
            _specialActionOverrides = new();

            // Combine regular and special actions into a single array for easier iteration.
            var newActions = new ActionInstance[actions.Count + specialActions.Count];
            for (int i = 0; i < _actions.Count; i++)
            {
                newActions[i] = _actions[i];
            }

            int index = _actions.Count;
            foreach (var action in _initialSpecialActions.Values)
            {
                newActions[index++] = action;
            }

            Actions = newActions;
        }

        public void AddAction(ActionInstance action)
        {
            var newActions = new ActionInstance[Actions.Count + 1];
            for (int i = 0; i < Actions.Count; i++)
            {
                newActions[i] = Actions[i];
            }
            newActions[Actions.Count] = action;
            Actions = newActions;
        }

        public void RemoveAction(ActionInstance action)
        {
            var newActions = new ActionInstance[Actions.Count - 1];
            var index = 0;
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i] == action) continue;
                newActions[index] = Actions[i];
                index++;
            }
            Actions = newActions;
        }

        public ActionInstance GetSpecialAction(SpecialActionKind kind)
        {
            if (_specialActionOverrides.TryGetNonDefaultValue(kind, out var action) || _initialSpecialActions.TryGetNonDefaultValue(kind, out action))
            {
                return action;
            }
            return null;
        }

        public bool TryGetSpecialAction(SpecialActionKind kind, out ActionInstance action)
        {
            return _specialActionOverrides.TryGetNonDefaultValue(kind, out action) || _initialSpecialActions.TryGetNonDefaultValue(kind, out action);
        }

        // Sets a special action override for the given kind. If the override is null, it removes the override and reverts to the initial action if it exists.
        public void SetSpecialAction(SpecialActionKind kind, ActionInstance action)
        {
            if (TryGetSpecialAction(kind, out var oldAction))
            {
                RemoveAction(oldAction);
            }

            _specialActionOverrides[kind] = action;
            if (action != null)
            {
                AddAction(action);
            }
            else if (_initialSpecialActions.TryGetNonDefaultValue(kind, out var initialAction))
            {
                AddAction(initialAction);
            }
        }
    }

    public struct SpecialAction
    {
        public SpecialActionKind Kind;
        public ActionInstance Action;
    }

    public enum SpecialActionKind
    {
        Attack,
        Pickup,
    }

    public sealed class ActionModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<ActionModuleDefinition>(out var moduleDefinition))
            {
                return null;
            }

            var specialActions = new Dictionary<SpecialActionKind, ActionInstance>();
            foreach (var specialAction in moduleDefinition.SpecialActions)
            {
                specialActions[specialAction.Kind] = new(specialAction.Action);
            }

            var actions = new List<ActionInstance>();
            foreach (var definition in moduleDefinition.Actions)
            {
                actions.Add(new(definition));
            }
            return new ActionModule(actions, specialActions);
        }
    }

}
