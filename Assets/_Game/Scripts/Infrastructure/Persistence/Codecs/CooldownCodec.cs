using Assets._Game.Scripts.Items;
using Assets._Game.Scripts.Items.Traits;
using Newtonsoft.Json;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class CooldownCodec : DataCodecBase<CooldownInstanceData>
    {
        public override string Type => "Cooldown";

        public override EncodedSaveData Encode(object data)
        {
            var d = (CooldownInstanceData)data;
            return new EncodedSaveData
            {
                Type = Type,
                Json = JsonConvert.SerializeObject(d.CooldownCounter.TimeSinceReset)
            };
        }

        public override object Decode(EncodedSaveData save, object payload)
        {
            if (payload is ItemDefinition itemDefinition && itemDefinition.TryGetTrait<UsableTrait>(out var usableTrait))
            {
                var timeSinceReset = JsonConvert.DeserializeObject<float>(save.Json);
                var cooldownInstanceData = new CooldownInstanceData(usableTrait.Cooldown);
                cooldownInstanceData.CooldownCounter.TimeSinceReset = timeSinceReset;
                return cooldownInstanceData;
            }
            return null;
        }
    }
}
