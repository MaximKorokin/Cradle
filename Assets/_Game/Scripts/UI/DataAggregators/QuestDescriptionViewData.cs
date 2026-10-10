using Assets._Game.Scripts.Entities;
using Assets._Game.Scripts.Entities.Modules;
using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class QuestDescriptionViewData : EntityBoundDataAggregatorBase
    {
        private readonly QuestDefinitionCatalog _questDefinitionCatalog;
        private readonly QuestStateFormatter _questStateFormatter;

        private QuestModule _questModule;
        private string _questId;

        public QuestStateDisplayData QuestData { get; private set; }

        public QuestDescriptionViewData(
            EntityRepository entityRepository,
            QuestDefinitionCatalog questDefinitionCatalog,
            QuestStateFormatter questStateFormatter) : base(entityRepository)
        {
            _questDefinitionCatalog = questDefinitionCatalog;
            _questStateFormatter = questStateFormatter;
        }

        protected override void OnBoundEntityChanged()
        {
            if (_questModule != null)
            {
                _questModule.QuestUpdated -= OnQuestUpdated;
            }
            _questModule = Entity?.GetModule<QuestModule>();
            if (_questModule != null)
            {
                _questModule.QuestUpdated += OnQuestUpdated;
            }
        }

        public void SetQuestId(string questId)
        {
            _questId = questId;
            var questState = _questModule != null
                ? _questModule.GetQuestStateSnapshot(_questId)
                : new QuestStateSnapshot(_questDefinitionCatalog.Get(_questId));

            OnQuestUpdated(questState);
        }

        private void OnQuestUpdated(QuestStateSnapshot questState)
        {
            if (questState.Equals(default) || questState.Definition == null || questState.Definition.Id != _questId)
            {
                QuestData = default;
            }
            else
            {
                QuestData = _questStateFormatter.FormatData(questState);
            }

            NotifyChanged();
        }

        public override void Dispose()
        {
            if (_questModule != null)
            {
                _questModule.QuestUpdated -= OnQuestUpdated;
            }

            base.Dispose();
        }
    }
}
