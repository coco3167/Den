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

        [SerializeField] private SerializedDictionary<SmartObjectParameter, ParameterValue> dynamicParametersStartValue;
        public Dictionary<SmartObjectParameter, ParameterValue> dynamicParameters { get; private set; } = new();

        private bool m_startedUse;
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
                
                if (dynamicParametersStartValue.TryGetValue(parameter, out ParameterValue value))
                    dynamicParameters.Add(parameter, value);
                else 
                    dynamicParameters.Add(parameter, new ParameterValue(0.0f));
            }
        }

        public void Reload()
        {
            m_keys = dynamicParameters.Keys.ToArray();
            m_keys.ForEach(x =>
            {
                if (dynamicParametersStartValue.TryGetValue(x, out ParameterValue value))
                    dynamicParameters[x].SetValue(value);
                else 
                    dynamicParameters[x].SetValue(new ParameterValue(0.0f));
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

        public void FinishUse()
        {
            m_startedUse = false;
        }

        public void Use(SmartAgent agent)
        {
            if (!m_startedUse)
            {
                StartUse(agent.animationAgent);
                m_startedUse = true;
                return;
            }
            
            foreach (KeyValuePair<AgentDynamicParameter, ParameterValue> parameterEffect in data.parameterEffectOnAgent)
            {
                agent.UpdateParameterValue(parameterEffect.Key, parameterEffect.Value);
            }

            foreach (KeyValuePair<SmartObjectParameter, ParameterValue> parameterEffect in data.dynamicParametersEffect)
            {
                dynamicParameters[parameterEffect.Key].AddValue(parameterEffect.Value);
            }
        }

        public void DynamicParameterVariation()
        {
            foreach (SmartObjectParameter parameter in data.dynamicParametersVariation.Keys)
            {
                dynamicParameters[parameter].AddValue(data.dynamicParametersVariation[parameter]);
            }
        }
    }
}