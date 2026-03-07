namespace Assets._Game.Scripts.Entities.Faction
{
    public sealed class FactionMatrix
    {
        private readonly FactionRelation[,] _relations;

        public FactionMatrix(FactionRelation[,] relations)
        {
            _relations = relations;
        }

        public void SetRelation(int a, int b, FactionRelation relation)
        {
            _relations[a, b] = relation;
        }

        public FactionRelation GetRelation(int a, int b)
        {
            if (a == b)
                return FactionRelation.Self;

            return _relations[a, b];
        }
    }

    public enum FactionRelation : byte
    {
        None = 0,
        Self = 10,
        Ally = 20,
        Neutral = 30,
        Enemy = 40
    }
}
