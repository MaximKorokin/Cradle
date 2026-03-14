using Assets._Game.Scripts.Entities.Modules;
using System;

namespace Assets._Game.Scripts.Infrastructure.Querying
{
    [Serializable]
    public struct EntityQueryData
    {
        public RestrictionState ExcludedRestrictions;
        public TargetDeclaration IncludedTargetDeclarations;
    }

    [Flags]
    public enum TargetDeclaration
    {
        LootItem = 1,
        Creature = 2,
        Projectile = 4,
    }
}
