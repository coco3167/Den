
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Sinj;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(SmartAgent))]
    public class MouseAgent : MonoBehaviour
    {
        private const float UpdateTime = 0.1f;

        [SerializeField] private MouseManager mouseManager;
        [SerializeField] private List<SinjActiveBehavior> mouseReactions;

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsDecrease;

        private SmartAgent m_smartAgent;
        
        private void Awake()
        {
            m_smartAgent = GetComponent<SmartAgent>();
            StartCoroutine(MouseReactionLoop());
        }

        private IEnumerator MouseReactionLoop()
        {
            while (true)
            {
                foreach (SinjActiveBehavior mouseReaction in mouseReactions)
                {
                    if (mouseReaction.IsApplying(this))
                    {
                        mouseReaction.ApplyReaction(this);
                    }
                }
                
                AttenuateDynamicParameter(AgentDynamicParameter.Tension);
                AttenuateDynamicParameter(AgentDynamicParameter.Curiosity);
                AttenuateDynamicParameter(AgentDynamicParameter.Aggression);
                AttenuateDynamicParameter(AgentDynamicParameter.Fear);
                
                yield return new WaitForSeconds(UpdateTime);
            }
        }

        private void AttenuateDynamicParameter(AgentDynamicParameter parameter)
        {
            float value = GetDynamicParameterValue(parameter);
            float emotionDecrease = emotionsDecrease[parameter];
        
            value -= emotionDecrease;
            value = Mathf.Clamp(value, 0, 100);
            m_smartAgent.dynamicParameters[parameter] = value;
        }

        public float DistanceToMouse()
        {
            return mouseManager.ObjectDistanceToMouse(transform.position);
        }
        public float MouseVelocity()
        {
            return mouseManager.MouseVelocity();
        }

        public float GetDynamicParameterValue(AgentDynamicParameter parameter)
        {
            return m_smartAgent.dynamicParameters[parameter];
        }

        public void AddDynamicParameterValue(AgentDynamicParameter parameter, float value)
        {
            // TODO Tension is constantly increasing
            m_smartAgent.dynamicParameters[parameter] += value;
        }
    }
}
