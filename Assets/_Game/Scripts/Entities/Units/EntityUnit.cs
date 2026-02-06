using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class EntityUnit
    {
        public string Path { get; }
        public GameObject GameObject { get; }
        public SpriteRenderer SpriteRenderer { get; }

        public int RelativeOrderInLayer { get; set; }

        public EntityUnit Parent { get; internal set; }
        public List<EntityUnit> Children { get; } = new();

        public EntityUnit(GameObject gameObject, string path, int relativeOrderInLayer)
        {
            GameObject = gameObject;
            Path = path;
            SpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            RelativeOrderInLayer = relativeOrderInLayer;
        }

        public void Set(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }

        public void ApplyOrderInLayer(int pivotOrderInLayer)
        {
            SpriteRenderer.sortingOrder = pivotOrderInLayer + RelativeOrderInLayer;
        }
    }
}
