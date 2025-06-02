using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartAgentData", fileName = "New SmartAgentData")]
    public class SmartAgentData : ScriptableObject
    {
        [field : SerializeField] public SerializedDictionary<AgentStaticParameter, float> staticParameters { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, float> dynamicParametersVariation { get; private set; }
    }
    
    public enum AgentStaticParameter
    {
        None
    }

    public enum AgentDynamicParameter
    {
        Tension,
        Curiosity,
        Aggression,
        Fear,
        Tiredness,
        Suspicion,
        Hunger,
        Social,
        Neutral
    }

    /*[Serializable]
    public class ParameterValue
    {
        public enum ParameterValueType
        {
            Float,
            Bool,
            None
        }

        public ParameterValueType valueType;
        [SerializeField] private float floatValue;
        [SerializeField] private bool boolValue;

        public ParameterValue()
        {
            valueType = ParameterValueType.None;
        }
        
        public ParameterValue(bool value)
        {
            valueType = ParameterValueType.Bool;
            boolValue = value;
        }

        public ParameterValue(float value)
        {
            valueType = ParameterValueType.Float;
            floatValue = value;
            ClampValue();
        }

        public bool GetBoolValue()
        {
            return boolValue;
        }

        public float GetFloatValue()
        {
            return floatValue;
        }

        public void SetValue(ParameterValue other)
        {
            TryToGetValueType(other);
            
            floatValue = other.floatValue; 
            ClampValue();            
            
            boolValue = other.boolValue;
        }

        public void AddValue(ParameterValue other)
        {
            TryToGetValueType(other);
            
            floatValue += other.floatValue;
            ClampValue();
            
            boolValue = other.boolValue;
        }

        private void TryToGetValueType(ParameterValue other)
        {
            if (valueType == ParameterValueType.None && other.valueType != ParameterValueType.None)
                valueType = other.valueType;
        }

        private void ClampValue()
        {
            floatValue = Math.Clamp(floatValue, 0.0f, 100.0f);
        }
    }*/
}