using Assets._Game.Scripts.Locations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class LocationTransitionListView : MonoBehaviour
    {
        [SerializeField]
        private Button _transitionButtonTemplate;

        public void Bind(IReadOnlyList<LocationDefinition> locationDefinitions)
        {

        }
    }
}
