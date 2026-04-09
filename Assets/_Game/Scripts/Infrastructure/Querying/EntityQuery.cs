using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Items;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Querying
{
    public readonly struct EntityQuery
    {
        private readonly RestrictionState _excludedRestrictions;
        private readonly Type[] _requiredModules;
        private readonly Type[] _excludedModules;
        private readonly List<Func<Entity, Entity, bool>> _predicates;

        public EntityQuery(
            RestrictionState excludedRestrictions = RestrictionState.None,
            Type[] requiredModules = null,
            Type[] excludedModules = null,
            Func<Entity, Entity, bool>[] predicates = null)
        {
            _excludedRestrictions = excludedRestrictions;
            _requiredModules = requiredModules;
            _excludedModules = excludedModules;
            _predicates = predicates == null ? new() : new(predicates);
        }

        public EntityQuery(EntityQueryData data)
        {
            _excludedRestrictions = data.ExcludedRestrictions;
            _excludedModules = new Type[0];
            _predicates = new();

            var includedModules = new List<Type>();
            if (data.IncludedTargetDeclarations.HasFlag(TargetDeclaration.Creature))
            {
                includedModules.Add(typeof(StatModule));
            }
            if (data.IncludedTargetDeclarations.HasFlag(TargetDeclaration.LootItem))
            {
                includedModules.Add(typeof(LootItemModule));
                // Add a predicate to check if the loot item can be added to the inventory (if self is not null)
                _predicates.Add((entity, self) =>
                    self == null ||
                    self.TryGetModule<InventoryModule>(out var inventoryModule) &&
                    entity.TryGetModule<LootItemModule>(out var lootItemModule) &&
                    inventoryModule.Inventory.PreviewAdd(new(lootItemModule.ItemDefinition, new EmptyInstanceData(), lootItemModule.Amount)) > 0);
            }
            _requiredModules = includedModules.ToArray();
        }

        public bool Match(Entity entity, Entity self = null)
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

            if (_predicates != null)
            {
                for (var i = 0; i < _predicates.Count; i++)
                {
                    if (!_predicates[i](entity, self))
                        return false;
                }
            }

            return true;
        }
    }
}
