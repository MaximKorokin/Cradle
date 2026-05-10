using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public abstract class DataCodecBase<T> : IDataCodec
    {
        public abstract string Type { get; }

        public virtual bool CanEncode(object data) => data is T;

        public virtual object Decode(EncodedSaveData save, object payload)
        {
            return JsonConvert.DeserializeObject<T>(save.Json);
        }

        public virtual EncodedSaveData Encode(object data)
        {
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(data)
            };
        }
    }
}
