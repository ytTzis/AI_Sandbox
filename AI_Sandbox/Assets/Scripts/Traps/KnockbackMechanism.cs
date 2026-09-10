using AISandbox.AI;
using UnityEngine;

namespace AISandbox.Traps
{
    public class KnockbackMechanism : MonoBehaviour
    {
        [SerializeField] private float forceMultiplier = 1f;
        [SerializeField] private bool stunAfterHit = true;

        private void OnTriggerEnter(Collider other)
        {
            Hit(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Hit(collision.collider);
        }

        private void Hit(Collider other)
        {
            SandboxAIStateMachine ai = other.GetComponentInParent<SandboxAIStateMachine>();
            if (ai == null)
            {
                return;
            }

            ai.ApplyHit(transform.position, forceMultiplier, stunAfterHit);
        }
    }
}
