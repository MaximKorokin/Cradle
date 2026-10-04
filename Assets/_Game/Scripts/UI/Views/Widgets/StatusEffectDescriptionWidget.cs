using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.DataFormatters;
using Assets.CoreScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class StatusEffectDescriptionWidget : MonoBehaviour
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

        [Inject]
        private void Construct(StatusEffectFormatter statusEffectFormatter)
        {
            _statusEffectFormatter = statusEffectFormatter;
        }

        private void Update()
        {
            RenderTimeLeft();
        }

        public void Render(StatusEffectDefinition statusEffectDefinition, float remainingDuration)
        {
            if (statusEffectDefinition == null) return;

            Clear();

            RenderHeader(statusEffectDefinition);
            RenderTimeLeft(statusEffectDefinition, remainingDuration);
            RenderEffects(statusEffectDefinition);
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

        private void RenderEffects(StatusEffectDefinition statusEffectDefinition)
        {
            var effectsText = _statusEffectFormatter.FormatData(statusEffectDefinition);
            if (string.IsNullOrEmpty(effectsText))
            {
                return;
            }
            _effectsInfo.SetActive(true);
            _effectsText.text = effectsText;
        }

        public void Clear()
        {
            gameObject.SetActive(false);

            _icon.sprite = null;
            _nameText.text = string.Empty;
            _timeLeftInfo.SetActive(false);
            _timeLeftText.text = string.Empty;
            _effectsInfo.SetActive(false);
            _effectsText.text = string.Empty;
        }
    }
}
