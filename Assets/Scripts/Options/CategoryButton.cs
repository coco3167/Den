using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Options
{
    public class CategoryButton : MonoBehaviour
    {
        private static List<CategoryButton> _categoryButtons = new();

        [SerializeField] private Color m_unselected, m_selected;
        
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
            m_button.onClick.AddListener(callAction);
        }

        public void ClickOnButton()
        {
            m_button.onClick.Invoke();
        }

        private void HideOtherCategoryButtons()
        {
            foreach (CategoryButton button in _categoryButtons)
            {
                if(button != this)
                    button.Hide();
            }
        }

        private void Show()
        {
            m_canvas.sortingOrder = 3;
        }

        private void Hide()
        {
            m_canvas.sortingOrder = 1;
        }
    }
}
