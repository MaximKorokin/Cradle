using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Systems.Tooltip;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public class StatusEffectView : UIViewBase<StatusEffectViewData>, ITooltipSource
    {
        [SerializeField]
        private FillBar _cooldownFillBar;
        [SerializeField]
        private TMP_Text _chargesText;

        public StatusEffectSnapshot StatusEffectSnapshot { get; private set; }
        public float RemainingDuration { get; private set; }

        protected override void Render(StatusEffectViewData data)
        {
            var statusEffectSnapshot = data.StatusEffectSnapshot;

            StatusEffectSnapshot = statusEffectSnapshot;
            RemainingDuration = statusEffectSnapshot.RemainingDuration;

            _cooldownFillBar.ForegroundImage.sprite = statusEffectSnapshot.Definition.Icon;
            _cooldownFillBar.gameObject.SetActive(true);

            _chargesText.text = data.StatusEffectSnapshot.RemainingChargesAmount.ToString();
            _chargesText.gameObject.SetActive(data.StatusEffectSnapshot.RemainingChargesAmount > 0);

            TickStatusEffectVisual();
        }

        private void Update()
        {
            TickStatusEffectVisual();
        }

        private void TickStatusEffectVisual()
        {
            if (StatusEffectSnapshot.Definition == null) return;

            if (RemainingDuration > 0 && float.IsFinite(RemainingDuration))
            {
                RemainingDuration -= Time.deltaTime;
                _cooldownFillBar.SetFillRatio(RemainingDuration / StatusEffectSnapshot.Definition.Duration);
            }
        }
    }
}
