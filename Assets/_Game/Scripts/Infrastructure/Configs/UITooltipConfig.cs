using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Widgets;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "UITooltipConfig", menuName = "Configs/UITooltipConfig")]
    public sealed class UITooltipConfig : ScriptableObject
    {
        [field: SerializeField]
        public TextViewWidget TextViewPrefab { get; private set; }
        [field: SerializeField]
        public ItemStackPreviewView ItemStackPreviewViewPrefab { get; private set; }
        [field: SerializeField]
        public StatusEffectDescriptionWidget StatusEffectDescriptionViewPrefab { get; private set; }
    }
}
