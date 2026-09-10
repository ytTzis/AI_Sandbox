using UnityEngine;

namespace AISandbox.Traps
{
    public class RotatingMechanism : MonoBehaviour
    {
        [SerializeField] private Vector3 localAxis = Vector3.up;
        [SerializeField] private float degreesPerSecond = 110f;

        private void Update()
        {
            transform.Rotate(localAxis.normalized, degreesPerSecond * Time.deltaTime, Space.Self);
        }
    }
}
