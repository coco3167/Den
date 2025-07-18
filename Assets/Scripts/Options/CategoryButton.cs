using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Options
{
    public class CategoryButton : MonoBehaviour, ISelectHandler
    {
        private static List<CategoryButton> _categoryButtons = new();

        [SerializeField] private Color m_unselected, m_selected;

        private bool suppressNextAccept = false;

        private Button m_button;
        private Canvas m_canvas;
        private void Awake()
        {
            _categoryButtons.Add(this);

            m_button = GetComponent<Button>();
            m_canvas = GetComponent<Canvas>();

            m_button.onClick.AddListener(Show);
            m_button.onClick.AddListener(HideOtherCategoryButtons);
        }

        public void AddButtonListener(UnityAction callAction)
        {
            if (!m_button)
                m_button = GetComponent<Button>();
            m_button.onClick.AddListener(callAction);
        }

        public void ClickOnButton()
        {
            suppressNextAccept = true;
            m_button.onClick.Invoke();
        }

        private void HideOtherCategoryButtons()
        {
            foreach (CategoryButton button in _categoryButtons)
            {
                if (button != this)
                    button.Hide();
            }
        }

        private void Show()
        {
            m_canvas.sortingOrder = 3;
            if (!suppressNextAccept)
                AudioManager.Instance.Accept.Post(gameObject);
            suppressNextAccept = false;
        }

        private void Hide()
        {
            m_canvas.sortingOrder = 1;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            AudioManager.Instance.Move.Post(gameObject);
        }

        private void OnDestroy()
        {
            _categoryButtons.Remove(this);
        }
    }
}
