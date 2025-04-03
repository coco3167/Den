using System.Collections.Generic;
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
        
        [Header("Tension")]
        [SerializeField] private float tensionMax = 100;
        [SerializeField] private float tensionDecrease = 10;

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

        private void Update()
        {
            foreach (SinjAgent sinj in sinjs)
            {
                sinj.HandleBehaviors();
                ClampTension(sinj);
            }
        }

        private void ClampTension(SinjAgent sinj)
        {
            float tension = sinj.GetTension();
            tension -= Time.deltaTime * tensionDecrease;
            tension = Mathf.Clamp(tension, 0, tensionMax);
            sinj.UpdateTension(tension);
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