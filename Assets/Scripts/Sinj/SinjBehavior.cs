using System;
using System.Collections.Generic;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace Sinj
{
    public abstract class SinjBehavior : ScriptableObject
    {
        //[SerializeField] private bool instantenous = false;
        [SerializeReference] private List<SinjStimulus> SinjStimuli;
        [SerializeReference] private List<SinjReaction> SinjReactions;
        
        public bool IsApplying(MouseAgent mouseAgent)
        {
            bool result = true;
            foreach (SinjStimulus sinjStimulus in SinjStimuli)
            {
                if(sinjStimulus.IsApplying(mouseAgent))
                    continue;
                result = false;
                break;
            }
            
            return result;
        }

        public void ApplyReaction(MouseAgent mouseAgent)
        {
            // if(!instantenous)
            //     agent.ChangeBehavior(this);
            
            foreach (SinjReaction sinjReaction in SinjReactions)
            {
                sinjReaction.ApplyReaction(mouseAgent);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /*public bool IsFinished(SinjAgent agent)
        {
            if (instantenous)
                return true;
            
            bool result = true;
            foreach (SinjReaction sinjReaction in SinjReactions)
            {
                if(sinjReaction.IsFinished(agent))
                    continue;
                result = false;
                break;
            }
            
            return result;
        }*/

        public override string ToString()
        {
            return name;
        }

        #region SubClassDefinition
        [Serializable]
        public abstract class SinjStimulus
        {
            public abstract bool IsApplying(MouseAgent mouseAgent);
        }
        
        [Serializable]
        public abstract class SinjReaction
        {
            public abstract void ApplyReaction(MouseAgent mouseAgent);
            
            //public abstract bool IsFinished(SinjAgent agent);
        }
        #endregion
    }
}
