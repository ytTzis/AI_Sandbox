using AISandbox.AI;
using UnityEngine;

namespace AISandbox.Traps
{
    public class KnockbackMechanism : MonoBehaviour
    {
        [SerializeField] private float forceMultiplier = 1f;
        [SerializeField] private bool stunAfterHit = true;
        [SerializeField] private float fallbackImpactSpeed = 7f;

        private void OnTriggerEnter(Collider other)
        {
            Hit(other);
        }

        private void Hit(Collider other)
        {
            SandboxAIStateMachine ai = other.GetComponentInParent<SandboxAIStateMachine>();
            if (ai == null)
            {
                return;
            }

            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 impactVelocity = GetImpactVelocity(contactPoint, other.bounds.center);
            impactVelocity *= Mathf.Max(0f, forceMultiplier);
            ai.ApplyHit(impactVelocity, stunAfterHit);
        }

        private Vector3 GetImpactVelocity(Vector3 contactPoint, Vector3 targetCenter)
        {
            RotatingMechanism rotatingMechanism = GetComponentInParent<RotatingMechanism>();
            if (rotatingMechanism != null)
            {
                Vector3 rotationalVelocity = rotatingMechanism.GetVelocityAtPoint(contactPoint);
                if (rotationalVelocity.sqrMagnitude > 0.0001f)
                {
                    return rotationalVelocity;
                }
            }

            MovingMechanism movingMechanism = GetComponentInParent<MovingMechanism>();
            if (movingMechanism != null)
            {
                Vector3 movementVelocity = movingMechanism.GetVelocity();
                if (movementVelocity.sqrMagnitude > 0.0001f)
                {
                    return movementVelocity;
                }
            }

            Vector3 fallbackDirection = targetCenter - transform.position;
            fallbackDirection.y = 0f;
            if (fallbackDirection.sqrMagnitude < 0.0001f)
            {
                fallbackDirection = transform.forward;
            }

            return fallbackDirection.normalized * Mathf.Max(0f, fallbackImpactSpeed);
        }
    }
}
