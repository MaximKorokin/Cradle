using System;

namespace Assets._Game.Scripts.Entities.Items
{
    public interface IItemTrait { }

    [Serializable]
    public abstract class ItemTraitBase : IItemTrait { }

    // пример
    [Serializable]
    public class RangedTrait : ItemTraitBase
    {
        public float Range = 6f;
    }

    // пример
    [Serializable]
    public class HealTrait : ItemTraitBase
    {
        public int HealAmount = 25;
    }
}
