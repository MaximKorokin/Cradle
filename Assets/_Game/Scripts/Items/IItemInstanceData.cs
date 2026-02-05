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
}
