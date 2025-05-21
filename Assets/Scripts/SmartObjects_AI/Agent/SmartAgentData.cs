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
        Hide
    }
}