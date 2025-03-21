using System;
using UnityEngine;
using UnityEngine.AI;

public class Sinj : MonoBehaviour
{
    [SerializeField, Range(1, 5)] private float distanceToReact;
    
    private NavMeshAgent m_navMeshAgent;
    
    // Fleeing
    private bool m_fleeing;
    private Vector3 m_fleeingTarget;

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceToReact);
        if(m_fleeing)
            Gizmos.DrawRay(transform.position, m_fleeingTarget);
    }
    
    public void Init()
    {
        // nothing now
    }
    
    public void ReactToMouseDistance(float distance, Vector3 mousePosition)
    {
        m_fleeing = distance < distanceToReact * distanceToReact;
        
        if (m_fleeing)
        {
            // gérer la 2D
            float distanceToFlee = distanceToReact*distanceToReact - distance;
            Vector3 directionToFlee = (transform.position - mousePosition).normalized;
            m_fleeingTarget = distanceToFlee * directionToFlee;
            m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
        }
    }
}