using Assets._Game.Scripts.UI.DataAggregators;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class QuestsViewController : ViewControllerBase<QuestsView>
    {
        private QuestsViewData _questsViewData;

        public void Bind(QuestsViewData questsViewData)
        {
            _questsViewData = questsViewData;
            _questsViewData.Changed += OnQuestsChanged;
        }

        public void Unbind()
        {
            if (_questsViewData == null) return;

            _questsViewData.Changed -= OnQuestsChanged;
            _questsViewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_questsViewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnQuestsChanged()
        {
            Render();
        }
    }
}
