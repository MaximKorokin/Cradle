using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CompactPlayerStateView : MonoBehaviour
    {
        [Header("HP")]
        [SerializeField]
        private FillBar _hpFillBar;
        [SerializeField]
        private TMP_Text _hpText;
        [Header("Level & Experience")]
        [SerializeField]
        private FillBar _experienceFillBar;
        [SerializeField]
        private TMP_Text _experienceText;
        [SerializeField]
        private TMP_Text _levelText;
        [Header("Status Effects")]
        [SerializeField]
        private FillBar _buffTemplate;
        [SerializeField]
        private RectTransform _buffsParent;
        [SerializeField]
        private FillBar _debuffTemplate;
        [SerializeField]
        private RectTransform _debuffsParent;

        private PlayerStateViewData _playerStateViewData;
        private readonly List<(FillBar FillBar, StatusEffectSnapshot Snapshot, float RemainingDuration)> _activeStatusEffects = new();

        private void Awake()
        {
            _buffTemplate.gameObject.SetActive(false);
            _debuffTemplate.gameObject.SetActive(false);
        }

        private void Update()
        {
            //foreach (var (fillBar, snapshot, remainingDuration) in _activeStatusEffects)
            for (int i = 0; i < _activeStatusEffects.Count; i++)
            {
                var (fillBar, snapshot, remainingDuration) = _activeStatusEffects[i];

                if (snapshot.IsEndless) continue;

                remainingDuration -= Time.deltaTime;
                _activeStatusEffects[i] = (fillBar, snapshot, remainingDuration);
                fillBar.SetFillRatio(remainingDuration / snapshot.Duration);
            }
        }

        public void Redraw(PlayerStateViewData playerStateViewData)
        {
            Clear();

            _playerStateViewData = playerStateViewData;
            _playerStateViewData.Changed += OnPlayerStateViewDataChanged;

            // HP
            _hpFillBar.SetFillRatio(_playerStateViewData.CurrentHp / _playerStateViewData.MaxHp);
            _hpText.text = $"{_playerStateViewData.CurrentHp} / {_playerStateViewData.MaxHp}";

            // Level & Experience
            _experienceFillBar.SetFillRatio(_playerStateViewData.Experience / 100);
            _experienceText.text = $"{_playerStateViewData.Experience}%";
            _levelText.text = $"{_playerStateViewData.Level}";

            // Buffs
            foreach (var buff in _playerStateViewData.Buffs)
            {
                var buffView = Instantiate(_buffTemplate, _buffsParent);
                _activeStatusEffects.Add((buffView, buff, buff.RemainingDuration));
                buffView.ForegroundImage.sprite = buff.Icon;
                buffView.gameObject.SetActive(true);
                buffView.SetFillRatio(buff.RemainingDuration / buff.Duration);
            }

            // Debuffs
            foreach (var debuff in _playerStateViewData.Debuffs)
            {
                var debuffView = Instantiate(_debuffTemplate, _debuffsParent);
                _activeStatusEffects.Add((debuffView, debuff, debuff.RemainingDuration));
                debuffView.ForegroundImage.sprite = debuff.Icon;
                debuffView.gameObject.SetActive(true);
                debuffView.SetFillRatio(debuff.RemainingDuration / debuff.Duration);
            }
        }

        private void OnPlayerStateViewDataChanged()
        {
            Redraw(_playerStateViewData);
        }

        public void Clear()
        {
            foreach (var (fillBar, _, _) in _activeStatusEffects)
            {
                if (fillBar != null) Destroy(fillBar.gameObject);
            }
            _activeStatusEffects.Clear();

            if (_playerStateViewData != null) _playerStateViewData.Changed -= OnPlayerStateViewDataChanged;
        }
    }
}
