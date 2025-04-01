using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SinjBehavior", menuName = "SinjBehavior")]
public class SinjBehavior : ScriptableObject
{
    [field : SerializeReference] public List<SinjStimulus> SinjStimuli { get; private set; }
    [field : SerializeReference] public List<SinjReaction> SinjReactions { get; private set; }

    // Sinj Simuli
    [Serializable]
    public abstract class SinjStimulus { }

    [Serializable]
    public class MousePositionStimulus : SinjStimulus
    {
        [field : SerializeField] public float Distance { get; private set; }
    }

    // Sinj Reaction
    [Serializable] public abstract class SinjReaction { }

    [Serializable]
    public class FleeReaction : SinjReaction
    {
        [field : SerializeField] public float Distance { get; private set; }
    }
}