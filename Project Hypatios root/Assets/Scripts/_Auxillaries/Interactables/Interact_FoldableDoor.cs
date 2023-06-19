using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_FoldableDoor : MonoBehaviour
{

    public AnimatorSetBool animationScript;
    public UnityEvent OnOpenedDoor;
    public UnityEvent OnClosedDoor;

    private bool _prevBool = false;
    private bool _isOpened = false;

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (_prevBool != _isOpened)
        {
            if (_isOpened)
            {
                TriggerOpenedDoorEvent();
            }
            else
            {
                TriggerClosedDoorEvent();
            }
        }

        _prevBool = _isOpened;
    }

    public void TriggerOpenedDoorEvent()
    {
        OnOpenedDoor?.Invoke();
        animationScript.SetBool(true);
    }
    public void TriggerClosedDoorEvent()
    {
        OnClosedDoor?.Invoke();
        animationScript.SetBool(false);
    }

    public void OpenDoor(bool isOpen)
    {
        _isOpened = isOpen;
    }

    public void ToggleDoor()
    {
        OpenDoor(!_isOpened);
    }

}
