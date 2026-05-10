using Assets._Game.Scripts.Items;

namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public class EmptyCodec : DataCodecBase<EmptyInstanceData>
    {
        public override string Type => "Empty";
    }
}
