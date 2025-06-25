using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SmartObjects_AI.Agent
{
    [Serializable, RequireComponent(typeof(MovementAgent))]
    public class SmartAgent : MonoBehaviour, IGameStateListener, IReloadable, IDebugDisplayAble
    {
        private const float AIUpdateSleepTime = 0.1f;

        [SerializeField] private SmartAgentData data;
        [SerializeField] private float agentDecisionFlexibility;

        [SerializeField] private SmartObject groomingObject;
        [SerializeField] private SmartObject fightObject;

        [field: SerializeField] public AnimationAgent animationAgent { get; private set; }

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> dynamicParametersStartValue;
        [SerializeField, ReadOnly] private SerializedDictionary<AgentDynamicParameter, float> dynamicParameters = new();

        // Score and SmartObjects
        private SmartObject[] m_smartObjects;
        private SmartObject[] m_smartObjectsOwning;
        private SmartObject m_currentSmartObject;
        private Dictionary<SmartObject, float> m_smartObjectScore = new();
        private KeyValuePair<SmartObject, float> m_smartObjectToUse;

        private DebugParameter[] m_debugParameters = { };

        private MovementAgent m_movementAgent;

        private void Awake()
        {
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
            Init();
            SearchForSmartObject();
            m_currentSmartObject = m_smartObjectToUse.Key;
            m_movementAgent.SetDestination(m_currentSmartObject.usingPoint, m_currentSmartObject.ShouldRun());
            InvokeRepeating(nameof(AIUpdate), 0, AIUpdateSleepTime);
        }

        public void OnGameEnded(object sender, EventArgs eventArgs)
        {
            m_smartObjects = null;
            m_debugParameters = null;
            m_currentSmartObject = null;
            m_movementAgent.ResetDestination();
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

        private void Init()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            m_debugParameters = new DebugParameter[m_smartObjects.Length];

            m_movementAgent.Activate(true);

            for (var loop = 0; loop < m_smartObjects.Length; loop++)
            {
                DebugParameter debugParameter = new DebugParameter(m_smartObjects[loop].name, "0");
                m_debugParameters.SetValue(debugParameter, loop);
            }
        }

        private void AIUpdate()
        {
            AddDynamicParameter(AgentDynamicParameter.UsableFear, fightObject.GetDynamicParameter(SmartObjectParameter.Fear));

            DynamicParameterVariation();

            TryToUseSmartObject();
        }

        private void TryToUseSmartObject()
        {
            if (!m_movementAgent.IsActive())
            {
                m_currentSmartObject.Use(this);
                return;
            }

            SearchForSmartObject();

            SmartObject smartObjectToUse = m_smartObjectToUse.Key;
            bool isStillSameObject = smartObjectToUse == m_currentSmartObject;

            if (!isStillSameObject && animationAgent.IsAnimationReady())
            {
                m_currentSmartObject.FinishUse(this);
                m_currentSmartObject = smartObjectToUse;
            }

            m_movementAgent.SetDestination(m_currentSmartObject.usingPoint, m_currentSmartObject.ShouldRun());
            if (m_currentSmartObject.IsUsing(this) && !m_movementAgent.IsCloseToDestination())
            {
                animationAgent.FinishUseAnimation(true, false, false);
                m_currentSmartObject.FinishUse(this);
            }

            animationAgent.FinishUseAnimation(!(isStillSameObject && m_movementAgent.IsCloseToDestination()), smartObjectToUse.ShouldInterrupt(), m_currentSmartObject.IsInInterruptable());

            if (m_movementAgent.IsCloseToDestination())
            {
                if (animationAgent.IsAnimationReady())
                {
                    m_currentSmartObject.StartUse(this);
                }
                else if (m_currentSmartObject.IsUsing(this))
                    m_currentSmartObject.Use(this);
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
            if (m_currentSmartObject)
            {
                float previousScore = m_smartObjectScore[m_currentSmartObject];
                if (m_smartObjectToUse.Value < previousScore + agentDecisionFlexibility)
                    m_smartObjectToUse = new KeyValuePair<SmartObject, float>(m_currentSmartObject, previousScore);
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
            return m_currentSmartObject == smartObject;
        }

        public float CurrentScore()
        {
            return m_smartObjectToUse.Value;
        }

        public void SnapToPoint(Vector3 position)
        {
            m_movementAgent.Activate(false);
            transform.position = new Vector3(Random.Range(-.2f, .2f), 0, Random.Range(-.2f, .2f)) + position;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, Random.Range(0, 360), transform.rotation.eulerAngles.z));
            transform.DOScale(0.1f, 2).From().Play();
        }

        public void SetStoppingDistance(float distance)
        {
            m_movementAgent.SetStoppingDistance(distance);
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