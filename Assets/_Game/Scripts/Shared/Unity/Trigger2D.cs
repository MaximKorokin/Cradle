using System;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Unity
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Trigger2D : MonoBehaviour
    {
        public event Action<Collider2D> OnTriggerEntered;
        public event Action<Collider2D> OnTriggerExited;

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEntered?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExited?.Invoke(other);
        }
    }
}
