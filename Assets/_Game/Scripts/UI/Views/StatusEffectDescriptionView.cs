using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets.CoreScripts;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatusEffectDescriptionView : UIViewBase<StatusEffectViewData>
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private GameObject _timeLeftInfo;
        [SerializeField]
        private TMP_Text _timeLeftText;
        [SerializeField]
        private GameObject _effectsInfo;
        [SerializeField]
        private TMP_Text _effectsText;

        private readonly CooldownCounter _cooldownCounter = new();

        private StatusEffectFormatter _statusEffectFormatter;

        public StatusEffectCategory StatusEffectCategory { get; private set; }
        public int StatusEffectIndex { get; private set; }

        [Inject]
        private void Construct(StatusEffectFormatter statusEffectFormatter)
        {
            _statusEffectFormatter = statusEffectFormatter;
        }

        private void Update()
        {
            RenderTimeLeft();
        }

        public void Bind(StatusEffectCategory statusEffectCategory, int statusEffectIndex)
        {
            StatusEffectCategory = statusEffectCategory;
            StatusEffectIndex = statusEffectIndex;
        }

        protected override void Render(StatusEffectViewData data)
        {
            var statusEffectSnapshot = data.StatusEffectSnapshot;
            var definition = statusEffectSnapshot.Definition;
            if (definition == null) return;

            Clear();

            RenderHeader(definition);
            RenderTimeLeft(definition, statusEffectSnapshot.RemainingDuration);
            RenderEffects(statusEffectSnapshot);
        }

        private void RenderHeader(StatusEffectDefinition statusEffectDefinition)
        {
            _icon.sprite = statusEffectDefinition.Icon;
            _nameText.text = statusEffectDefinition.Name;
        }

        private void RenderTimeLeft(StatusEffectDefinition statusEffectDefinition, float remainingDuration)
        {
            if (statusEffectDefinition.Duration <= 0 || remainingDuration == float.PositiveInfinity)
            {
                return;
            }

            _cooldownCounter.Cooldown = statusEffectDefinition.Duration;
            _cooldownCounter.TimeSinceReset = statusEffectDefinition.Duration - remainingDuration;

            _timeLeftInfo.SetActive(true);
            RenderTimeLeft();
        }

        private void RenderTimeLeft()
        {
            if (!_timeLeftInfo.activeSelf) return;
            var timeLeft = Mathf.Max(0, _cooldownCounter.Cooldown - _cooldownCounter.TimeSinceReset);
            _timeLeftText.text = $"{timeLeft:F1}s";
        }

        private void RenderEffects(StatusEffectSnapshot statusEffectSnapshot)
        {
            var effectsText = _statusEffectFormatter.FormatData(statusEffectSnapshot);
            if (string.IsNullOrEmpty(effectsText))
            {
                return;
            }
            _effectsInfo.SetActive(true);
            _effectsText.text = effectsText;
        }

        public void Clear()
        {
            _icon.sprite = null;
            _nameText.text = string.Empty;
            _timeLeftInfo.SetActive(false);
            _timeLeftText.text = string.Empty;
            _effectsInfo.SetActive(false);
            _effectsText.text = string.Empty;
        }
    }
}
