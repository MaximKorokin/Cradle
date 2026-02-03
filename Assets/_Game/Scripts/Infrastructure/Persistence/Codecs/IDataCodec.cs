namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public interface IDataCodec
    {
        string Type { get; }
        bool CanEncode(object data);
        EncodedSaveData Encode(object data);
        object Decode(EncodedSaveData save);
    }
}
