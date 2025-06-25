using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SmartObjects_AI
{
    public class JumpscareManager : MonoBehaviour
    {
        [SerializeField] private SmartObject[] jumpscares;
    
        [NonSerialized] public float Value;

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
