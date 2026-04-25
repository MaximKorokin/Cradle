namespace Assets._Game.Scripts.UI.DataFormatters
{
    public interface IDataFormatter<TData, TResult>
    {
        public TResult FormatData(TData data);
    }
}
