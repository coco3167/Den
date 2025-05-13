using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace SmartObjects_AI
{
    public class SmartObjectData : ScriptableObject
    {
        [field : SerializeField] public SerializedDictionary<ParameterType, float> dynamicParameters { get; private set; }
        [field : SerializeField] public Animator animator { get; private set; }
        [field : SerializeReference] public Func<float> scoreCalculationScore { get; private set; }

        private float TestScore()
        {
            return dynamicParameters[ParameterType.None] + WorldParameters.Parameters[WorldParameterType.None];
        }
    }
}