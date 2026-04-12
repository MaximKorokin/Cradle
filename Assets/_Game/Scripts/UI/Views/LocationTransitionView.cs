using Assets._Game.Scripts.Infrastructure.Game;
using Assets._Game.Scripts.Infrastructure.Systems.Location;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class LocationTransitionView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _locationNameText;
        [SerializeField]
        private Button _button;

        private IGlobalEventBus _globalEventBus;

        private string _targetLocationId;
        private string _targetEntranceId;

        [Inject]
        private void Construct(IGlobalEventBus globalEventBus)
        {
            _globalEventBus = globalEventBus;
        }

        public void Show(string targetLocationName, string targetLocationId, string targetEntranceId)
        {
            _targetLocationId = targetLocationId;
            _targetEntranceId = targetEntranceId;

            _locationNameText.text = targetLocationName;
            _button.gameObject.SetActive(true);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void Hide()
        {
            _button.gameObject.SetActive(false);

            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            _globalEventBus.Publish(new LocationTransitionRequest(_targetLocationId, _targetEntranceId));
        }
    }
}
