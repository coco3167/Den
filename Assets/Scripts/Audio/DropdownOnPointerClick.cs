using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownOnPointerClick : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> onDropdownOpened;

    public void OnPointerClick(PointerEventData eventData)
    {
        onDropdownOpened?.Invoke(eventData);
    }
}
