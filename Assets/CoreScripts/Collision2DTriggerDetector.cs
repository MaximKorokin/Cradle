using System;
using UnityEngine;

namespace Assets.CoreScripts
{
    public class Collision2DTriggerDetector : MonoBehaviourBase
    {
        public event Action<Collision2D> Collision2DEntered;
        public event Action<Collision2D> Collision2DExited;
        public event Action<Collider2D> Trigger2DEntered;
        public event Action<Collider2D> Trigger2DExited;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Collision2DEntered?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            Collision2DExited?.Invoke(collision);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Trigger2DEntered?.Invoke(collider);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            Trigger2DExited?.Invoke(collider);
        }
    }
}