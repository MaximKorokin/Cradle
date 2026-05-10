namespace Assets._Game.Scripts.Infrastructure.Persistence.Codecs
{
    public sealed class EntityKillsObjectiveCodec : DataCodecBase<EntityKillsObjectiveProgressData>
    {
        public override string Type => "EntityKillsObjective";
    }

    public sealed class EntityKillsObjectiveProgressData
    {
        public string EntityId { get; set; }
        public int KillsAmount { get; set; }
    }
}
