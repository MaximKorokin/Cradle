using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public class EntityUnit
    {
        private readonly SpriteRenderer _spriteRenderer;
        private readonly List<EntityUnit> _childrenUnits = new();

        public EntityUnit(GameObject gameObject, string path)
        {
            GameObject = gameObject;
            Path = path;
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        }

        public GameObject GameObject { get; private set; }

        public string Path { get; private set; }

        public void Set(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void AddChild(EntityUnit entityUnit)
        {
            _childrenUnits.Add(entityUnit);
            entityUnit.GameObject.transform.parent = GameObject.transform;
        }
    }
}
