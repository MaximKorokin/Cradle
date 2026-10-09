using Assets._Game.Scripts.Entities.StatusEffects;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.Systems.Tooltip;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public class StatusEffectWidget : MonoBehaviour, ITooltipSource
    {
        [SerializeField]
        private FillBar _cooldownFillBar;

        public StatusEffectDefinition StatusEffectDefinition { get; private set; }
        public float RemainingDuration { get; private set; }

        public void Render(StatusEffectSnapshot statusEffectSnapshot)
        {
            StatusEffectDefinition = statusEffectSnapshot.Definition;
            RemainingDuration = statusEffectSnapshot.RemainingDuration;

            _cooldownFillBar.ForegroundImage.sprite = statusEffectSnapshot.Definition.Icon;
            _cooldownFillBar.gameObject.SetActive(true);

            TickStatusEffectVisual();
        }

        private void Update()
        {
            TickStatusEffectVisual();
        }

        private void TickStatusEffectVisual()
        {
            if (StatusEffectDefinition == null) return;
            if (RemainingDuration > 0 && float.IsFinite(RemainingDuration))
            {
                RemainingDuration -= Time.deltaTime;
                _cooldownFillBar.SetFillRatio(RemainingDuration / StatusEffectDefinition.Duration);
            }
        }
    }
}
