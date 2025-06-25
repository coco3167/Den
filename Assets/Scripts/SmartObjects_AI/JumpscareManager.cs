using System.Linq;
using UnityEngine;

namespace SmartObjects_AI
{
    public class JumpscareManager : MonoBehaviour
    {
        [SerializeField] private SmartObject[] jumpscares;
    
        public static float Value;

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
