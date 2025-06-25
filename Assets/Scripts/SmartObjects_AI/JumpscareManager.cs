using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace SmartObjects_AI
{
    public class JumpscareManager : MonoBehaviour
    {
        [SerializeField] private SmartObject[] jumpscares;
    
        [NonSerialized] public float Value;

        private void Awake()
        {
            jumpscares.ForEach(x => x.JumpscareManager = this);
        }

        private void Update()
        {
            Value = jumpscares
                .Aggregate(
                    (x, y) =>
                        x.GetDynamicParameter(SmartObjectParameter.Usage) <
                        y.GetDynamicParameter(SmartObjectParameter.Usage) 
                            ? x : y)
                .GetDynamicParameter(SmartObjectParameter.Usage);
        }
    }
}