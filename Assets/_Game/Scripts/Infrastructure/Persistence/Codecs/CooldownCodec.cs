using Assets._Game.Scripts.Items;
using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class CooldownCodec : IDataCodec
    {
        public string Type => "Cooldown";

        public bool CanEncode(object data) => data is CooldownInstanceData;

        public EncodedSaveData Encode(object data)
        {
            var d = (CooldownInstanceData)data;
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(d.Cooldown.Cooldown)
            };
        }

        public object Decode(EncodedSaveData save)
            => new CooldownInstanceData(JsonConvert.DeserializeObject<float>(save.Json));
    }
}
