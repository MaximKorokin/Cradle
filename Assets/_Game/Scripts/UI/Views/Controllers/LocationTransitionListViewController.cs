using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Views.Controllers
{
    public sealed class LocationTransitionListViewController : ViewControllerBase<LocationTransitionListView>
    {
        private ILocationTransitionListViewData _viewData;

        public void Bind(ILocationTransitionListViewData viewData)
        {
            _viewData = viewData;
            _viewData.Changed += OnDataChanged;
        }

        public void Unbind()
        {
            if (_viewData == null) return;

            _viewData.Changed -= OnDataChanged;
            _viewData = null;
        }

        protected override void OnRender()
        {
            View.RequestRender(_viewData);
        }

        public override void Dispose()
        {
            Unbind();
            base.Dispose();
        }

        private void OnDataChanged()
        {
            Render();
        }
    }
}
