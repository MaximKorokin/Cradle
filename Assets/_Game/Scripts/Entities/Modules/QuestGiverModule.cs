using Assets._Game.Scripts.Quests;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class QuestGiverModule : EntityModuleBase
    {
        public IReadOnlyList<QuestDefinition> OfferedQuests { get; }

        public QuestGiverModule(IReadOnlyList<QuestDefinition> offeredQuests)
        {
            OfferedQuests = offeredQuests;
        }
    }

    public sealed class QuestGiverModuleFactory : IEntityModuleFactory
    {
        public EntityModuleBase Create(EntityDefinition entityDefinition)
        {
            if (!entityDefinition.TryGetModuleDefinition<QuestGiverModuleDefinition>(out var definition))
                return null;

            return new QuestGiverModule(definition.OfferedQuests);
        }
    }
}
