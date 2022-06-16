using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScifiDoor : MonoBehaviour
{

    public Transform targetDoorClosed;
    public Transform targetDoorOpened;
    public Transform DoorObject;
    public bool defaultClosed = false;
    public float speed = 4;

    private bool toggle_Open = false;

    void Start()
    {
        if (defaultClosed)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    [ContextMenu("Close")]
    public void Close()
    {
        iTween.MoveTo(DoorObject.gameObject, iTween.Hash("position", targetDoorClosed.position, "speed", speed * 2, "easetype", iTween.EaseType.linear));
    }

    [ContextMenu("Open")]
    public void Open()
    {
        iTween.MoveTo(DoorObject.gameObject, iTween.Hash("position", targetDoorOpened.position, "speed", speed * 2, "easetype", iTween.EaseType.linear));

    }

    [Button("Toggle Elevator")]
    [ContextMenu("Toggle")]
    public void Toggle()
    {
        toggle_Open = !toggle_Open;

        if (toggle_Open)
        {
            Open();
        }
        else
        {
            Close();
        }

    }


}
