using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class UnitView : MonoBehaviour, IPoolable
    {
        Component IPoolable.Prefab { get; set; }

        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; private set; }

        public string Path { get; set; }
        public int RelativeOrderInLayer { get; set; }

        public UnitView Parent { get; set; }
        public List<UnitView> Children { get; } = new();

        public void ApplyOrderInLayer(int pivotOrderInLayer)
        {
            SpriteRenderer.sortingOrder = pivotOrderInLayer + RelativeOrderInLayer;
        }

        public void OnTake()
        {
        }

        public void OnReturn()
        {
        }
    }
}
