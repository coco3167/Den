using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Options
{
    public class Selectable : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static List<Selectable> _selectables = new();

        [SerializeField] private SelectedFlowers selectedFlowers;

        private bool m_selected;

        private void Awake()
        {
            _selectables.Add(this);
            
            selectedFlowers.OnSelect(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            foreach (Selectable selectable in _selectables)
            {
                m_selected = selectable.gameObject == eventData.selectedObject;
                selectable.selectedFlowers.OnSelect(m_selected);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!m_selected)
                selectedFlowers.OnSelect(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!m_selected)
                selectedFlowers.OnSelect(false);
        }

        private void OnDestroy()
        {
            _selectables.Remove(this);
        }
    }
}
