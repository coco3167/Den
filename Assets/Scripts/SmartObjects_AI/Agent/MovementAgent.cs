using UnityEngine;
using UnityEngine.AI;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovementAgent : MonoBehaviour
    {
        private NavMeshAgent m_navMeshAgent;

        [SerializeField] private float walkSpeed, runSpeed;
        
        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetDestination(Transform destination, bool shouldRun)
        {
            m_navMeshAgent.speed = shouldRun ? runSpeed : walkSpeed;
            m_navMeshAgent.SetDestination(destination.position);
        }

        public void ResetDestination()
        {
            m_navMeshAgent.ResetPath();
        }
        
        public bool IsCloseToDestination()
        {
            return !m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance;
        }

        public float GetSpeed()
        {
            return m_navMeshAgent.velocity.magnitude;
        }

        public void StartAgent()
        {
            m_navMeshAgent.isStopped = false;
        }
        
        public void StopAgent()
        {
            m_navMeshAgent.isStopped = true;
        }

        public void Deactivate()
        {
            m_navMeshAgent.enabled = false;
        }

        public bool IsActive()
        {
            return m_navMeshAgent.enabled;
        }
    }
}
