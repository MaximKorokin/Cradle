using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class EntityNameplateWidget : MonoBehaviour
    {
        private EntityNameplateWidgetData data;

        [SerializeField]
        private RectTransform _rect;
        [SerializeField]
        private TMP_Text _levelText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private FillBar _healthFillBar;

        public void Bind(EntityNameplateWidgetData entityNameplateViewData)
        {
            data = entityNameplateViewData;

            entityNameplateViewData.Changed += OnDataChanged;
            OnDataChanged();
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

        private void OnDataChanged()
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
