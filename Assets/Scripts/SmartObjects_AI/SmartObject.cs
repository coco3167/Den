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
        [field: SerializeField, ChildGameObjectsOnly] public Transform usingPoint { get; private set; }
        [SerializeField] private float gizmosRadius;
        [SerializeField] private SmartObjectData data;

        [SerializeField] private SerializedDictionary<SmartObjectParameter, float> dynamicParametersStartValue;
        [field : SerializeField, ReadOnly] public SerializedDictionary<SmartObjectParameter, float> dynamicParameters { get; private set; } = new();

        private List<SmartAgent> m_startedUseList;
        private SmartObjectParameter[] m_keys;
        
        private void Awake()
        {
            if (!usingPoint)
                usingPoint = transform;

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

        public void Reload()
        {
            m_keys = dynamicParameters.Keys.ToArray();
            m_keys.ForEach(x =>
            {
                dynamicParameters[x] = dynamicParametersStartValue.GetValueOrDefault(x);
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(usingPoint.position, gizmosRadius);
        }

        public float CalculateScore(SmartAgent smartAgent)
        {
            return data.scoreCalculation.CalculateScore(smartAgent, this);
        }

        private void StartUse(AnimationAgent animationAgent)
        {
            animationAgent.SwitchAnimator(data.animatorController);
        }

        public void FinishUse(SmartAgent agent)
        {
            m_startedUseList.Remove(agent);
        }

        public void Use(SmartAgent agent)
        {
            if (!m_startedUseList.Contains(agent))
            {
                StartUse(agent.animationAgent);
                m_startedUseList.Add(agent);
                return;
            }
            
            foreach (KeyValuePair<AgentDynamicParameter, float> parameterEffect in data.parameterEffectOnAgent)
            {
                agent.UpdateParameterValue(parameterEffect.Key, parameterEffect.Value);
            }

            foreach (KeyValuePair<SmartObjectParameter, float> parameterEffect in data.dynamicParametersEffect)
            {
                dynamicParameters[parameterEffect.Key] += parameterEffect.Value;
            }
        }

        public bool HasRoomForUse()
        {
            return data.maxUser > m_startedUseList.Count;
        }

        public void DynamicParameterVariation()
        {
            foreach (SmartObjectParameter parameter in data.dynamicParametersVariation.Keys)
            {
                dynamicParameters[parameter] += data.dynamicParametersVariation[parameter];
            }
        }
    }
}