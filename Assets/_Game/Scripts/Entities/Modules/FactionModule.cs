using Assets._Game.Scripts.Entities.Faction;

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

    public class FactionModuleFactory : IEntityModuleFactory
    {
        private readonly FactionRelations _factionRelations;

        public FactionModuleFactory(FactionRelations factionRelations)
        {
            _factionRelations = factionRelations;
        }

        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<FactionModuleDefinition>(out var factionModuleDefinition))
            {
                return null;
            }

            return new FactionModule(_factionRelations.Factions.IndexOf(factionModuleDefinition.Faction));
        }
    }
}
