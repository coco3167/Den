using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable, RequireComponent(typeof(MovementAgent), typeof(AnimationAgent))]
    public class SmartAgent : MonoBehaviour
    {
        private const float AIUpdateSleepTime = 0.1f;
        
        [SerializeField] private SmartAgentData data;

        public Dictionary<AgentDynamicParameter, float> dynamicParameters { get; private set; } = new();


        private SmartObject[] m_smartObjects;
        private SmartObject m_previousSmartObject;
        
        private MovementAgent m_movementAgent;
        private AnimationAgent m_animationAgent;
        
        private void Awake()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            m_movementAgent = GetComponent<MovementAgent>();
            m_animationAgent = GetComponent<AnimationAgent>();

            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(AgentDynamicParameter)).Length; loop++)
            {
                dynamicParameters.Add((AgentDynamicParameter)loop, 0);
            }

            StartCoroutine(AIUpdate());
        }

        private IEnumerator AIUpdate()
        {
            while (true)
            {
                SmartObject objToUse = SearchForSmartObject();

                m_movementAgent.SetDestination(objToUse.usingPoint);

                if (m_movementAgent.IsCloseToDestination())
                {
                    if (m_previousSmartObject != objToUse)
                    {
                        m_previousSmartObject = objToUse;
                        objToUse.StartUse(m_animationAgent);
                    }

                    objToUse.Use(this);
                }
                
                yield return new WaitForSeconds(AIUpdateSleepTime);
            }
        }

        private SmartObject SearchForSmartObject()
        {
            Dictionary<SmartObject, float> smartObjectScore = new Dictionary<SmartObject, float>();
            foreach (SmartObject smartObject in m_smartObjects)
            {
                smartObjectScore.Add(smartObject, smartObject.CalculateScore(this));
            }
            
            return smartObjectScore.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;
        }

        public void UpdateParameterValue(AgentDynamicParameter parameter, float value)
        {
            dynamicParameters[parameter] += value;
            //Debug.Log(value);
        }
    }
}