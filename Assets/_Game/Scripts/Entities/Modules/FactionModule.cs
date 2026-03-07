namespace Assets._Game.Scripts.Entities.Modules
{
    public class FactionModule : EntityModuleBase
    {
        public int FactionId { get; }

        public FactionModule(int factionId)
        {
            FactionId = factionId;
        }
    }
}
