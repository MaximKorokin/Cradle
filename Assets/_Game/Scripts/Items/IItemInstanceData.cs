using Assets.CoreScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {
        event Action<IItemInstanceData> Changed;

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
        public event Action<IItemInstanceData> Changed;
        public string GetStackingKey() => "";
        public IItemInstanceData Clone() => new EmptyInstanceData();
    }

    [Serializable]
    public class CompositeInstanceData : IItemInstanceData
    {
        private readonly List<IItemInstanceData> _children;
        public IReadOnlyList<IItemInstanceData> Children => _children;
        public event Action<IItemInstanceData> Changed;

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
            foreach (var c in _children)
            {
                if (c != null)
                    c.Changed += OnChildChanged;
            }
        }

        public string GetStackingKey() => string.Join(",", Children.OrderBy(child => child.GetType().Name).Select(child => child.GetStackingKey()));
        public IItemInstanceData Clone() => new CompositeInstanceData(Children);

        private void OnChildChanged(IItemInstanceData child)
        {
            Changed?.Invoke(this);
        }
    }

    [Serializable]
    public class CooldownInstanceData : IItemInstanceData
    {
        public CooldownCounter CooldownCounter { get; private set; }
        public event Action<IItemInstanceData> Changed;

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
        private int _level;
        public event Action<IItemInstanceData> Changed;

        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                Changed?.Invoke(this);
            }
        }

        public EnchantInstanceData(int level)
        {
            _level = level;
        }

        public string GetStackingKey() => Level.ToString();
        public IItemInstanceData Clone() => new EnchantInstanceData(Level);
    }
}
