using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sinj
{
    public abstract class SinjBehavior : ScriptableObject
    {
        [Serializable]
        public abstract class SinjStimulus
        {
            public abstract bool IsApplying(SinjAgent agent);
        }
        
        [Serializable]
        public abstract class SinjReaction
        {
            public abstract void ApplyReaction(SinjAgent agent);
        }

        [SerializeReference] private List<SinjStimulus> SinjStimuli;
        [SerializeReference] private List<SinjReaction> SinjReactions;
        
        public bool IsApplying(SinjAgent agent)
        {
            bool result = true;
            foreach (SinjStimulus sinjStimulus in SinjStimuli)
            {
                if(sinjStimulus.IsApplying(agent))
                    continue;
                result = false;
                break;
            }
            
            return result;
        }

        public void ApplyReaction(SinjAgent agent)
        {
            foreach (SinjReaction sinjReaction in SinjReactions)
            {
                sinjReaction.ApplyReaction(agent);
            }
        }
    }
}
