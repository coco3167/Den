using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartAgentData", fileName = "New SmartAgentData")]
    public class SmartAgentData : ScriptableObject
    {
        [field : SerializeField] public SerializedDictionary<AgentStaticParameter, float> staticParameters { get; private set; }
    }
    
    public enum AgentStaticParameter
    {
        None
    }
        
    public enum AgentDynamicParameter
    {
        Hide
    }
}