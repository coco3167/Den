using System;
using UnityEngine;

namespace Sinj
{
    [CreateAssetMenu(fileName = "SinjActiveBehavior", menuName = "SinjBehavior/ActiveBehavior", order = 0)]
    public class SinjActiveBehavior : SinjBehavior
    {
        private enum Comparison
        {
            Inferior,
            Superior,
            Equal
        }

        #region Stimuli
        [Serializable]
        public class MousePositionStimulus : SinjStimulus
        {
            [SerializeField] private float distance;
            [SerializeField] private Comparison comparison;

            public override bool IsApplying(SinjAgent agent)
            {
                float distanceToMouse = agent.DistanceToMouse();
                float realDistance = Mathf.Pow(distance, 2);
                switch (comparison)
                {
                    case Comparison.Inferior:
                        return distanceToMouse < realDistance;
                    case Comparison.Superior:
                        return distanceToMouse > realDistance;
                    case Comparison.Equal:
                        return Mathf.Abs(distanceToMouse-realDistance) <= Mathf.Epsilon;
                }
                return false;
            }
        }

        [Serializable]
        public class MouseVelocityStimulus : SinjStimulus
        {
            [SerializeField] private float velocity;
            [SerializeField] private Comparison comparison;
            
            public override bool IsApplying(SinjAgent agent)
            {
                float mouseVelocity = agent.MouseVelocity();
                switch (comparison)
                {
                    case Comparison.Inferior:
                        return mouseVelocity < velocity;
                    case Comparison.Superior:
                        return mouseVelocity > velocity;
                    case Comparison.Equal:
                        return Mathf.Abs(mouseVelocity-velocity) <= Mathf.Epsilon;
                }
                return false;
            }
        }

        [Serializable]
        public class TensionAmount : SinjStimulus
        {
            [SerializeField] private float tension;
            [SerializeField] private Comparison comparison;

            public override bool IsApplying(SinjAgent agent)
            {
                float agentTension = agent.GetEmotion(Emotions.Tension);
                switch (comparison)
                {
                    case Comparison.Inferior:
                        return agentTension < tension;
                    case Comparison.Superior:
                        return agentTension > tension;
                    case Comparison.Equal:
                        return Mathf.Abs(agentTension - tension) <= Mathf.Epsilon;
                }
                return false;
            }
        }
        #endregion

        #region Reaction
        [Serializable]
        public class FleeReaction : SinjReaction
        {
            [field: SerializeField] private float distance;
            
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.FleeReaction(distance);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return agent.IsCloseToDestination();
            }
        }
        
        [Serializable]
        public class TensionReaction : SinjReaction
        {
            [field: SerializeField] private float amount;
            
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.AddEmotion(amount, Emotions.Tension);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return true;
            }
        }

        [Serializable]
        public class CuriosityReaction : SinjReaction
        {
            [SerializeField] private float amount;

            public override void ApplyReaction(SinjAgent agent)
            {
                agent.AddEmotion(amount, Emotions.Curiosity);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return true;
            }
        }
        
        [Serializable]
        public class AgressionReaction : SinjReaction
        {
            [SerializeField] private float amount;

            public override void ApplyReaction(SinjAgent agent)
            {
                agent.AddEmotion(amount, Emotions.Agression);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return true;
            }
        }
        
        [Serializable]
        public class FearReaction : SinjReaction
        {
            [SerializeField] private float amount;

            public override void ApplyReaction(SinjAgent agent)
            {
                agent.AddEmotion(amount, Emotions.Fear);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return true;
            }
        }

        public class BarkReaction : SinjReaction
        {
            [SerializeField] private AK.Wwise.Event barkEvent;

            public override void ApplyReaction(SinjAgent agent)
            {
                agent.PlaySound(barkEvent);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return true;
            }
        }
        #endregion
    }
}