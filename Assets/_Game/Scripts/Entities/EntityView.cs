using UnityEngine;

namespace Assets._Game.Scripts.Entities
{
    public sealed class EntityView : MonoBehaviour
    {
        [field: SerializeField]
        public Transform UnitsRoot { get; private set; }
        [field: SerializeField]
        public Animator UnitsAnimator { get; private set; }
    }
}
