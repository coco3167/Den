using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    public class SmartObject : MonoBehaviour, IReloadable
    {
        [NonSerialized] public bool IsUsable = true;
        
        [field: SerializeField] public Transform usingPoint { get; private set; }
        [field: SerializeField] public Transform lookingPoint { get; private set; }
        [SerializeField] private bool shouldSnapToLookingPoint;
        [SerializeField] private SmartObjectData data;
        [SerializeField] private JumpscareManager jumpscareManager;

        [SerializeField] private SerializedDictionary<SmartObjectParameter, float> dynamicParametersStartValue;
        [SerializeField, ReadOnly] private SerializedDictionary<SmartObjectParameter, float> dynamicParameters = new();

        private List<SmartAgent> m_startedUseList = new();
        private SmartObjectParameter[] m_keys;
        
        private void Awake()
        {
            if (!usingPoint)
                usingPoint = transform;
            
            data.Init();
            if (data.defaultLookingPoint == SmartObjectData.DefaultLookingPoint.Mouse)
            {
                lookingPoint = GameManager.Instance.GetMouseManager().GetMouseTransform();
                Debug.Log(lookingPoint, this);
            }

            // ReSharper disable once TooWideLocalVariableScope => no need to initialize multiple times
            SmartObjectParameter parameter;
            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(SmartObjectParameter)).Length; loop++)
            {
                parameter = (SmartObjectParameter)loop;

                dynamicParameters.Add(parameter,
                    dynamicParametersStartValue.GetValueOrDefault(parameter));
            }
        }

        private void Update()
        {
            if (jumpscareManager)
            {
                data.Update(jumpscareManager.Value);
            }
        }

        public void Reload()
        {
            m_keys = dynamicParameters.Keys.ToArray();
            m_keys.ForEach(x =>
            {
                SetDynamicParameter(x, dynamicParametersStartValue.GetValueOrDefault(x));
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(usingPoint.position, data.minRadius);
        }

        public float CalculateScore(SmartAgent smartAgent)
        {
            return data.scoreCalculation.CalculateScore(smartAgent, this);
        }

        public void StartUse(SmartAgent agent)
        {
            m_startedUseList.Add(agent);
            
            agent.animationAgent.SwitchAnimator(data, lookingPoint);
            agent.SetStoppingDistance(data.stoppingDistance);

            if (data.wwiseEvent.IsValid())
                data.wwiseEvent.Post(agent.gameObject);

            if (shouldSnapToLookingPoint)
                agent.SnapToPoint(lookingPoint.position);
        }

        public void FinishUse(SmartAgent agent)
        {
            m_startedUseList.Remove(agent);
        }

        public void Use(SmartAgent agent)
        {
            foreach (KeyValuePair<AgentDynamicParameter, float> parameterEffect in data.parameterEffectOnAgent)
            {
                agent.AddDynamicParameter(parameterEffect.Key, parameterEffect.Value);
            }

            foreach (KeyValuePair<SmartObjectParameter, float> parameterEffect in data.dynamicParametersEffect)
            {
                AddDynamicParameter(parameterEffect.Key, parameterEffect.Value);
            }
            
            if(data.shouldStopAgent)
                agent.animationAgent.StopMovementAgent();
        }

        public bool HasRoomForUse()
        {
            return data.maxUser > m_startedUseList.Count;
        }

        public bool IsUsing(SmartAgent agent)
        {
            return m_startedUseList.Contains(agent);
        }

        public bool ShouldRun()
        {
            return data.shouldRunTo;
        }

        public bool IsRest()
        {
            return data.IsRest();
        }

        public bool ShouldInterrupt()
        {
            return data.shouldInterruptNext;
        }

        public bool IsInInterruptable()
        {
            return data.inInterruptable;
        }

        /// <summary>
        /// Calculate how much the distance affects the agent want to use the object
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>Value between 0 and 1 representing proximity</returns>
        public float DistanceCoefficient(SmartAgent agent)
        {
            float distance = Vector3.Distance(agent.transform.position, transform.position);
            return 1 + 1/Math.Max(data.minRadius, distance);
        }

        #region DynamicParameters
        public float GetDynamicParameter(SmartObjectParameter parameter)
        {
            return dynamicParameters[parameter];
        }
        
        public void SetDynamicParameter(SmartObjectParameter parameter, float value)
        {
            dynamicParameters[parameter] = Math.Clamp(value, 0, 100);
        }

        public void AddDynamicParameter(SmartObjectParameter parameter, float value)
        {
            SetDynamicParameter(parameter, dynamicParameters[parameter] + value);
        }
        
        public void DynamicParameterVariation()
        {
            foreach (SmartObjectParameter parameter in data.dynamicParametersVariation.Keys)
            {
                AddDynamicParameter(parameter, data.dynamicParametersVariation[parameter]);
            }
        }
        #endregion
    }
}