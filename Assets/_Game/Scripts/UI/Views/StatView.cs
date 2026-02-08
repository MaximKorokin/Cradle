using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class StatView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _statName;
        [SerializeField]
        private TMP_Text _statValue;

        public void Render(string name, string value)
        {
            _statName.text = name;
            _statValue.text = value;
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
