using Sirenix.OdinInspector;
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
        [SerializeField] private Button quit;
        
        [Title("Pop Up")]
        [SerializeField] private GameObject popUp;
        [SerializeField] private Button apply;
        [SerializeField] private Button back;

        private AgentDynamicParameter m_cursorModeParameter = AgentDynamicParameter.Tension;

        private void Awake()
        {
            cursorMode.interactable = false;
            GameParameters.CursorMode = AgentDynamicParameter.Tension;
            
            godMode.onValueChanged.AddListener(OnGodModeToggle);
            cursorMode.onValueChanged.AddListener(OnCursorMode);
            quit.onClick.AddListener(Quit);
            
            apply.onClick.AddListener(OnGodModeApply);
            back.onClick.AddListener(OnGodModeBack);
            
            popUp.SetActive(false);
        }

        private void OnGodModeToggle(bool value)
        {
            cursorMode.interactable = false;
            GameParameters.CursorMode = AgentDynamicParameter.Tension;

            if (value)
            {
                popUp.SetActive(true);
            }
        }

        private void OnGodModeApply()
        {
            cursorMode.interactable = true;
            GameParameters.CursorMode = m_cursorModeParameter;
            godMode.isOn = true;
            popUp.SetActive(false);
        }

        private void OnGodModeBack()
        {
            godMode.isOn = false;
            popUp.SetActive(false);
        }

        private void Quit()
        {
            Application.Quit();
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
