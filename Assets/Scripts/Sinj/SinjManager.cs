using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sinj
{
    public class SinjManager : MonoBehaviour, IDebugDisplayAble
    {
        [Header("Sinjs")]
        [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefab")] private GameObject sinjPrefab;
        [SerializeField, Range(1,10)] private int sinjCount;
        [SerializeField, ReadOnly] private List<SinjAgent> sinjs = new();

        [Header("Emotions")]
        [SerializeField, ReadOnly] private SerializedDictionary<Emotions, float> emotionsJaugeValues = new();
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsMin;
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsMax;
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsDecrease;
        [SerializeField, Tooltip("Used to know how many emotion/second is given to SinjManager")]
        private SerializedDictionary<Emotions, AnimationCurve> emotionsTransmissionCurve;

        [Header("Navigation")]
        [SerializeField] private EnvironmentManager environmentManager;
    
        [Header("Mouse Input")]
        [SerializeField] private MouseManager mouseManager;
    
        private readonly List<DebugParameter> m_debugParameters = new();
        private float m_intensity;

        private void Awake()
        {
            sinjs.Clear();
            for (int loop = 0; loop < sinjCount; loop++)
            {
                sinjs.Add(Instantiate(sinjPrefab, transform).GetComponent<SinjAgent>());
                sinjs[loop].Init(mouseManager);
            }
            
            emotionsJaugeValues.Clear();
            int emotionsCount = Enum.GetNames(typeof(Emotions)).Length;
            for (int loop = 0; loop < emotionsCount; loop++)
            {
                Emotions emotion = (Emotions)loop;
                emotionsJaugeValues.Add(emotion, 0);
                m_debugParameters.Add(new DebugParameter(emotion.ToString(), "0"));
            }
            GameManager.OnGameReady();
        }

        private void FixedUpdate()
        {
            foreach (SinjAgent sinj in sinjs)
            {
                sinj.HandleBehaviors();
                sinj.UpdateEmotion(ClampEmotion(sinj, Emotions.Tension), Emotions.Tension);
                sinj.UpdateEmotion(ClampEmotion(sinj, Emotions.Curiosity), Emotions.Curiosity);
                sinj.UpdateEmotion(ClampEmotion(sinj, Emotions.Agression), Emotions.Agression);
                sinj.UpdateEmotion(ClampEmotion(sinj, Emotions.Fear),Emotions.Fear);
            }
            ClampIntensity();
            UpdateDebugValues();
        }

        private float ClampEmotion(SinjAgent agent, Emotions emotion)
        {
            float value = agent.GetEmotion(emotion);
            float emotionDecrease = emotionsDecrease[emotion];
            float emotionMax = emotionsMax[emotion];
            float emotionMin = emotionsMin[emotion];
            
            value -= Time.deltaTime * emotionDecrease;
            value = Mathf.Clamp(value, emotionMin, emotionMax);
            
            UpdateManagerEmotion(emotion, value);
            
            return value;
        }

        private void ClampIntensity()
        {
            float emotionMin = emotionsMin[Emotions.Intensity];
            float emotionMax = emotionsMax[Emotions.Intensity];
            m_intensity = Mathf.Clamp(m_intensity, emotionMin, emotionMax);
            m_debugParameters[4].Value = m_intensity.ToString();
            
            UpdateManagerEmotion(Emotions.Intensity, m_intensity);
        }

        private void UpdateManagerEmotion(Emotions emotion, float value)
        {
            if(!emotionsTransmissionCurve.TryGetValue(emotion, out AnimationCurve curve))
                return;

            value /= 100;
            emotionsJaugeValues[emotion] += curve.Evaluate(value)*Time.deltaTime;
        }
        #region Debug
        private void UpdateDebugValues()
        {
            for (int loop = 0; loop < m_debugParameters.Count; loop++)
            {
                Emotions emotion = (Emotions)loop;
                m_debugParameters[loop].UpdateValue(((int)emotionsJaugeValues[emotion]).ToString());
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
    }
}