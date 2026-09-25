using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class StatWidget : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _statName;
        [SerializeField]
        private TMP_Text _statValue;

        public void Render((string Name, string Value) data)
        {
            _statName.text = data.Name;
            _statValue.text = data.Value;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            _statName.text = string.Empty;
            _statValue.text = string.Empty;
        }
    }
}
