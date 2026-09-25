using Assets._Game.Scripts.Locations;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class LocationTransitionListViewController : ViewControllerBase<LocationTransitionListView>
    {
        private IReadOnlyList<LocationTransitionData> _transitions;

        public void SetTransitions(IReadOnlyList<LocationTransitionData> transitions)
        {
            _transitions = transitions;
            Render();
        }

        protected override void OnRender()
        {
            View.RequestRender(_transitions);
        }
    }
}
