using System;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SmartObjects_AI
{
    [Serializable, CreateAssetMenu(menuName = "SmartObject/New SmartObjectData", fileName = "New SmartObjectData")]
    public class SmartObjectData : ScriptableObject
    {
        [Title("Base Info")]
        [field : SerializeReference] public BaseScoreCalcul scoreCalculation { get; private set; }

        [SerializeField] private AnimatorOverrideController[] animatorControllers;
        [field : SerializeField] public AK.Wwise.Event wwiseEvent { get; private set; }
        [field : SerializeField] public int maxUser { get; private set; }
        [field: SerializeField] public float minRadius { get; private set; } = 1;
        [field: SerializeField] public float stoppingDistance { get; private set; } = 1;
        
        [Title("Boolean")]
        [field: SerializeField] public bool shouldRunTo { get; private set; } = false;
        [field: SerializeField] public bool shouldStopAgent { get; private set; } = false;
        [field: SerializeField] public bool shouldLookAtObject { get; private set; } = false;
        [field: SerializeField, EnableIf("shouldLookAtObject")] public DefaultLookingPoint defaultLookingPoint { get; private set; }
        [field: SerializeField] public bool shouldEndFast { get; private set; } = false;
        [field: SerializeField] public bool shouldSkipStart { get; private set; } = false;
        [field: SerializeField] public bool shouldSkipEnd { get; private set; } = false;
        [field: SerializeField] public bool shouldInterruptNext { get; private set; } = false;
        [field: SerializeField] public bool inInterruptable { get; private set; } = false;
        
        [Title("Dictionnaries")]
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersEffect { get; private set; }
        [field : SerializeField] public SerializedDictionary<SmartObjectParameter, float> dynamicParametersVariation { get; private set; }
        [field : SerializeField] public SerializedDictionary<AgentDynamicParameter, float> parameterEffectOnAgent { get; private set; }

        public void Init()
        {
            scoreCalculation.Init();
        }

        public bool IsRest()
        {
            return scoreCalculation.GetType() == typeof(RestScore);
        }
        
        public AnimatorOverrideController GetAnimator()
        {
            int rand = Random.Range(0, animatorControllers.Length);
            Debug.Log(animatorControllers[rand]);
            return animatorControllers[rand];
        }

        public enum DefaultLookingPoint
        {
            None,
            Mouse
        }
    }
    public enum SmartObjectParameter
    {
        Usage,
        Dirtiness,
        Fear,
    }
}