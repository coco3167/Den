using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sinj : MonoBehaviour
{
    [SerializeField] private List<SinjBehavior> behaviors;
    
    private NavMeshAgent m_navMeshAgent;
    private MouseManager m_mouseManager;
    
    // Fleeing
    private bool m_fleeing;
    private Vector3 m_fleeingTarget;

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        if(m_fleeing)
            Gizmos.DrawRay(transform.position, m_fleeingTarget);
    }
    
    public void Init(MouseManager mouseManager)
    {
        m_mouseManager = mouseManager;
    }
    
    private bool ReactToMouseDistance(SinjBehavior.MousePositionStimulus stimulus)
    {
        return m_mouseManager.ObjectDistanceToMouse(transform.position) < Mathf.Pow(stimulus.Distance, 2);
    }

    private void FleeReaction(SinjBehavior.FleeReaction reaction)
    {
        float distanceToFlee = reaction.Distance;
        Vector3 directionToFlee = (transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
        m_fleeingTarget = distanceToFlee * directionToFlee;
        m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
    }

    public void HandleStimuli()
    {
        foreach (SinjBehavior behavior in behaviors)
        {
            bool reactToStimuli = true;
            foreach (SinjBehavior.SinjStimulus stimulus in behavior.SinjStimuli)
            {
                if (stimulus is SinjBehavior.MousePositionStimulus mousePositionStimulus)
                {
                    if (!ReactToMouseDistance(mousePositionStimulus))
                    {
                        reactToStimuli = false;
                        break;
                    }
                }
            }

            if (!reactToStimuli)
                continue;

            foreach (SinjBehavior.SinjReaction reaction in behavior.SinjReactions)
            {
                if (reaction is SinjBehavior.FleeReaction fleeReaction)
                {
                    FleeReaction(fleeReaction);
                }
            }
        }
    }
}