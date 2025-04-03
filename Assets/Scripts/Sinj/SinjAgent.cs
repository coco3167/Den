using System.Collections.Generic;
using DebugHUD;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Sinj
{
    public class SinjAgent : MonoBehaviour, IDebugDisplayAble
    {
        [SerializeField] private List<SinjBehavior> behaviors;
        [SerializeField, ReadOnly] private float tension;
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;

        private readonly List<DebugParameter> m_debugParameters = new();
    
        // Fleeing
        private bool m_fleeing;
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_debugParameters.Add(new DebugParameter("Tension", "0"));
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

        public void HandleBehaviors()
        {
            foreach (SinjBehavior behavior in behaviors)
            {
                bool reactToStimuli = true;
                foreach (SinjBehavior.SinjStimulus stimulus in behavior.SinjStimuli)
                {
                    if (!stimulus.IsApplying(this))
                    {
                        reactToStimuli = false;
                        break;
                    }
                }

                if (!reactToStimuli)
                    continue;

                foreach (SinjBehavior.SinjReaction reaction in behavior.SinjReactions)
                {
                    reaction.ApplyReaction(this);
                }
            }
        }

        public void UpdateTension(float value)
        {
            tension = value;
            m_debugParameters[0].UpdateValue(((int)tension).ToString());
        }

        #region Getter
        public float DistanceToMouse()
        {
            return m_mouseManager.ObjectDistanceToMouse(transform.position);
        }

        public float MouseVelocity()
        {
            return m_mouseManager.MouseVelocity();
        }

        public float GetTension()
        {
            return tension;
        }
        #endregion

        #region Reactions
        public void FleeReaction(float distanceToFlee)
        {
            Vector3 directionToFlee = (transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
            m_fleeingTarget = distanceToFlee * directionToFlee;
            m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
        }

        public void AddTension(float amount)
        {
            tension += amount*Time.deltaTime;
        }
        #endregion

        #region Debug
        public int GetParameterCount()
        {
            return m_debugParameters.Count;
        }

        public DebugParameter GetParameter(int index)
        {
            return m_debugParameters[index];
        }
        #endregion
        
    }
}