using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class Unit
    {
        public string Path { get; }
        public GameObject GameObject { get; }
        public SpriteRenderer SpriteRenderer { get; }

        public int RelativeOrderInLayer { get; set; }

        public Unit Parent { get; internal set; }
        public List<Unit> Children { get; } = new();

        public Unit(GameObject gameObject, string path, int relativeOrderInLayer)
        {
            GameObject = gameObject;
            Path = path;
            SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            RelativeOrderInLayer = relativeOrderInLayer;
        }

        public void SetSprite(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }

        public void ApplyOrderInLayer(int pivotOrderInLayer)
        {
            SpriteRenderer.sortingOrder = pivotOrderInLayer + RelativeOrderInLayer;
        }
    }
}
