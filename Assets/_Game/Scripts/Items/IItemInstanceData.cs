using System;

namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {

    }

    public interface IImmutableInstanceData : IItemInstanceData
    {
        int GetStackKey();
    }

    [Serializable]
    public class EmptyInstanceData : IImmutableInstanceData
    {
        public int GetStackKey()
        {
            return 0;
        }
    }
}
