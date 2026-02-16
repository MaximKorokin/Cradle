using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public class FillBar : MonoBehaviour
    {
        [field: SerializeField]
        public Image BackgroundImage { get; private set; }
        [field: SerializeField]
        public Image ForegroundImage { get; private set; }
        [field: SerializeField]
        public Image Fill { get; private set; }
        [field: SerializeField]
        public Image FillImage { get; private set; }
        [field: Header("Smooth")]
        [field: SerializeField]
        public Image SmoothFill { get; private set; }
        [field: SerializeField]
        public Image SmoothFillImage { get; private set; }
        [field: SerializeField]
        public float SmoothSpeed { get; private set; }
        [field: Header("Gradient")]
        [field: SerializeField]
        public bool UseGradient { get; private set; }
        [field: SerializeField]
        public Gradient Gradient { get; private set; }
        [field: Header("Fade")]
        [field: SerializeField]
        public float TimeToStartFade { get; private set; }
        [field: SerializeField]
        public float TimeToFade { get; private set; }

        private const float DefaultAlphaValue = 1;

        public float CurrentFillRatio { get; private set; }
        private float _currentStartFadeTime;

        public void SetFillRatio(float value)
        {
            if (FillImage == null || value < 0 || value > 1 || value is float.NaN)
            {
                return;
            }
            if (TimeToFade > 0)
            {
                _currentStartFadeTime = Time.time + TimeToStartFade;
                IncrementAlphaValue(DefaultAlphaValue);
            }

            CurrentFillRatio = value;
            Fill.fillAmount = CurrentFillRatio;
            if (UseGradient)
            {
                FillImage.color = Gradient.Evaluate(CurrentFillRatio);
            }
        }

        private void Update()
        {
            SetSmoothBar();
            Fade();
        }

        private void SetSmoothBar()
        {
            if (SmoothFillImage == null)
            {
                return;
            }
            var fillDifference = CurrentFillRatio - SmoothFill.fillAmount;
            if (fillDifference != 0)
            {
                var fillStep = (fillDifference > 0 ? 1 : -1) * SmoothSpeed * Time.deltaTime;
                if (Mathf.Abs(fillDifference) - Mathf.Abs(fillStep) > 0)
                {
                    SmoothFill.fillAmount += fillStep;
                }
                else
                {
                    SmoothFill.fillAmount = CurrentFillRatio;
                }
            }
        }

        private void Fade()
        {
            if (TimeToFade <= 0 || Time.time < _currentStartFadeTime)
            {
                return;
            }
            var fadeStep = -1 / TimeToFade * Time.deltaTime;
            IncrementAlphaValue(fadeStep);
        }

        private void IncrementAlphaValue(float alphaStep)
        {
            var newAlpha = Mathf.Clamp01(BackgroundImage.color.a + alphaStep);
            if (BackgroundImage != null)
            {
                SetAlpha(BackgroundImage, newAlpha);
            }
            if (ForegroundImage != null)
            {
                SetAlpha(ForegroundImage, newAlpha);
            }
            if (FillImage != null)
            {
                SetAlpha(FillImage, newAlpha);
            }
            if (SmoothFillImage != null)
            {
                SetAlpha(SmoothFillImage, newAlpha);
            }
        }

        private void SetAlpha(Image image, float alpha)
        {
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                alpha);
        }
    }
}
