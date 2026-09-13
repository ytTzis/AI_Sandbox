using UnityEngine;

namespace AISandbox.Traps
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovingMechanism : MonoBehaviour
    {
        [SerializeField] private Vector3 localMoveOffset = new Vector3(6f, 0f, 0f);
        [SerializeField] private float cycleSeconds = 2.5f;

        private Rigidbody body;
        private Vector3 startPosition;
        private Vector3 worldMoveOffset;
        private Vector3 currentVelocity;
        private float elapsedTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            startPosition = body.position;
            worldMoveOffset = transform.TransformDirection(localMoveOffset);
        }

        private void FixedUpdate()
        {
            elapsedTime += Time.fixedDeltaTime;
            float t = Mathf.PingPong(elapsedTime / Mathf.Max(0.1f, cycleSeconds), 1f);
            Vector3 targetPosition = startPosition + worldMoveOffset * t;

            currentVelocity = (targetPosition - body.position) / Time.fixedDeltaTime;
            body.MovePosition(targetPosition);
        }

        internal Vector3 GetVelocity()
        {
            return currentVelocity;
        }
    }
}
