using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugHUD
{
    public class DebugDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject parameterPrefab;
        [SerializeField] private GameObject parentDisplayable;
        
        private List<ParameterVisualisation> parameters = new();
        
        private int m_index = 0;
        private List<IDebugDisplayAble> m_displayAbles = new();
        private IDebugDisplayAble m_currentDisplayAble;

        private void Awake()
        {
            
            SetupParameters();
        }

        private void Update()
        {
            DisplayParameters();
        }
        
        private void SetupParameters()
        {
            m_currentDisplayAble = m_displayAbles[m_index];
            for (int loop = 0; loop < m_currentDisplayAble.GetParameterCount() - parameters.Count; loop++)
            {
                ParameterVisualisation parameter = Instantiate(parameterPrefab, transform).GetComponent<ParameterVisualisation>();
                parameters.Add(parameter);
            }

            for (int loop = 0; loop < parameters.Count - m_currentDisplayAble.GetParameterCount(); loop++)
            {
                ParameterVisualisation parameter = parameters.Last();
                parameters.Remove(parameter);
                Destroy(parameter.gameObject);
            }
        }

        private void DisplayParameters()
        {
            for (int loop = 0; loop < m_currentDisplayAble.GetParameterCount(); loop++)
            {
                DebugParameter parameter = m_currentDisplayAble.GetParameter(loop);
                ParameterVisualisation parameterVisualisation = parameters[loop];
                
                parameterVisualisation.Display(parameter);
            }
        }
    }

    public struct DebugParameter
    {
        public string Name;
        public float Value;
    }

    public interface IDebugDisplayAble
    {
        public int GetParameterCount();
        public DebugParameter GetParameter(int index);
    }
}
