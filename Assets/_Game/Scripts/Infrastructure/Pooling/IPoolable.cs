using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Pooling
{
    public interface IPoolable
    {
        Component Prefab { get; set; }

        void OnTake();

        void OnReturn();
    }
}