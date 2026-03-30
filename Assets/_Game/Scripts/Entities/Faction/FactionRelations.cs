using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Faction
{
    [CreateAssetMenu(menuName = "Game/FactionRelations")]
    public class FactionRelations : ScriptableObject
    {
        public List<Faction> Factions = new();

        [HideInInspector]
        public List<FactionRelation> Relations = new();

        public int Count => Factions.Count;

        public void Resize()
        {
            int size = Count * (Count - 1) / 2;

            while (Relations.Count < size)
                Relations.Add(FactionRelation.Neutral);

            while (Relations.Count > size)
                Relations.RemoveAt(Relations.Count - 1);
        }

        private int GetIndex(int a, int b)
        {
            if (a > b)
            {
                (a, b) = (b, a);
            }

            return b * (b - 1) / 2 + a;
        }

        public FactionRelation Get(int a, int b)
        {
            if (a == b) return FactionRelation.Self;

            return Relations[GetIndex(a, b)];
        }

        public void Set(int a, int b, FactionRelation r)
        {
            if (a == b) return;

            Relations[GetIndex(a, b)] = r;
        }
    }
}
