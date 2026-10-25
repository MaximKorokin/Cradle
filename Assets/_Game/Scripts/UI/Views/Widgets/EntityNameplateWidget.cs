using Assets._Game.Scripts.Shared.Extensions;
using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class EntityNameplateWidget : UIViewBase<EntityNameplateWidgetData>
    {
        [SerializeField]
        private RectTransform _rect;
        [SerializeField]
        private TMP_Text _levelText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private FillBar _healthFillBar;

        protected override void Awake()
        {
            base.Awake();
            transform.DisableRaycastTargets();
        }

        public void SetPosition(Vector3 screenPosition)
        {
            _rect.position = screenPosition;
        }

        public void SetVisible(bool value)
        {
            if (gameObject.activeSelf != value)
                gameObject.SetActive(value);
        }

        protected override void Render(EntityNameplateWidgetData data)
        {
            _levelText.text = data.Level;
            _nameText.text = data.Name;
            if (data.ShouldViewHealthBar)
            {
                _healthFillBar.SetFillRatio(data.HealthRatio);
            }
        }
    }
}
