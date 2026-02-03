using Assets._Game.Scripts.Items;
using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class DurabilityCodec : IDataCodec
    {
        public string Type => "Durability";

        public bool CanEncode(object data) => data is DurabilityInstanceData;

        public EncodedSaveData Encode(object data)
        {
            var d = (DurabilityInstanceData)data;
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(d)
            };
        }

        public object Decode(EncodedSaveData save)
            => JsonConvert.DeserializeObject<DurabilityInstanceData>(save.Json);
    }
}
