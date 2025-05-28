using UnityEngine;
using UnityEngine.AI;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovementAgent : MonoBehaviour
    {
        private NavMeshAgent m_navMeshAgent;

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetDestination(Transform destination)
        {
            m_navMeshAgent.SetDestination(destination.position);
        }
        
        public bool IsCloseToDestination()
        {
            return !m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance;
        }

        public float GetSpeed()
        {
            return m_navMeshAgent.speed;
        }
    }
}
