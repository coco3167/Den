using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [Serializable, RequireComponent(typeof(MovementAgent), typeof(AnimationAgent))]
    public class SmartAgent : MonoBehaviour, IGameStateListener, IReloadable, IDebugDisplayAble
    {
        private const float AIUpdateSleepTime = 0.1f;
        
        [SerializeField] private SmartAgentData data;

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> dynamicParametersStartValue;
        [SerializeField, ReadOnly] private SerializedDictionary<AgentDynamicParameter, float> dynamicParameters = new();

        private SmartObject[] m_smartObjects, m_smartObjectsOwning;
        private SmartObject m_previousSmartObject, m_smartObjectToUse;

        private Dictionary<SmartObject, float> m_smartObjectScore = new();
        private DebugParameter[] m_debugParameters;
        
        private MovementAgent m_movementAgent;
        public AnimationAgent animationAgent { get; private set; }
        
        private void Awake()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            m_debugParameters = new DebugParameter[m_smartObjects.Length];
            
            for (var loop = 0; loop < m_smartObjects.Length; loop++)
            {
                DebugParameter debugParameter = new DebugParameter(m_smartObjects[loop].name, "0");
                m_debugParameters.SetValue(debugParameter, loop);
            }
            
            m_movementAgent = GetComponent<MovementAgent>();
            animationAgent = GetComponent<AnimationAgent>();
            
            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(AgentDynamicParameter)).Length; loop++)
            {
                AgentDynamicParameter parameter = (AgentDynamicParameter)loop;

                dynamicParameters.Add(parameter,
                    dynamicParametersStartValue.GetValueOrDefault(parameter, 0.0f));
            }
            
            //Owning SmartObjects
            m_smartObjectsOwning = GetComponentsInChildren<SmartObject>();
        }
        
        public void OnGameReady(object sender, EventArgs eventArgs)
        {
            SearchForSmartObject();
            m_previousSmartObject = m_smartObjectToUse;
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
                SetDynamicParameter(x, dynamicParametersStartValue.GetValueOrDefault(x));
            });
        }

        private void AIUpdate()
        {
            DynamicParameterVariation();
            
            TryToUseSmartObject();
        }

        private void TryToUseSmartObject()
        {
            SearchForSmartObject();

            m_movementAgent.SetDestination(m_smartObjectToUse.usingPoint);
                
            if (m_previousSmartObject != m_smartObjectToUse)
            {
                m_previousSmartObject.FinishUse(this);
                m_previousSmartObject = m_smartObjectToUse;
                animationAgent.FinishUseAnimation();
            }

            if (m_movementAgent.IsCloseToDestination())
            {
                m_smartObjectToUse.Use(this);
            }
        }

        private void SearchForSmartObject()
        {
            m_smartObjectScore.Clear();
            for (int loop = 0; loop < m_smartObjects.Length; loop++)
            {
                SmartObject smartObject = m_smartObjects[loop];
                m_smartObjectScore.Add(smartObject, smartObject.CalculateScore(this));
                smartObject.DynamicParameterVariation();
                
                m_debugParameters[loop].UpdateValue(m_smartObjectScore[smartObject].ToString("0.00"));
            }
            
            m_smartObjectToUse = m_smartObjectScore.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;
            m_debugParameters[m_smartObjectScore.Keys.ToList().IndexOf(m_smartObjectToUse)].IsSpecial = true;
        }
        
        public bool IsUsing(SmartObject smartObject)
        {
            // If the object was used last time and tries to be used this time => it's being used
            if (m_smartObjectToUse == m_previousSmartObject)
                return m_smartObjectToUse == smartObject;
            return false;
        }
        
        public bool IsOwner(SmartObject smartObject)
        {
            return m_smartObjectsOwning.Contains(smartObject);
        }

        

        #region DynamicParameters
        public float GetDynamicParameter(AgentDynamicParameter parameter)
        {
            return dynamicParameters[parameter];
        }
        
        public void SetDynamicParameter(AgentDynamicParameter parameter, float value)
        {
            dynamicParameters[parameter] = Math.Clamp(value, 0, 100);
        }

        public void AddDynamicParameter(AgentDynamicParameter parameter, float value)
        {
            SetDynamicParameter(parameter, dynamicParameters[parameter] + value);
        }
        
        private void DynamicParameterVariation()
        {
            foreach (AgentDynamicParameter parameter in data.dynamicParametersVariation.Keys)
            {
                AddDynamicParameter(parameter, data.dynamicParametersVariation[parameter]);
            }
        }
        
        #endregion

        public int GetParameterCount()
        {
            return m_debugParameters.Length;
        }

        public DebugParameter GetParameter(int index)
        {
            return m_debugParameters[index];
        }
    }
}