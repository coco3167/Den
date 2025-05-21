using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
namespace SmartObjects_AI.Agent
{
    [Serializable, RequireComponent(typeof(MovementAgent), typeof(AnimationAgent))]
    public class SmartAgent : MonoBehaviour, IGameStateListener, IReloadable
    {
        private const float AIUpdateSleepTime = 0.1f;
        
        [SerializeField] private SmartAgentData data;

        [field : SerializeField, ReadOnly] public SerializedDictionary<AgentDynamicParameter, float> dynamicParameters { get; private set; } = new();


        private SmartObject[] m_smartObjects;
        private SmartObject m_previousSmartObject;
        
        private MovementAgent m_movementAgent;
        public AnimationAgent animationAgent { get; private set; }
        
        private void Awake()
        {
            m_smartObjects = FindObjectsByType<SmartObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            m_movementAgent = GetComponent<MovementAgent>();
            animationAgent = GetComponent<AnimationAgent>();
            
            //Dynamic values to default state
            for (int loop = 0; loop < Enum.GetNames(typeof(AgentDynamicParameter)).Length; loop++)
            {
                dynamicParameters.Add((AgentDynamicParameter)loop, 0);
            }

        }
        
        public void OnGameReady(object sender, EventArgs eventArgs)
        {
            StartCoroutine(AIUpdate());
            Debug.Log("start coroutine");
        }

        public void OnGameEnded(object sender, EventArgs eventArgs)
        {
            StopCoroutine(AIUpdate());
            Debug.Log("stop coroutine");
        }

        public void Reload()
        {
            transform.localPosition = Vector3.zero;
            
            AgentDynamicParameter[] keys = dynamicParameters.Keys.ToArray();
            keys.ForEach(x => dynamicParameters[x] = 0);
        }

        private IEnumerator AIUpdate()
        {
            yield return new WaitForEndOfFrame();
            m_previousSmartObject = SearchForSmartObject();
            while (true)
            {
                DynamicParameterVariation();
                
                TryToUseSmartObject();
                
                yield return new WaitForSeconds(AIUpdateSleepTime);
            }
        }

        private void DynamicParameterVariation()
        {
            foreach (AgentDynamicParameter parameter in data.dynamicParametersVariation.Keys)
            {
                dynamicParameters[parameter] += data.dynamicParametersVariation[parameter];
            }
        }

        private void TryToUseSmartObject()
        {
            SmartObject objToUse = SearchForSmartObject();

            m_movementAgent.SetDestination(objToUse.usingPoint);
                
            if (m_previousSmartObject != objToUse)
            {
                m_previousSmartObject.FinishUse();
                m_previousSmartObject = objToUse;
                animationAgent.FinishUseAnimation();
            }

            if (m_movementAgent.IsCloseToDestination())
            {
                objToUse.Use(this);
            }
        }

        private SmartObject SearchForSmartObject()
        {
            Dictionary<SmartObject, float> smartObjectScore = new Dictionary<SmartObject, float>();
            foreach (SmartObject smartObject in m_smartObjects)
            {
                smartObjectScore.Add(smartObject, smartObject.CalculateScore(this));
                smartObject.DynamicParameterVariation();
            }
            
            return smartObjectScore.Aggregate((a,b) => a.Value > b.Value ? a : b).Key;
        }

        public void UpdateParameterValue(AgentDynamicParameter parameter, float value)
        {
            dynamicParameters[parameter] += value;
        }
    }
}