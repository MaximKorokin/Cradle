namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class SpawnSourceModule : EntityModuleBase
    {
        public string SourceId { get; }

        public SpawnSourceModule(string sourceId)
        {
            SourceId = sourceId;
        }
    }
}
