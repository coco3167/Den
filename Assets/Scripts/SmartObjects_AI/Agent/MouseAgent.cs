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

        private MouseManager m_mouseManager;
        [SerializeField] private List<SinjActiveBehavior> mouseReactions;

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsDecrease;

        private SmartAgent m_smartAgent;

        private float m_emotionDecrease;
        private float m_parameterValue;
        
        public void Init(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
            m_smartAgent = GetComponent<SmartAgent>();
            InvokeRepeating(nameof(MouseReactionLoop), 0, UpdateTime);
        }

        private void MouseReactionLoop()
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
        }
        
        private void AttenuateDynamicParameter(AgentDynamicParameter parameter)
        {
            m_parameterValue = GetDynamicParameterValue(parameter).GetFloatValue();
            m_emotionDecrease  = emotionsDecrease[parameter];

            m_parameterValue -= m_emotionDecrease;
            m_parameterValue = Mathf.Clamp(m_parameterValue, 0, 100);
            m_smartAgent.dynamicParameters[parameter].SetValue(new ParameterValue(m_parameterValue));
        }

        public float DistanceToMouse()
        {
            return m_mouseManager.ObjectDistanceToMouse(transform.position);
        }
        public float MouseVelocity()
        {
            return m_mouseManager.MouseVelocity();
        }

        public ParameterValue GetDynamicParameterValue(AgentDynamicParameter parameter)
        {
            return m_smartAgent.dynamicParameters[parameter];
        }

        public void AddDynamicParameterValue(AgentDynamicParameter parameter, float value)
        {
            m_smartAgent.dynamicParameters[parameter].AddValue(new ParameterValue(value));
        }

        public void SetDynamicParameterValue(AgentDynamicParameter parameter, float value)
        {
            m_smartAgent.dynamicParameters[parameter].SetValue(new ParameterValue(value));
        }

    }
}
