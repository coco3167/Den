using Sirenix.OdinInspector;
using SmartObjects_AI.Agent;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Options.Categories
{
    public class General : MonoBehaviour
    {
        [SerializeField] private Toggle godMode;
        [SerializeField] private TMP_Dropdown cursorMode;
        [SerializeField] private GameObject cursorModeGameObject;
        [SerializeField] private Toggle closeCaptionning;
        [SerializeField] private Button quit;
        
        [Title("Pop Up")]
        [SerializeField] private GameObject popUp;
        [SerializeField] private Button apply;
        [SerializeField] private Button back;

        private AgentDynamicParameter m_cursorModeParameter = AgentDynamicParameter.Tension;
        private Navigation m_godNavigation, m_closeCaptionNavigation;

        private void Awake()
        {
            cursorMode.interactable = false;
            GameParameters.CursorMode = AgentDynamicParameter.Tension;
            
            godMode.onValueChanged.AddListener(OnGodModeToggle);
            cursorMode.onValueChanged.AddListener(OnCursorMode);
            closeCaptionning.onValueChanged.AddListener(OnCloseCaptionningToggle);
            quit.onClick.AddListener(Quit);
            
            apply.onClick.AddListener(OnGodModeApply);
            back.onClick.AddListener(OnGodModeBack);
            
            cursorModeGameObject.SetActive(false);
            popUp.SetActive(false);

            m_godNavigation = godMode.navigation;
            m_godNavigation.selectOnDown = closeCaptionning;
            godMode.navigation = m_godNavigation;

            m_closeCaptionNavigation = closeCaptionning.navigation;
            m_closeCaptionNavigation.selectOnUp = godMode;
            closeCaptionning.navigation = m_closeCaptionNavigation;
        }

        private void OnGodModeToggle(bool value)
        {
            cursorMode.interactable = false;
            cursorModeGameObject.SetActive(false);
            GameParameters.CursorMode = AgentDynamicParameter.Tension;

            m_godNavigation.selectOnDown = closeCaptionning;
            godMode.navigation = m_godNavigation;

            m_closeCaptionNavigation.selectOnUp = godMode;
            closeCaptionning.navigation = m_closeCaptionNavigation;
            
            if (value)
            {
                popUp.SetActive(true);
                EventSystem.current.SetSelectedGameObject(back.gameObject);
            }
        }

        private void OnGodModeApply()
        {
            cursorMode.interactable = true;
            GameParameters.CursorMode = m_cursorModeParameter;
            cursorModeGameObject.SetActive(true);
            godMode.isOn = true;
            popUp.SetActive(false);
            
            m_godNavigation.selectOnDown = cursorMode;
            godMode.navigation = m_godNavigation;

            m_closeCaptionNavigation.selectOnUp = cursorMode;
            closeCaptionning.navigation = m_closeCaptionNavigation;
            
            EventSystem.current.SetSelectedGameObject(cursorMode.gameObject);
        }

        private void OnGodModeBack()
        {
            cursorModeGameObject.SetActive(false);
            godMode.isOn = false;
            popUp.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(godMode.gameObject);
        }

        private void OnCloseCaptionningToggle(bool value)
        {
            GameParameters.HasCloseCaptions = value;
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
