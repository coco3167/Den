using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace SmartObjects_AI
{
    public class SmartAgentData : ScriptableObject
    {
        [field : SerializeField] public SerializedDictionary<ParameterType, float> staticParameters { get; private set; }
        [SerializeField] public SerializedDictionary<ParameterType, float> dynamicParameters;
    }
}