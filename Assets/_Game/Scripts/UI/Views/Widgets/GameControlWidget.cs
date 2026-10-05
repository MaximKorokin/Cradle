using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class GameControlWidget : MonoBehaviour
    {
        [SerializeField]
        private Button _resetPlayerQuestsButton;
        [SerializeField]
        private Button _resetPlayerLevelButton;
        [SerializeField]
        private Button _resetWindowPositionsButton;

        public event Action ResetPlayerQuestsButtonClicked;
        public event Action ResetPlayerLevelButtonClicked;
        public event Action ResetWindowPositionsButtonClicked;

        private void Awake()
        {
            _resetPlayerQuestsButton.onClick.AddListener(OnResetPlayerQuestsButtonClicked);
            _resetPlayerLevelButton.onClick.AddListener(OnResetPlayerLevelButtonClicked);
            _resetWindowPositionsButton.onClick.AddListener(OnResetWindowPositionsButtonClicked);
        }

        private void OnResetPlayerQuestsButtonClicked()
        {
            ResetPlayerQuestsButtonClicked?.Invoke();
        }

        private void OnResetPlayerLevelButtonClicked()
        {
            ResetPlayerLevelButtonClicked?.Invoke();
        }

        private void OnResetWindowPositionsButtonClicked()
        {
            ResetWindowPositionsButtonClicked?.Invoke();
        }
    }
}
