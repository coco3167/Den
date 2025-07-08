using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace SmartObjects_AI
{
    public class JumpscareManager : MonoBehaviour
    {
        [SerializeField] private SmartObject[] jumpscares;
        
        private void Awake()
        {
            jumpscares.ForEach(x => x.JumpscareManager = this);
        }

        private void Update()
        {
            // Value = jumpscares
            //     .Aggregate(
            //         (x, y) =>
            //             x.GetDynamicParameter(SmartObjectParameter.Usage) >
            //             y.GetDynamicParameter(SmartObjectParameter.Usage) 
            //                 ? x : y)
            //     .GetDynamicParameter(SmartObjectParameter.Usage);

            float usage = 100;
            //int loop = 0;
            foreach (SmartObject smartObject in jumpscares)
            {
                float objectUsage = smartObject.GetDynamicParameter(SmartObjectParameter.Usage);
                if (objectUsage < usage)
                {
                    usage = objectUsage;
                }
                //Debug.Log($"{objectUsage} | {loop}");
                //loop++;
            }

            jumpscares.ForEach(x => x.SetDynamicParameter(SmartObjectParameter.Usage, usage));
        }
    }
}