using UnityEngine;

namespace AISandbox.Traps
{
    [RequireComponent(typeof(Rigidbody))]
    public class RotatingMechanism : MonoBehaviour
    {
        [SerializeField] private Vector3 localAxis = Vector3.up;
        [SerializeField] private float degreesPerSecond = 110f;

        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void FixedUpdate()
        {
            Vector3 axis = GetLocalAxis();
            Quaternion deltaRotation = Quaternion.AngleAxis(
                degreesPerSecond * Time.fixedDeltaTime,
                axis);
            body.MoveRotation(body.rotation * deltaRotation);
        }

        internal Vector3 GetVelocityAtPoint(Vector3 worldPoint)
        {
            Vector3 worldAxis = transform.TransformDirection(GetLocalAxis());
            Vector3 radius = worldPoint - body.worldCenterOfMass;
            radius -= Vector3.Project(radius, worldAxis);

            float radiansPerSecond = degreesPerSecond * Mathf.Deg2Rad;
            return Vector3.Cross(worldAxis, radius) * radiansPerSecond;
        }

        private Vector3 GetLocalAxis()
        {
            return localAxis.sqrMagnitude > 0.0001f
                ? localAxis.normalized
                : Vector3.up;
        }
    }
}
