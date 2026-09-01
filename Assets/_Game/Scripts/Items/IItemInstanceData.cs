using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {
        bool TryGet<T>(out T value) where T : class, IItemInstanceData
        {
            if (this is T t)
            {
                value = t;
                return true;
            }
            value = null;
            return false;
        }

        string GetStackingKey();

        IItemInstanceData Clone();
    }

    [Serializable]
    public class EmptyInstanceData : IItemInstanceData
    {
        public string GetStackingKey() => "";
        public IItemInstanceData Clone() => new EmptyInstanceData();
    }

    [Serializable]
    public class CompositeInstanceData : IItemInstanceData
    {
        private readonly List<IItemInstanceData> _children;
        public IReadOnlyList<IItemInstanceData> Children => _children;

        bool IItemInstanceData.TryGet<T>(out T value)
        {
            foreach (var child in Children)
            {
                if (child.TryGet(out value))
                    return true;
            }
            value = null;
            return false;
        }

        public CompositeInstanceData(IEnumerable<IItemInstanceData> children)
        {
            if (children.Any(child => child is CompositeInstanceData))
            {
                SLog.Error($"Children of {nameof(CompositeInstanceData)} cannot contain composite instance data.");
            }
            _children = new List<IItemInstanceData>(children);
        }

        public string GetStackingKey() => string.Join(",", Children.OrderBy(child => child.GetType().Name).Select(child => child.GetStackingKey()));
        public IItemInstanceData Clone() => new CompositeInstanceData(Children);
    }

    [Serializable]
    public class CooldownInstanceData : IItemInstanceData
    {
        public CooldownCounter CooldownCounter { get; private set; }

        public CooldownInstanceData(float cooldown)
        {
            CooldownCounter = new CooldownCounter(cooldown);
        }

        public string GetStackingKey() => "";
        public IItemInstanceData Clone()
        {
            var instanceData = new CooldownInstanceData(CooldownCounter.Cooldown);
            instanceData.CooldownCounter.TimeSinceReset = CooldownCounter.TimeSinceReset;
            return instanceData;
        }
    }

    [Serializable]
    public class EnchantInstanceData : IItemInstanceData
    {
        public int Level { get; private set; }

        public EnchantInstanceData(int level)
        {
            Level = level;
        }

        public string GetStackingKey() => Level.ToString();
        public IItemInstanceData Clone() => new EnchantInstanceData(Level);
    }
}
