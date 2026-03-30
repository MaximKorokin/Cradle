using Assets._Game.Scripts.Entities;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Game
{
    public sealed class PlayerReference
    {
        public GameObject Player { get; private set; }
        public Entity Entity { get; private set; }

        public void Set(GameObject player, Entity entity)
        {
            Player = player;
            Entity = entity;
        }
    }
}
