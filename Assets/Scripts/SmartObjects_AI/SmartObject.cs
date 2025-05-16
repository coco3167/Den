using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SmartObjects_AI
{
    public class SmartObject : MonoBehaviour
    {
        [field: SerializeField, ChildGameObjectsOnly]
        public Transform usingPoint { get; private set; }

        [SerializeField] private SmartObjectData data;

        public Dictionary<SmartObjectParameter, float> dynamicParameters { get; private set; } = new();


        private void Awake()
        {
            if (!usingPoint)
                usingPoint = transform;

            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(SmartObjectParameter)).Length; loop++)
            {
                dynamicParameters.Add((SmartObjectParameter)loop, 0);
            }
        }

        public float CalculateScore(SmartAgent smartAgent)
        {
            return data.scoreCalculation.CalculateScore(smartAgent, this);
        }

        public void StartUse(AnimationAgent animationAgent)
        {
            animationAgent.SwitchAnimator(data.animatorController);
        }

        public void Use(SmartAgent agent)
        {
            foreach (KeyValuePair<AgentDynamicParameter, float> parameterEffect in data.parameterEffectOnAgent)
            {
                agent.UpdateParameterValue(parameterEffect.Key, parameterEffect.Value);
            }

            foreach (KeyValuePair<SmartObjectParameter, float> parameterEffect in data.dynamicParametersEffect)
            {
                dynamicParameters[parameterEffect.Key] += parameterEffect.Value;
            }
        }
    }
}