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
        [SerializeField] private SmartObjectData data;

        [SerializeField] private SerializedDictionary<SmartObjectParameter, ParameterValue> dynamicParametersStartValue;
        [field : SerializeField, ReadOnly] public SerializedDictionary<SmartObjectParameter, ParameterValue> dynamicParameters { get; private set; } = new();

        private bool m_startedUse;

        private void Awake()
        {
            if (!usingPoint)
                usingPoint = transform;

            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(SmartObjectParameter)).Length; loop++)
            {
                SmartObjectParameter parameter = (SmartObjectParameter)loop;

                if (dynamicParametersStartValue.TryGetValue(parameter, out ParameterValue value))
                    dynamicParameters.Add(parameter, value);
                else
                    dynamicParameters.Add(parameter, new ParameterValue(0.0f));
            }
        }

        public void Reload()
        {
            SmartObjectParameter[] keys = dynamicParameters.Keys.ToArray();
            keys.ForEach(x =>
            {
                if (dynamicParametersStartValue.TryGetValue(x, out ParameterValue value))
                    dynamicParameters[x].SetValue(value);
                else 
                    dynamicParameters[x].SetValue(new ParameterValue(0.0f));
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(usingPoint.position, .2f);
        }

        public float CalculateScore(SmartAgent smartAgent)
        {
            
            return data.scoreCalculation.CalculateScore(smartAgent, this);
        }

        public void StartUse(AnimationAgent animationAgent)
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