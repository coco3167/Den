using System;
using AYellowpaper.SerializedCollections;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartObjectData", fileName = "New SmartObjectData")]
    public class SmartObjectData : ScriptableObject
    {
        [field : SerializeReference] public BaseScoreCalcul scoreCalculation { get; private set; }
        [field : SerializeField] public AnimatorOverrideController animatorController { get; private set; }
        
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, ParameterValue> dynamicParametersEffect { get; private set; }
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, ParameterValue> dynamicParametersVariation { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, ParameterValue> parameterEffectOnAgent { get; private set; }
    }
    public enum SmartObjectParameter
    {
        Usage,
    }
}