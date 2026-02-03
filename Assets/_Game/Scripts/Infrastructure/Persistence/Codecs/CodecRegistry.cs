using System.Collections.Generic;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class CodecRegistry
    {
        private readonly Dictionary<string, IDataCodec> _byType = new();

        public CodecRegistry(IEnumerable<IDataCodec> codecs)
        {
            foreach (var c in codecs) _byType[c.Type] = c;
        }

        public EncodedSaveData EncodeOrNull(object data)
            => data == null ? null : FindEncoder(data).Encode(data);

        public object DecodeOrNull(EncodedSaveData save)
            => save == null ? null : _byType[save.Type].Decode(save);

        private IDataCodec FindEncoder(object data)
        {
            foreach (var c in _byType.Values)
                if (c.CanEncode(data))
                    return c;

            throw new KeyNotFoundException($"No codec for data type {data.GetType().Name}");
        }
    }
}
