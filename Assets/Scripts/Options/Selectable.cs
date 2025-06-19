using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Options
{
    public class Selectable : MonoBehaviour, ISelectHandler
    {
        private static List<Selectable> _selectables = new();

        [SerializeField] private SelectedFlowers selectedFlowers;

        private void Awake()
        {
            _selectables.Add(this);
            
            selectedFlowers.OnSelect(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            foreach (Selectable selectable in _selectables)
            {
                selectable.selectedFlowers.OnSelect(selectable.gameObject == eventData.selectedObject);
            }
        }
    }
}
