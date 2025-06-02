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
        [field : SerializeField] public int maxUser { get; private set; }
        
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersEffect { get; private set; }
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersVariation { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, float> parameterEffectOnAgent { get; private set; }
    }
    public enum SmartObjectParameter
    {
        Usage,
    }
}