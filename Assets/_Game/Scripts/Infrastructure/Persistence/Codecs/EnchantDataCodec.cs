using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class EnchantDataCodec : DataCodecBase<EnchantInstanceData>
    {
        public override string Type => "Enchant";

        public override EncodedSaveData Encode(object data)
        {
            var d = (EnchantInstanceData)data;
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(d.Level)
            };
        }

        public override object Decode(EncodedSaveData save, object payload)
        {
            if (payload is ItemDefinition itemDefinition && itemDefinition.TryGetTrait<EnchantableTrait>(out var enchantableTrait))
            {
                var enchantLevel = JsonConvert.DeserializeObject<int>(save.Json);
                var enchantInstanceData = new EnchantInstanceData(enchantLevel);
                return enchantInstanceData;
            }
            return null;
        }
    }
}
