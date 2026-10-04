using Assets._Game.Scripts.Locations;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public interface ILocationTransitionListViewData : IDataAggregator
    {
        IReadOnlyList<LocationTransitionData> Transitions { get; }
        void SetTransitions(IReadOnlyList<LocationTransitionData> transitions);
    }

    public class LocationTransitionListViewData : DataAggregatorBase, ILocationTransitionListViewData
    {
        private List<LocationTransitionData> _transitions = new();

        public IReadOnlyList<LocationTransitionData> Transitions => _transitions.AsReadOnly();

        public void SetTransitions(IReadOnlyList<LocationTransitionData> transitions)
        {
            _transitions = transitions.ToList();
            NotifyChanged();
        }
    }
}
