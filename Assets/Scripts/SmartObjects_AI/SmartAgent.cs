using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable]
    public class SmartAgent : MonoBehaviour
    {
        [SerializeField] private SmartAgentData data;

        private SmartObject[] m_smartObjects;
        
        private void Awake()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        private void Update()
        {
            SearchForSmartObject();
            // TODO do smthing with it
        }

        private SmartObject SearchForSmartObject()
        {
            Dictionary<SmartObject, float> smartObjectScore = new Dictionary<SmartObject, float>();
            Parallel.ForEach(m_smartObjects, o =>
            {
                smartObjectScore.Add(o, o.CalculateScore(this));
            });

            return smartObjectScore.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;
        }
    }
}