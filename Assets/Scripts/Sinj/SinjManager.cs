using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace Sinj
{
    public class SinjManager : MonoBehaviour, IDebugDisplayAble, IReloadable
    {
        [Title("Sinjs")]
        [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefab")] private GameObject smartAgent;
        [SerializeField, Range(1,10)] private int sinjCount;
        [SerializeField, ReadOnly] private List<MouseAgent> mouseAgents = new();

        [Title("Emotions")]
        [SerializeField, ReadOnly] private SerializedDictionary<AgentDynamicParameter, float> emotionsJaugeValues = new();
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsMin;
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsMax;
        [SerializeField] private SerializedDictionary<AgentDynamicParameter, float> emotionsDecrease;
        [SerializeField, Tooltip("Used to know how many emotion/second is given to SinjManager")]
        private SerializedDictionary<AgentDynamicParameter, AnimationCurve> emotionsTransmissionCurve;
        
        [Title("Mouse Input")]
        [SerializeField] private MouseManager mouseManager;
    
        private List<DebugParameter> m_debugParameters = new();
        private float m_intensity;


        private void Awake()
        {
            for (int loop = 0; loop < sinjCount; loop++)
            {
                mouseAgents.Add(Instantiate(smartAgent, transform).GetComponent<MouseAgent>());
                mouseAgents[loop].Init(mouseManager);
                
                //Being instanced in runtime they arent picked by GameManager at Awake
                GameManager.Instance.GameReady += mouseAgents[loop].GetComponent<IGameStateListener>().OnGameReady;
            }
            
            emotionsJaugeValues.Add(AgentDynamicParameter.Tension, 0.0f);
            emotionsJaugeValues.Add(AgentDynamicParameter.Curiosity, 0.0f);
            emotionsJaugeValues.Add(AgentDynamicParameter.Aggression, 0.0f);
            emotionsJaugeValues.Add(AgentDynamicParameter.Fear, 0.0f);

            m_debugParameters.Add(new DebugParameter(AgentDynamicParameter.Tension.ToString(), "0"));
            m_debugParameters.Add(new DebugParameter(AgentDynamicParameter.Curiosity.ToString(), "0"));
            m_debugParameters.Add(new DebugParameter(AgentDynamicParameter.Aggression.ToString(), "0"));
            m_debugParameters.Add(new DebugParameter(AgentDynamicParameter.Fear.ToString(), "0"));
            
            GameManager.Instance.OnGameReady();
        }

        private void FixedUpdate()
        {
            foreach (MouseAgent agent in mouseAgents)
            {
                agent.SetDynamicParameterValue(AgentDynamicParameter.Tension, ClampEmotion(agent, AgentDynamicParameter.Tension));
                agent.SetDynamicParameterValue(AgentDynamicParameter.Curiosity, ClampEmotion(agent, AgentDynamicParameter.Curiosity));
                agent.SetDynamicParameterValue(AgentDynamicParameter.Aggression, ClampEmotion(agent, AgentDynamicParameter.Aggression));
                agent.SetDynamicParameterValue(AgentDynamicParameter.Fear, ClampEmotion(agent, AgentDynamicParameter.Fear));
            }
            //ClampIntensity();
            UpdateDebugValues();
        }

        public void Reload()
        {
            emotionsJaugeValues[AgentDynamicParameter.Tension] = 0;
            emotionsJaugeValues[AgentDynamicParameter.Curiosity] = 0;
            emotionsJaugeValues[AgentDynamicParameter.Aggression] = 0;
            emotionsJaugeValues[AgentDynamicParameter.Fear] = 0;
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
            emotionsJaugeValues[parameter] += curve.Evaluate(value)*Time.deltaTime;

            GameManager.Instance.HandlePallier(parameter, (int)emotionsJaugeValues[parameter]);
        }

        
        #region Debug
        private void UpdateDebugValues()
        {
            m_debugParameters[0].UpdateValue(((int)emotionsJaugeValues[AgentDynamicParameter.Tension]).ToString());
            m_debugParameters[1].UpdateValue(((int)emotionsJaugeValues[AgentDynamicParameter.Curiosity]).ToString());
            m_debugParameters[2].UpdateValue(((int)emotionsJaugeValues[AgentDynamicParameter.Aggression]).ToString());
            m_debugParameters[3].UpdateValue(((int)emotionsJaugeValues[AgentDynamicParameter.Fear]).ToString());
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
    }
}