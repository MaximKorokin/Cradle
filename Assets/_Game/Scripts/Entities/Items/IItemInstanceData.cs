namespace Assets._Game.Scripts.Entities.Items
{
    public interface IItemInstanceData
    {

    }

    public interface IImmutableInstanceData : IItemInstanceData
    {
        int GetStackKey();
    }
}
