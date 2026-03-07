using Assets._Game.Scripts.Entities.Modules;

namespace Assets._Game.Scripts.Entities.Faction
{
    public sealed class FactionRelationResolver
    {
        private readonly FactionMatrix _matrix;

        public FactionRelationResolver(FactionRelations factionRelations)
        {
            int n = factionRelations.Count;
            var matrix = new FactionRelation[n, n];

            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    matrix[x, y] = factionRelations.Get(x, y);

            _matrix = new(matrix);
        }

        public FactionRelation GetRelation(Entity a, Entity b)
        {
            if (a.TryGetModule<FactionModule>(out var fa) && b.TryGetModule<FactionModule>(out var fb))
            {
                return _matrix.GetRelation(fa.FactionId, fb.FactionId);
            }

            return FactionRelation.None;
        }

        public bool IsEnemy(Entity a, Entity b)
        {
            return GetRelation(a, b) == FactionRelation.Enemy;
        }

        public bool IsAlly(Entity a, Entity b)
        {
            return GetRelation(a, b) == FactionRelation.Ally;
        }

        public bool IsNeutral(Entity a, Entity b)
        {
            return GetRelation(a, b) == FactionRelation.Neutral;
        }

        public bool IsSelf(Entity a, Entity b)
        {
            return GetRelation(a, b) == FactionRelation.Self;
        }
    }
}
