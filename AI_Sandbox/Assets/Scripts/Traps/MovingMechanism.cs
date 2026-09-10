using UnityEngine;

namespace AISandbox.Traps
{
    public class MovingMechanism : MonoBehaviour
    {
        [SerializeField] private Vector3 localMoveOffset = new Vector3(6f, 0f, 0f);
        [SerializeField] private float cycleSeconds = 2.5f;

        private Vector3 startPosition;

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            float t = Mathf.PingPong(Time.time / Mathf.Max(0.1f, cycleSeconds), 1f);
            transform.position = startPosition + transform.TransformDirection(localMoveOffset) * t;
        }
    }
}
