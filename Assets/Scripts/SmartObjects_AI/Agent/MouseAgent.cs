using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsDisplayCap;

        private SmartAgent m_smartAgent;
        private AnimationAgent m_animationAgent;

        private float m_emotionDecrease;
        private float m_parameterValue;
        private Dictionary<AgentDynamicParameter, float> m_currentMouseParameters;
        
        public void Init(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
            m_smartAgent = GetComponent<SmartAgent>();
            m_animationAgent = GetComponent<AnimationAgent>();

            m_currentMouseParameters = new()
            {
                { AgentDynamicParameter.Curiosity, 0 },
                { AgentDynamicParameter.Aggression, 0 },
                { AgentDynamicParameter.Fear, 0 },
            };
            
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

            UpdateAnimationEmotion();
        }

        private void UpdateAnimationEmotion()
        {
            KeyValuePair<AgentDynamicParameter, float> maxEmotion = m_currentMouseParameters.Aggregate((a, b) => a.Value > b.Value ? a : b);
            if(maxEmotion.Value < emotionsDisplayCap[maxEmotion.Key])
                m_animationAgent.ResetMood();
            else
                m_animationAgent.SwitchMood(maxEmotion.Key);
        }
        
        private void AttenuateDynamicParameter(AgentDynamicParameter parameter)
        {
            m_parameterValue = GetDynamicParameterValue(parameter);
            m_emotionDecrease  = emotionsDecrease[parameter];

            m_parameterValue -= m_emotionDecrease;
            m_smartAgent.SetDynamicParameter(parameter, m_parameterValue);

            if(parameter == AgentDynamicParameter.Tension)
                return;
            
            m_currentMouseParameters[parameter] = m_smartAgent.GetDynamicParameter(parameter);
        }

        public float DistanceToMouse()
        {
            return m_mouseManager.ObjectDistanceToMouse(transform.position);
        }
        public float MouseVelocity()
        {
            return m_mouseManager.MouseVelocity();
        }

        public float GetDynamicParameterValue(AgentDynamicParameter parameter)
        {
            return m_smartAgent.GetDynamicParameter(parameter);
        }

        public void AddDynamicParameterValue(AgentDynamicParameter parameter, float value)
        {
            m_smartAgent.AddDynamicParameter(parameter, value);
        }

        public void SetDynamicParameterValue(AgentDynamicParameter parameter, float value)
        {
            m_smartAgent.SetDynamicParameter(parameter, value);
        }
    }
}
