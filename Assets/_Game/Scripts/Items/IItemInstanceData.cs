namespace Assets._Game.Scripts.Items
{
    public interface IItemInstanceData
    {

    }

    public interface IImmutableInstanceData : IItemInstanceData
    {
        int GetStackKey();
    }
}
