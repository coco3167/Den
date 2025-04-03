using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Sinj
{
    [MovedFrom(false, null, null, "SinjBehavior")]
    [CreateAssetMenu(fileName = "SinjBehavior", menuName = "SinjBehavior")]
    public class SinjBehavior : ScriptableObject
    {
        [field : SerializeReference] public List<SinjStimulus> SinjStimuli { get; private set; }
        [field : SerializeReference] public List<SinjReaction> SinjReactions { get; private set; }

        // Sinj Simuli
        [Serializable]
        public abstract class SinjStimulus
        {
            public abstract bool IsApplying(Sinj.SinjAgent agent);
        }

        [Serializable]
        public class MousePositionStimulus : SinjStimulus
        {
            [field: SerializeField] private float distance;

            public override bool IsApplying(SinjAgent agent)
            {
                return agent.DistanceToMouse() < Mathf.Pow(distance, 2);
            }
        }

        [Serializable]
        public class MouseVelocityStimulus : SinjStimulus
        {
            [field: SerializeField] private float velocity;
            
            public override bool IsApplying(SinjAgent agent)
            {
                return agent.MouseVelocity() >= velocity;
            }
        }
        
        // Sinj Reaction
        [Serializable]
        public abstract class SinjReaction
        {
            public abstract void ApplyReaction(SinjAgent agent);
        }

        [Serializable]
        public class FleeReaction : SinjReaction
        {
            [field: SerializeField] private float distance;
            
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.FleeReaction(distance);
            }
        }
        
        [Serializable]
        public class TensionReaction : SinjReaction
        {
            [field: SerializeField] private float amount;
            
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.AddTension(amount);
            }
        }
    }
}