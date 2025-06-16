using System;
using System.Collections.Generic;
using Options;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace Sinj
{
    public abstract class SinjBehavior : ScriptableObject
    {
        //[SerializeField] private bool instantenous = false;
        [SerializeReference] private List<SinjStimulus> SinjStimuli;
        [SerializeReference] private List<SinjReaction> SinjReactions;
        [SerializeField] private AgentDynamicParameter parameter;
        
        public bool IsApplying(MouseAgent mouseAgent)
        {
            foreach (SinjStimulus sinjStimulus in SinjStimuli)
            {
                if (sinjStimulus is not SinjActiveBehavior.MousePositionStimulus)
                {
                    // If cursor is set to the same type of obj
                    if (parameter == GameParameters.CursorMode)
                        continue;

                    if (GameParameters.CursorMode != AgentDynamicParameter.Tension)
                    {
                        return false;
                    }
                }

                if(sinjStimulus.IsApplying(mouseAgent))
                    continue;
                return false;
            }
            
            return true;
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
