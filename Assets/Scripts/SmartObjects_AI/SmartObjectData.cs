using System;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartObjectData", fileName = "New SmartObjectData")]
    public class SmartObjectData : ScriptableObject
    {
        [Title("Base Info")]
        [field : SerializeReference] public BaseScoreCalcul scoreCalculation { get; private set; }
        [field : SerializeField] public AnimatorOverrideController animatorController { get; private set; }
        [field : SerializeField] public AK.Wwise.Event wwiseEvent { get; private set; }
        [field : SerializeField] public int maxUser { get; private set; }
        [field: SerializeField] public float minRadius { get; private set; } = 1;
        
        [Title("Boolean")]
        [field: SerializeField] public bool adatpToMood { get; private set; } = false;
        [field: SerializeField] public bool shouldStopAgent { get; private set; } = false;
        [field: SerializeField] public bool shouldLookAtObject { get; private set; } = false;
        
        [Title("Dictionnaries")]
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersEffect { get; private set; }
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersVariation { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, float> parameterEffectOnAgent { get; private set; }

        public void Init()
        {
            scoreCalculation.Init();
        }
    }
    public enum SmartObjectParameter
    {
        Usage,
    }
}