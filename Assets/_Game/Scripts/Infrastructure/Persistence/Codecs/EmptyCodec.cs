using Assets._Game.Scripts.Items;
using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public class EmptyCodec : IDataCodec
    {
        public string Type => "Empty";

        public bool CanEncode(object data)
        {
            return data is EmptyInstanceData;
        }

        public object Decode(EncodedSaveData save)
        {
            return JsonConvert.DeserializeObject<EmptyInstanceData>(save.Json);
        }

        public EncodedSaveData Encode(object data)
        {
            var d = (EmptyInstanceData)data;
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(d)
            };
        }
    }
}
