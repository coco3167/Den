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
            [SerializeField] private string description;
            
            // ReSharper disable Unity.PerformanceAnalysis
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.Rest();
            }
        }
        #endregion
    }
}