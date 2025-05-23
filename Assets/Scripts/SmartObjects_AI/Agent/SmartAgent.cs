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

        private SmartObject[] m_smartObjects, m_smartObjectsOwning;
        private SmartObject m_previousSmartObject, m_smartObjectToUse;

        private Dictionary<SmartObject, float> m_smartObjectScore = new();
        
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

                dynamicParameters.Add(parameter,
                    dynamicParametersStartValue.TryGetValue(parameter, out ParameterValue value) ? value : new ParameterValue());
            }
            
            //Owning SmartObjects
            m_smartObjectsOwning.AddRange(GetComponentsInChildren<SmartObject>());
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
                dynamicParameters[x].SetValue(
                    dynamicParametersStartValue.TryGetValue(x, out ParameterValue value) ? value : new ParameterValue());
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

        public bool IsOwner(SmartObject smartObject)
        {
            return m_smartObjectsOwning.Contains(smartObject);
        }

        public bool IsUsing(SmartObject smartObject)
        {
            // If the object was used last time and tries to be used this time => it's being used
            if (m_smartObjectToUse == m_previousSmartObject)
                return m_smartObjectToUse == smartObject;
            return false;
        }

        public void UpdateParameterValue(AgentDynamicParameter parameter, ParameterValue value)
        {
            dynamicParameters[parameter].AddValue(value);
        }
    }
}