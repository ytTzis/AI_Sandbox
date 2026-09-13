using UnityEngine;
using UnityEngine.AI;

namespace AISandbox.AI
{
    public enum SandboxAIState
    {
        Patrol,
        Chase,
        Hit,
        Stunned
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class SandboxAIStateMachine : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] patrolPoints;

        [Header("Sensing")]
        [SerializeField] private float chaseRange = 8f;
        [SerializeField] private float stopChaseRange = 12f;

        [Header("State Timing")]
        [SerializeField] private float hitDuration = 0.45f;
        [SerializeField] private float stunDuration = 0.8f;

        private NavMeshAgent agent;
        private SandboxAIState currentState = SandboxAIState.Patrol;
        private int patrolIndex;
        private float stateTimer;
        private Vector3 knockbackVelocity;
        private bool stunAfterCurrentHit;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.freezeRotation = true;
        }

        private void Start()
        {
            EnterPatrol();
        }

        private void Update()
        {
            switch (currentState)
            {
                case SandboxAIState.Patrol:
                    TickPatrol();
                    break;
                case SandboxAIState.Chase:
                    TickChase();
                    break;
                case SandboxAIState.Hit:
                    TickHit();
                    break;
                case SandboxAIState.Stunned:
                    TickStunned();
                    break;
            }
        }

        internal void ApplyHit(Vector3 impactVelocity, bool stunAfterHit)
        {
            impactVelocity.y = 0f;
            knockbackVelocity = impactVelocity;
            stateTimer = hitDuration;
            stunAfterCurrentHit = stunAfterHit;
            SetState(SandboxAIState.Hit);
        }

        private void TickPatrol()
        {
            if (CanSeeTarget(chaseRange))
            {
                EnterChase();
                return;
            }

            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }

            if (!CanNavigate())
            {
                return;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                TrySetDestination(patrolPoints[patrolIndex].position);
            }
        }

        private void TickChase()
        {
            if (target == null)
            {
                EnterPatrol();
                return;
            }

            if (!CanSeeTarget(stopChaseRange))
            {
                EnterPatrol();
                return;
            }

            TrySetDestination(target.position);
        }

        private void TickHit()
        {
            if (CanNavigate())
            {
                agent.Move(knockbackVelocity * Time.deltaTime);
            }

            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 6f);

            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                if (stunAfterCurrentHit)
                {
                    EnterStunned();
                }
                else
                {
                    ResumeNormalState();
                }
            }
        }

        private void TickStunned()
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                ResumeNormalState();
            }
        }

        private void ResumeNormalState()
        {
            if (CanSeeTarget(chaseRange))
            {
                EnterChase();
            }
            else
            {
                EnterPatrol();
            }
        }

        private bool CanSeeTarget(float range)
        {
            return target != null && Vector3.Distance(transform.position, target.position) <= range;
        }

        private void EnterPatrol()
        {
            SetState(SandboxAIState.Patrol);

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                patrolIndex = Mathf.Clamp(patrolIndex, 0, patrolPoints.Length - 1);
                TrySetDestination(patrolPoints[patrolIndex].position);
            }
        }

        private void EnterChase()
        {
            SetState(SandboxAIState.Chase);
        }

        private void EnterStunned()
        {
            stateTimer = stunDuration;
            SetState(SandboxAIState.Stunned);
        }

        private void SetState(SandboxAIState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            currentState = nextState;
            if (CanNavigate())
            {
                bool stopNavigation = nextState == SandboxAIState.Hit
                    || nextState == SandboxAIState.Stunned;
                agent.isStopped = stopNavigation;

                if (stopNavigation)
                {
                    agent.ResetPath();
                }
            }
        }

        private bool CanNavigate()
        {
            return agent != null && agent.enabled && agent.isOnNavMesh;
        }

        private void TrySetDestination(Vector3 destination)
        {
            if (CanNavigate())
            {
                agent.SetDestination(destination);
            }
        }
    }
}
