using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class CodecRegistry
    {
        private readonly Dictionary<string, IDataCodec> _byType = new();

        public CodecRegistry(IEnumerable<IDataCodec> codecs)
        {
            foreach (var c in codecs)
            {
                if (_byType.ContainsKey(c.Type))
                {
                    SLog.Error($"Duplicate codec type '{c.Type}' found in registry.");
                    continue;
                }
                _byType[c.Type] = c;
            }
        }

        public EncodedSaveData EncodeOrNull(object data)
        {
            if (data == null) return null;

            foreach (var c in _byType.Values)
            {
                if (c.CanEncode(data))
                {
                    return c.Encode(data);
                }
            }

            return null;
        }

        public object DecodeOrNull(EncodedSaveData save, object payload = null)
        {
            if (save == null || !_byType.TryGetValue(save.Type, out var codec)) return null;

            return codec.Decode(save, payload);
        }
    }
}
