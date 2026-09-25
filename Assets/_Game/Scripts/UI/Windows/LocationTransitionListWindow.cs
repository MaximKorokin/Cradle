using Assets._Game.Scripts.Locations;
using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Windows
{
    public sealed class LocationTransitionListWindow : UIWindowBase
    {
        [SerializeField]
        private LocationTransitionListView _view;

        public LocationTransitionListView View => _view;

        public event Action<LocationTransitionData> TransitionButtonClicked
        {
            add => _view.TransitionButtonClicked += value;
            remove => _view.TransitionButtonClicked -= value;
        }

    }
}
