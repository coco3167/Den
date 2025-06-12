using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [Serializable, RequireComponent(typeof(MovementAgent))]
    public class SmartAgent : MonoBehaviour, IGameStateListener, IReloadable, IDebugDisplayAble
    {
        private const float AIUpdateSleepTime = 0.1f;
        
        [SerializeField] private SmartAgentData data;
        [SerializeField] private float agentDecisionFlexibility;
        
        [SerializeField] private SmartObject groomingObject;

        [field : SerializeField] public AnimationAgent animationAgent { get; private set; }
        
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> dynamicParametersStartValue;
        [SerializeField, ReadOnly] private SerializedDictionary<AgentDynamicParameter, float> dynamicParameters = new();

        // Score and SmartObjects
        private SmartObject[] m_smartObjects;
        [SerializeField, ReadOnly] private SmartObject[] m_smartObjectsOwning;
        private SmartObject m_previousSmartObject;
        private Dictionary<SmartObject, float> m_smartObjectScore = new();
        private KeyValuePair<SmartObject, float> m_smartObjectToUse;
        
        private DebugParameter[] m_debugParameters;
        
        private MovementAgent m_movementAgent;
        
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
            m_previousSmartObject = m_smartObjectToUse.Key;
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

            SmartObject smartObjectToUse = m_smartObjectToUse.Key;

            m_movementAgent.SetDestination(smartObjectToUse.usingPoint, smartObjectToUse.ShouldRun());

            if (m_previousSmartObject != smartObjectToUse && m_previousSmartObject.IsUsing(this))
            {
                m_previousSmartObject.FinishUse(this);
                animationAgent.FinishUseAnimation(smartObjectToUse.ShouldInterrupt());
            }
            m_previousSmartObject = smartObjectToUse;

            if (m_movementAgent.IsCloseToDestination())
            {
                if(animationAgent.IsAnimationReady())
                    smartObjectToUse.StartUse(this);
                else if(smartObjectToUse.IsUsing(this))
                    smartObjectToUse.Use(this);
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
            
            m_smartObjectToUse = m_smartObjectScore.Aggregate((a, b) => a.Value > b.Value ? a : b);

            // Dont change smartobject unless score remarkable score diff 
            if (m_previousSmartObject)
            {
                float previousScore = m_smartObjectScore[m_previousSmartObject]; 
                if (m_smartObjectToUse.Value < previousScore + agentDecisionFlexibility)
                    m_smartObjectToUse = new KeyValuePair<SmartObject, float>(m_previousSmartObject, previousScore);
            }

            groomingObject.IsUsable = m_smartObjectToUse.Key.IsRest() && m_smartObjectToUse.Key.IsUsing(this);
            
            m_debugParameters[m_smartObjectScore.Keys.ToList().IndexOf(m_smartObjectToUse.Key)].IsSpecial = true;
        }
        
        public bool IsOwner(SmartObject smartObject)
        {
            return m_smartObjectsOwning.Contains(smartObject);
        }

        public bool IsGoing(SmartObject smartObject)
        {
            return m_previousSmartObject == smartObject;
        }

        public float CurrentScore()
        {
            return m_smartObjectToUse.Value;
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

        public float GetBiggestEmotion()
        {
            return Math.Max(
                Math.Max(GetDynamicParameter(AgentDynamicParameter.Curiosity),
                    GetDynamicParameter(AgentDynamicParameter.Aggression)),
                GetDynamicParameter(AgentDynamicParameter.Fear));
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