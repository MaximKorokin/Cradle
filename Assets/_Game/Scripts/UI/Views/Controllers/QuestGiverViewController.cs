using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class QuestGiverViewController : ViewControllerBase<QuestGiverView>
    {
        private QuestGiverViewData _questGiverViewData;

        public void Bind(QuestGiverViewData questGiverViewData)
        {
            _questGiverViewData = questGiverViewData;
            _questGiverViewData.Changed += OnQuestGiverDataChanged;
        }

        public void Unbind()
        {
            if (_questGiverViewData == null) return;

            _questGiverViewData.Changed -= OnQuestGiverDataChanged;
            _questGiverViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_questGiverViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnQuestGiverDataChanged()
        {
            Render();
        }
    }
}
