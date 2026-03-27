using Assets.CoreScripts;
using System;

namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {

    }

    public interface IImmutableItemInstanceData : IItemInstanceData
    {
        string GetStackingKey();
    }

    [Serializable]
    public class EmptyInstanceData : IImmutableItemInstanceData
    {
        public string GetStackingKey()
        {
            return "";
        }
    }

    [Serializable]
    public class DurabilityInstanceData : IItemInstanceData
    {
        public int Current;
        public int Max;
    }

    [Serializable]
    public class CooldownInstanceData : IImmutableItemInstanceData
    {
        public CooldownCounter CooldownCounter { get; private set; }

        public CooldownInstanceData(float cooldown)
        {
            CooldownCounter = new CooldownCounter(cooldown);
        }

        public string GetStackingKey()
        {
            return "";
        }
    }
}
