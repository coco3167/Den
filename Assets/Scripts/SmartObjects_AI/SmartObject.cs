using UnityEngine;

namespace SmartObjects_AI
{
    public class SmartObject : MonoBehaviour
    {
        [SerializeField] private Transform usingPoint;
        [SerializeField] private SmartObjectData data;
        

        private void Awake()
        {
            if (!usingPoint)
                usingPoint = transform;
        }

        public float CalculateScore(SmartAgent agent)
        { 
            return data.scoreCalculationScore.Invoke();
        }
    }
    
    public enum ParameterType
    {
        None
    }
}