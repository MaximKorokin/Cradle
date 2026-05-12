using Assets.CoreScripts;
using System;

namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {
        IItemInstanceData Clone();
    }

    public interface IImmutableItemInstanceData : IItemInstanceData
    {
        string GetStackingKey();
    }

    [Serializable]
    public class EmptyInstanceData : IImmutableItemInstanceData
    {
        public string GetStackingKey() => "";
        public IItemInstanceData Clone() => new EmptyInstanceData();
    }

    [Serializable]
    public class CooldownInstanceData : IImmutableItemInstanceData
    {
        public CooldownCounter CooldownCounter { get; private set; }

        public CooldownInstanceData(float cooldown)
        {
            CooldownCounter = new CooldownCounter(cooldown);
        }

        public string GetStackingKey() => "";
        public IItemInstanceData Clone() => new CooldownInstanceData(CooldownCounter.Cooldown);
    }
}
