using UnityEngine;

public interface IPoolable
{
    Component Prefab { get; set; }

    void OnTake();

    void OnReturn();
}
