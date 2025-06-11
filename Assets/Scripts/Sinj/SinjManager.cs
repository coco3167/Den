using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SmartObjects_AI;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace Sinj
{
    public class SinjManager : MonoBehaviour, IDebugDisplayAble, IReloadable
    {
        [Title("Sinjs")]
        [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefab")] private GameObject smartAgent;
        [SerializeField, Range(0,10)] private int sinjCount;
        [SerializeField, ReadOnly] public List<MouseAgent> mouseAgents = new();

        [Title("Emotions")]
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsMin;
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsMax;
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsDecrease;
        [SerializeField, Tooltip("Used to know how many emotion/second is given to SinjManager")]
        private SerializedDictionary<AgentDynamicParameter, AnimationCurve> emotionsTransmissionCurve;
        
        [Title("Mouse Input")]
        [SerializeField] private MouseManager mouseManager;
    
        private List<DebugParameter> m_debugParameters = new();
        private float m_intensity;
        private WorldParameters m_worldParameters;


        private void Awake()
        {
            m_worldParameters = GameManager.Instance.worldParameters;
            
            for (int loop = 0; loop < sinjCount; loop++)
            {
                mouseAgents.Add(Instantiate(smartAgent, transform).GetComponent<MouseAgent>());
                mouseAgents[loop].Init(mouseManager);
                
                //Being instanced in runtime they arent picked by GameManager at Awake
                GameLoopManager.Instance.GameReady += mouseAgents[loop].GetComponent<IGameStateListener>().OnGameReady;
            }

            m_worldParameters.AgentGlobalParameters.ForEach(x => m_debugParameters.Add(new DebugParameter(x.Key.ToString(), "0")));
            
            //GameManager.Instance.OnGameReady();
        }

        private void FixedUpdate()
        {
            if(GameManager.Instance.IsPaused)
                return;
            
            foreach (MouseAgent agent in mouseAgents)
            {
                AgentDynamicParameter[] keys = m_worldParameters.AgentGlobalParameters.Keys.ToArray();
                keys.ForEach(x => agent.SetDynamicParameterValue(x, ClampEmotion(agent, x)));
            }
            //ClampIntensity();
            UpdateDebugValues();
        }

        private float ClampEmotion(MouseAgent agent, AgentDynamicParameter parameter)
        {
            float value = agent.GetDynamicParameterValue(parameter);
            float emotionDecrease = emotionsDecrease[parameter];
            float emotionMax = emotionsMax[parameter];
            float emotionMin = emotionsMin[parameter];
            
            value -= Time.deltaTime * emotionDecrease;
            value = Mathf.Clamp(value, emotionMin, emotionMax);
            
            UpdateManagerEmotion(parameter, value);
            
            return value;
        }

        // private void ClampIntensity()
        // {
        //     float emotionMin = emotionsMin[Emotions.Intensity];
        //     float emotionMax = emotionsMax[Emotions.Intensity];
        //     m_intensity = Mathf.Clamp(m_intensity, emotionMin, emotionMax);
        //     m_debugParameters[4].Value = m_intensity.ToString();
        //     
        //     UpdateManagerEmotion(Emotions.Intensity, m_intensity);
        // }

        private void UpdateManagerEmotion(AgentDynamicParameter parameter, float value)
        {
            if(!emotionsTransmissionCurve.TryGetValue(parameter, out AnimationCurve curve))
                return;

            value /= 100;
            m_worldParameters.AgentGlobalParameters[parameter] += curve.Evaluate(value)*Time.deltaTime;

            GameManager.Instance.HandlePallier(parameter, (int)m_worldParameters.AgentGlobalParameters[parameter]);
        }

        
        #region Debug
        private void UpdateDebugValues()
        {
            int loop = 0;
            foreach (float value in m_worldParameters.AgentGlobalParameters.Values)
            {
                m_debugParameters[loop].UpdateValue(value.ToString("0"));
                loop++;
            }
        }
        public int GetParameterCount()
        {
            return m_debugParameters.Count;
        }

        public DebugParameter GetParameter(int index)
        {
            return m_debugParameters[index];
        }
        #endregion

        public void Reload()
        {
            AgentDynamicParameter[] keys = m_worldParameters.AgentGlobalParameters.Keys.ToArray();
            foreach (AgentDynamicParameter parameter in keys)
            {
                m_worldParameters.AgentGlobalParameters[parameter] = 0;
            }
        }
    }
}