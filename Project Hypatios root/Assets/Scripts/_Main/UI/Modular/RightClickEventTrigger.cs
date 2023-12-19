using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class RightClickEventTrigger : MonoBehaviour, IPointerClickHandler
{

    public UnityEvent OnRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
        }
    }
}
