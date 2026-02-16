using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CompactPlayerStateView : MonoBehaviour
    {
        [SerializeField]
        private FillBar _hpFillBar;
        [SerializeField]
        private TMP_Text _hpText;
        [SerializeField]
        private FillBar _experienceFillBar;
        [SerializeField]
        private TMP_Text _experienceText;
        [SerializeField]
        private TMP_Text _levelText;

        private PlayerStateViewData _playerStateViewData;

        public void Redraw(PlayerStateViewData playerStateViewData)
        {
            Clear();

            _playerStateViewData = playerStateViewData;

            _playerStateViewData.Changed += OnPlayerStateViewDataChanged;
        }

        private void OnPlayerStateViewDataChanged()
        {

        }

        public void Clear()
        {
            _playerStateViewData.Changed -= OnPlayerStateViewDataChanged;
        }
    }
}
