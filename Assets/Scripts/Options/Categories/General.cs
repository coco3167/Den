using SmartObjects_AI.Agent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Options.Categories
{
    public class General : MonoBehaviour
    {
        [SerializeField] private Toggle godMode;
        [SerializeField] private TMP_Dropdown cursorMode;

        private AgentDynamicParameter m_cursorModeParameter = AgentDynamicParameter.Tension;

        private void Awake()
        {
            cursorMode.interactable = false;
            GameParameters.CursorMode = AgentDynamicParameter.Tension;
            
            godMode.onValueChanged.AddListener(OnGodModeToggle);
            cursorMode.onValueChanged.AddListener(OnCursorMode);
        }

        private void OnGodModeToggle(bool value)
        {
            cursorMode.interactable = value;

            GameParameters.CursorMode = value ? m_cursorModeParameter : AgentDynamicParameter.Tension;
        }

        private void OnCursorMode(int value)
        {
            m_cursorModeParameter = (AgentDynamicParameter)value;

            if (cursorMode.interactable)
                GameParameters.CursorMode = m_cursorModeParameter;
            
            Debug.Log(GameParameters.CursorMode);
        }
    }
}
