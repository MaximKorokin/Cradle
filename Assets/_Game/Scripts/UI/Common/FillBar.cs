using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    public class FillBar : MonoBehaviour
    {
        [field: SerializeField]
        public Image BackgroundImage { get; set; }
        [field: SerializeField]
        public Image ForegroundImage { get; set; }
        [field: SerializeField]
        public Image BarImage { get; set; }
        [field: Header("Smooth")]
        [field: SerializeField]
        public Image SmoothBarImage { get; set; }
        [field: SerializeField]
        public float SmoothSpeed { get; set; }
        [field: Header("Gradient")]
        [field: SerializeField]
        public bool UseGradient { get; set; }
        [field: SerializeField]
        public Gradient Gradient { get; set; }
        [field: Header("Fade")]
        [field: SerializeField]
        public float TimeToStartFade { get; set; }
        [field: SerializeField]
        public float TimeToFade { get; set; }

        private const float DefaultAlphaValue = 1;

        public float CurrentFillRatio { get; private set; }
        private float _currentStartFadeTime;

        public void SetFillRatio(float value)
        {
            if (BarImage == null || value < 0 || value > 1 || value is float.NaN)
            {
                return;
            }
            if (TimeToFade > 0)
            {
                _currentStartFadeTime = Time.time + TimeToStartFade;
                IncrementAlphaValue(DefaultAlphaValue);
            }

            CurrentFillRatio = value;
            BarImage.fillAmount = CurrentFillRatio;
            if (UseGradient)
            {
                BarImage.color = Gradient.Evaluate(CurrentFillRatio);
            }
        }

        private void Update()
        {
            SetSmoothBar();
            Fade();
        }

        private void SetSmoothBar()
        {
            if (SmoothBarImage == null)
            {
                return;
            }
            var fillDifference = CurrentFillRatio - SmoothBarImage.fillAmount;
            if (fillDifference != 0)
            {
                var fillStep = (fillDifference > 0 ? 1 : -1) * SmoothSpeed * Time.deltaTime;
                if (Mathf.Abs(fillDifference) - Mathf.Abs(fillStep) > 0)
                {
                    SmoothBarImage.fillAmount += fillStep;
                }
                else
                {
                    SmoothBarImage.fillAmount = CurrentFillRatio;
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
            if (BarImage != null)
            {
                SetAlpha(BarImage, newAlpha);
            }
            if (SmoothBarImage != null)
            {
                SetAlpha(SmoothBarImage, newAlpha);
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
