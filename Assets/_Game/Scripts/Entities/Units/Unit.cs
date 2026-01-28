using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public class Unit
    {
        private readonly SpriteRenderer _spriteRenderer;
        private readonly List<Unit> _childrenUnits = new();
        private readonly int _relativeOrderInLayer;

        public Unit(GameObject gameObject, string path, int relativeOrderInLayer)
        {
            GameObject = gameObject;
            Path = path;
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            _relativeOrderInLayer = relativeOrderInLayer;
        }

        public GameObject GameObject { get; private set; }

        public string Path { get; private set; }

        public void Set(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void UpdateOrderInLayer(int pivotOrder)
        {
            _spriteRenderer.sortingOrder = pivotOrder + _relativeOrderInLayer;
        }

        public void AddChild(Unit entityUnit)
        {
            _childrenUnits.Add(entityUnit);
            entityUnit.GameObject.transform.parent = GameObject.transform;
        }
    }
}
