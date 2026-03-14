using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Querying
{
    public readonly struct EntityQuery
    {
        private readonly RestrictionState _excludedRestrictions;
        private readonly Type[] _requiredModules;
        private readonly Type[] _excludedModules;

        public EntityQuery(
            RestrictionState excludedRestrictions = RestrictionState.None,
            Type[] requiredModules = null,
            Type[] excludedModules = null)
        {
            _excludedRestrictions = excludedRestrictions;
            _requiredModules = requiredModules;
            _excludedModules = excludedModules;
        }

        public EntityQuery(EntityQueryData data)
        {
            _excludedRestrictions = data.ExcludedRestrictions;
            _excludedModules = new Type[0];

            var includedModules = new List<Type>();
            if (data.IncludedTargetDeclarations.HasFlag(TargetDeclaration.Creature))
            {
                includedModules.Add(typeof(StatModule));
            }
            if (data.IncludedTargetDeclarations.HasFlag(TargetDeclaration.LootItem))
            {
                includedModules.Add(typeof(LootItemModule));
            }
            _requiredModules = includedModules.ToArray();
        }

        public bool Match(Entity entity)
        {
            if (entity == null)
                return false;

            if (entity.TryGetModule<RestrictionStateModule>(out var restrictionModule))
            {
                if ((restrictionModule.State & _excludedRestrictions) != 0)
                    return false;
            }

            if (_requiredModules != null)
            {
                for (var i = 0; i < _requiredModules.Length; i++)
                {
                    if (!entity.HasModule(_requiredModules[i]))
                        return false;
                }
            }

            if (_excludedModules != null)
            {
                for (var i = 0; i < _excludedModules.Length; i++)
                {
                    if (entity.HasModule(_excludedModules[i]))
                        return false;
                }
            }

            return true;
        }
    }
}
