using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class LocationTransitionListWindow : UIWindowBase
    {
        [SerializeField]
        private LocationTransitionListView _view;

        public event Action<LocationTransitionData> TransitionButtonClicked
        {
            add => _view.TransitionButtonClicked += value;
            remove => _view.TransitionButtonClicked -= value;
        }

        public void Render(IReadOnlyList<LocationTransitionData> transitions)
        {
            _view.Render(transitions);
        }
    }
}
