using UnityEngine;

namespace Assets._Game.Scripts.UI.Core
{
    public class UIRootReferences : MonoBehaviour
    {
        [field: SerializeField]
        public RectTransform WindowsRoot { get; private set; }
    }
}
