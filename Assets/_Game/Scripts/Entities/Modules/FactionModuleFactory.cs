using Assets._Game.Scripts.Entities.Faction;

namespace Assets._Game.Scripts.Entities.Modules
{
    public class FactionModuleFactory
    {
        private readonly FactionRelations _factionRelations;

        public FactionModuleFactory(FactionRelations factionRelations)
        {
            _factionRelations = factionRelations;
        }

        public FactionModule Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<FactionModuleDefinition>(out var factionModuleDefinition))
            {
                return null;
            }

            return new(_factionRelations.Factions.IndexOf(factionModuleDefinition.Faction));
        }
    }
}
