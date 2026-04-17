using Assets._Game.Scripts.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.Items
{
    [CreateAssetMenu(menuName = "Definitions/Item")]
    public class ItemDefinition : GuidScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        [field: SerializeField]
        public int MaxAmount { get; private set; }
        [field: SerializeField]
        public int Weight { get; private set; }
        [field: SerializeReference]
        public ItemTraitBase[] Traits { get; private set; } = Array.Empty<ItemTraitBase>();

        public bool TryGetTrait<T>(out T trait) where T : ItemTraitBase
        {
            for (int i = 0; i < Traits.Length; i++)
            {
                if (Traits[i] is T t)
                {
                    trait = t;
                    return true;
                }
            }

            trait = null;
            return false;
        }

        public T GetTrait<T>() where T : ItemTraitBase
        {
            return TryGetTrait<T>(out var trait) ? trait : null;
        }

        public IEnumerable<T> GetTraits<T>() where T : ItemTraitBase
        {
            for (int i = 0; i < Traits.Length; i++)
            {
                if (Traits[i] is T t)
                {
                    yield return t;
                }
            }
        }
    }
}
