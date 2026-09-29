using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class QuestDescriptionViewController : ViewControllerBase<QuestDescriptionView>
    {
        private IQuestDescriptionViewData _questDescriptionViewData;

        public void Bind(IQuestDescriptionViewData questDescriptionViewData)
        {
            _questDescriptionViewData = questDescriptionViewData;
            _questDescriptionViewData.Changed += OnQuestDataChanged;
        }

        public void Unbind()
        {
            if (_questDescriptionViewData == null) return;

            _questDescriptionViewData.Changed -= OnQuestDataChanged;
            _questDescriptionViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_questDescriptionViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnQuestDataChanged()
        {
            Render();
        }
    }
}
