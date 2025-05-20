using System;
using System.Collections.Generic;
using System.Linq;
using Sinj;
using UnityEngine;

namespace DebugHUD
{
    public class DebugDisplay : MonoBehaviour, IReloadable, IGameStateListener
    {
        [SerializeField] private GameObject parameterPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private DebugCategory category;
        
        private List<ParameterVisualisation> m_parameters = new();
        
        private int m_index = 0;
        private List<IDebugDisplayAble> m_displayAbles = new();
        private IDebugDisplayAble m_currentDisplayAble;

        private void Update()
        {
            if(m_currentDisplayAble == null)
                return;
            DisplayParameters();
        }

        public void Reload()
        {
            // Nothing There
        }
        
        public void OnGameReady(object sender, EventArgs eventArgs)
        {
            SetupDisplayAbles();
            if(m_displayAbles.Count == 0)
                return;
            SetupParameters();
        }

        public void OnGameEnded(object sender, EventArgs eventArgs)
        {
            // Nothing there
        }

        public void OnGamePaused(object sender, EventArgs eventArgs)
        {
            // Nothing there
        }

        private void SetupDisplayAbles()
        {
            m_displayAbles.Clear();
            MonoBehaviour[] displayAbles = {};
            switch (category)
            {
                case DebugCategory.SinjManager:
                    displayAbles = FindObjectsByType<SinjManager>(FindObjectsSortMode.None);
                    break;
                
                case DebugCategory.Sinj: 
                    displayAbles = FindObjectsByType<SinjAgent>(FindObjectsSortMode.None);
                    break;
            }
            
            foreach (MonoBehaviour sinjManager in displayAbles)
                m_displayAbles.Add(sinjManager.GetComponent<IDebugDisplayAble>());
            
        }
        private void SetupParameters()
        {
            Debug.Log(m_parameters.Count);
            Debug.Log(m_currentDisplayAble.GetParameterCount());
            m_currentDisplayAble = m_displayAbles[m_index];
            int parameterToInstantiate = m_currentDisplayAble.GetParameterCount() - m_parameters.Count;
            int parameterToRemove = m_parameters.Count - m_currentDisplayAble.GetParameterCount();
            
            for (int loop = 0; loop < parameterToInstantiate; loop++)
            {
                ParameterVisualisation parameter = Instantiate(parameterPrefab, content).GetComponent<ParameterVisualisation>();
                m_parameters.Add(parameter);
            }

            for (int loop = 0; loop < parameterToRemove; loop++)
            {
                ParameterVisualisation parameter = m_parameters.Last();
                m_parameters.Remove(parameter);
                Destroy(parameter.gameObject);
            }
            Debug.Log(m_parameters.Count);
        }

        private void DisplayParameters()
        {
            for (int loop = 0; loop < m_currentDisplayAble.GetParameterCount(); loop++)
            {
                DebugParameter parameter = m_currentDisplayAble.GetParameter(loop);
                ParameterVisualisation parameterVisualisation = m_parameters[loop];
                
                parameterVisualisation.Display(parameter);
            }
        }
    }

    public class DebugParameter
    {
        public string Name;
        public string Value;

        public DebugParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public void UpdateValue(string value)
        {
            Value = value;
        }
    }

    public interface IDebugDisplayAble
    {
        public int GetParameterCount();
        public DebugParameter GetParameter(int index);
    }

    enum DebugCategory
    {
        SinjManager,
        Sinj
    }
}
