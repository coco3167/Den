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
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsMin;
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsMax;
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsDecrease;

        [Header("Navigation")]
        [SerializeField] private EnvironmentManager environmentManager;
    
        [Header("Mouse Input")]
        [SerializeField] private MouseManager mouseManager;
    
        private readonly List<DebugParameter> m_debugParameters = new();

        private void Awake()
        {
            sinjs.Clear();
            for (int loop = 0; loop < sinjCount; loop++)
            {
                sinjs.Add(Instantiate(sinjPrefab, transform).GetComponent<SinjAgent>());
                sinjs[loop].Init(mouseManager);
            }
        
            m_debugParameters.Add(new DebugParameter("Health", "0"));
        
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
        }

        private float ClampEmotion(SinjAgent agent, Emotions emotion)
        {
            float value = agent.GetEmotion(emotion);
            float emotionDecrease = emotionsDecrease[emotion];
            float emotionMax = emotionsMax[emotion];
            float emotionMin = emotionsMin[emotion];
            
            value -= Time.deltaTime * emotionDecrease;
            value = Mathf.Clamp(value, emotionMin, emotionMax);
            return value;
        }

        #region Debug
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