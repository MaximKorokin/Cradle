using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Scripts.UI.Common
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LayoutElement))]
    public class LayoutElementMaxHeight : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _content;
        [SerializeField]
        private float _minHeight = 100f;
        [SerializeField]
        private float _maxHeight = 400f;

        private LayoutElement layoutElement;

        void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        void Update()
        {
            if (_content == null || layoutElement == null) return;

            var desiredHeight = _content.sizeDelta.y;
            layoutElement.preferredHeight = Mathf.Clamp(desiredHeight, _minHeight, _maxHeight);
        }
    }

}
