using System;
using UnityEngine;

namespace Sinj
{
    [CreateAssetMenu(fileName = "SinjBehavior", menuName = "SinjBehavior/PassiveBehavior", order = 1)]
    public class SinjPassivBehavior : SinjBehavior
    {
        #region Reaction
        [Serializable]
        public class RestReaction : SinjReaction
        {
            [SerializeField, Range(.5f,5)] private float minTime = .5f, maxTime = 5f; 
            public override void ApplyReaction(SinjAgent agent)
            {
                Debug.Log(minTime);
                agent.Rest(minTime, maxTime);
            }
        }
        
        [Serializable]
        public class WalkReaction : SinjReaction
        {
            [SerializeField] private float distance;
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.Walk(distance);
            }
        }
        #endregion
    }
}