using Assets._Game.Scripts.Quests;
using Assets._Game.Scripts.UI.DataFormatters;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface IQuestDescriptionViewData : IDataAggregator
    {
        QuestStateDisplayData QuestData { get; }
        void SetQuestState(QuestState questState);
    }

    public class QuestDescriptionViewData : DataAggregatorBase, IQuestDescriptionViewData
    {
        private readonly QuestStateFormatter _questStateFormatter;
        private QuestState _questState;
        private QuestStateDisplayData _questData;

        public QuestStateDisplayData QuestData => _questData;

        public QuestDescriptionViewData(QuestStateFormatter questStateFormatter)
        {
            _questStateFormatter = questStateFormatter;
        }

        public void SetQuestState(QuestState questState)
        {
            if (_questState != null)
            {
                _questState.Updated -= OnQuestUpdated;
            }

            _questState = questState;

            if (_questState != null)
            {
                _questState.Updated += OnQuestUpdated;
                UpdateQuestData();
            }
        }

        private void OnQuestUpdated(QuestState quest)
        {
            UpdateQuestData();
        }

        private void UpdateQuestData()
        {
            if (_questState == null)
            {
                _questData = default;
            }
            else
            {
                _questData = _questStateFormatter.FormatData(_questState);
            }

            NotifyChanged();
        }

        public override void Dispose()
        {
            if (_questState != null)
            {
                _questState.Updated -= OnQuestUpdated;
            }

            base.Dispose();
        }
    }
}
