using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartObjectData", fileName = "New SmartObjectData")]
    public class SmartObjectData : ScriptableObject
    {
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersEffect { get; private set; }
        [field : SerializeReference] public BaseScoreCalcul scoreCalculation { get; private set; }

        [field : SerializeField] public RuntimeAnimatorController animatorController { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, float> parameterEffectOnAgent { get; private set; }
    }
    public enum SmartObjectParameter
    {
        None,
    }
}