using System.Collections.Generic;
using DebugHUD;
using UnityEngine;
using UnityEngine.AI;

public class Sinj : MonoBehaviour, IDebugDisplayAble
{
    [SerializeField] private List<SinjBehavior> behaviors;
    
    private NavMeshAgent m_navMeshAgent;
    private MouseManager m_mouseManager;
    
    private List<DebugParameter> m_debugParameters = new();
    
    // Fleeing
    private bool m_fleeing;
    private Vector3 m_fleeingTarget;

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_debugParameters.Add(new DebugParameter("Health", 0));
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

    public int GetParameterCount()
    {
        return m_debugParameters.Count;
    }

    public DebugParameter GetParameter(int index)
    {
        return m_debugParameters[index];
    }
}