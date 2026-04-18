using Assets._Game.Scripts.UI.Views;
using System;
using UnityEngine;

namespace Assets._Game.Scripts.Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "FloatingTextConfig", menuName = "Configs/FloatingTextConfig")]
    public class FloatingTextConfig : ScriptableObject
    {
        public FloatingTextView FloatingTextPrefab;
        public Vector2 Offset;

        [Space]
        public FloatingTextStyle DamageStyle;
        public FloatingTextStyle CriticalStyle;
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
