using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views.Widgets
{
    public sealed class LocationAnnounceWidget : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private TMP_Text _text;

        public void Show(string locationName)
        {
            _text.text = locationName;
            _animator.SetTrigger("ToShow");
        }
    }
}
