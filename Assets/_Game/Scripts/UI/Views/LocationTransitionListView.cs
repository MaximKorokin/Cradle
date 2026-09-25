using Assets._Game.Scripts.Locations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class LocationTransitionListView : UIViewBase<IReadOnlyList<LocationTransitionData>>
    {
        [SerializeField]
        private Button _transitionButtonTemplate;
        [SerializeField]
        private Transform _buttonsRoot;

        private readonly List<Button> _transitionButtons = new();

        public event Action<LocationTransitionData> TransitionButtonClicked;

        protected override void Awake()
        {
            base.Awake();
            _transitionButtonTemplate.gameObject.SetActive(false);
        }

        public override void Render(IReadOnlyList<LocationTransitionData> transitions)
        {
            foreach (var button in _transitionButtons)
            {
                Destroy(button.gameObject);
            }

            foreach (var transition in transitions)
            {
                var button = Instantiate(_transitionButtonTemplate, _buttonsRoot);
                _transitionButtons.Add(button);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = $"{transition.LocationDefinition.DisplayName} ({transition.EntranceDefinition.DisplayName})";
                button.onClick.AddListener(() => TransitionButtonClicked?.Invoke(transition));
            }
        }
    }
}
