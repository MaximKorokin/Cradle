using Assets._Game.Scripts.UI.Views.Widgets;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "FloatingTextConfig", menuName = "Configs/FloatingTextConfig")]
    public class FloatingTextConfig : ScriptableObject
    {
        public FloatingTextWidget FloatingTextPrefab;
        public Vector2 Offset;
        public Vector2 RandomOffset;

        [Space]
        public FloatingTextStyle DamageStyle;
        public FloatingTextStyle CriticalStyle;
        public FloatingTextStyle DamageReceiveStyle;
        public FloatingTextStyle HealStyle;
        public FloatingTextStyle ExperienceStyle;
    }

    [Serializable]
    public struct FloatingTextStyle
    {
        public Color Color;
        public float SizeScale;
        public float Duration;
    }
}
