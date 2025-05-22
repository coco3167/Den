using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [Serializable, RequireComponent(typeof(MovementAgent), typeof(AnimationAgent))]
    public class SmartAgent : MonoBehaviour, IGameStateListener, IReloadable
    {
        private const float AIUpdateSleepTime = 0.1f;
        
        [SerializeField] private SmartAgentData data;

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, ParameterValue> dynamicParametersStartValue;
        [field : SerializeField, ReadOnly] public SerializedDictionary<AgentDynamicParameter, ParameterValue> dynamicParameters { get; private set; } = new();

        private SmartObject[] m_smartObjects;
        private SmartObject m_previousSmartObject, m_smartObjectToUse;

        private Dictionary<SmartObject, float> m_smartObjectScore;
        
        private MovementAgent m_movementAgent;
        public AnimationAgent animationAgent { get; private set; }
        
        private void Awake()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            m_movementAgent = GetComponent<MovementAgent>();
            animationAgent = GetComponent<AnimationAgent>();
            
            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(AgentDynamicParameter)).Length; loop++)
            {
                AgentDynamicParameter parameter = (AgentDynamicParameter)loop;
                
                if (dynamicParametersStartValue.TryGetValue(parameter, out ParameterValue value))
                    dynamicParameters.Add(parameter, value);
                else 
                    dynamicParameters.Add(parameter, new ParameterValue(0.0f)); // Need to know if parameter is float or bool
            }

        }
        
        public void OnGameReady(object sender, EventArgs eventArgs)
        {
            m_previousSmartObject = SearchForSmartObject();
            InvokeRepeating(nameof(AIUpdate), 0, AIUpdateSleepTime);
        }

        public void OnGameEnded(object sender, EventArgs eventArgs)
        {
            CancelInvoke(nameof(AIUpdate));
        }

        public void Reload()
        {
            transform.localPosition = Vector3.zero;
            
            AgentDynamicParameter[] keys = dynamicParameters.Keys.ToArray();
            keys.ForEach(x =>
            {
                if (dynamicParametersStartValue.TryGetValue(x, out ParameterValue value))
                    dynamicParameters[x].SetValue(value);
                else 
                    dynamicParameters[x].SetValue(new ParameterValue(0.0f));
            });
        }

        private void AIUpdate()
        {
            DynamicParameterVariation();
            
            TryToUseSmartObject();
        }

        private void DynamicParameterVariation()
        {
            foreach (AgentDynamicParameter parameter in data.dynamicParametersVariation.Keys)
            {
                dynamicParameters[parameter].AddValue(data.dynamicParametersVariation[parameter]);
            }
        }

        private void TryToUseSmartObject()
        {
            m_smartObjectToUse = SearchForSmartObject();

            m_movementAgent.SetDestination(m_smartObjectToUse.usingPoint);
                
            if (m_previousSmartObject != m_smartObjectToUse)
            {
                m_previousSmartObject.FinishUse();
                m_previousSmartObject = m_smartObjectToUse;
                animationAgent.FinishUseAnimation();
            }

            if (m_movementAgent.IsCloseToDestination())
            {
                m_smartObjectToUse.Use(this);
            }
        }

        private SmartObject SearchForSmartObject()
        {
            m_smartObjectScore.Clear();
            foreach (SmartObject smartObject in m_smartObjects)
            {
                m_smartObjectScore.Add(smartObject, smartObject.CalculateScore(this));
                smartObject.DynamicParameterVariation();
            }
            
            return m_smartObjectScore.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;
        }

        public void UpdateParameterValue(AgentDynamicParameter parameter, ParameterValue value)
        {
            dynamicParameters[parameter].AddValue(value);
        }
    }
}