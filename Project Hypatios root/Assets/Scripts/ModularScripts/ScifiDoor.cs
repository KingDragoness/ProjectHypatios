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
    public AudioSource audioDoor;
    public bool playDoorClose = false;
    public bool playDoorOpen = false;
    public bool playDoorToggle = false;
    public float speed = 4;

    private bool toggle_Open = false;
    private bool isDoorOpened = false;

    void Start()
    {
        if (defaultClosed)
        {
            toggle_Open = !defaultClosed;
            Close();
        }
        else
        {
            toggle_Open = !defaultClosed;
            Open();
        }
    }

    [ContextMenu("Close")]
    public void Close()
    {

        if (playDoorClose && audioDoor != null && isDoorOpened == true && Time.timeSinceLevelLoad > 1)
            audioDoor.Play();

        iTween.MoveTo(DoorObject.gameObject, iTween.Hash("position", targetDoorClosed.position, "speed", speed * 2, "easetype", iTween.EaseType.linear));
        isDoorOpened = false;

    }

    [ContextMenu("Open")]
    public void Open()
    {
        if (playDoorOpen && audioDoor != null && isDoorOpened == false && Time.timeSinceLevelLoad > 1)
            audioDoor.Play();

        iTween.MoveTo(DoorObject.gameObject, iTween.Hash("position", targetDoorOpened.position, "speed", speed * 2, "easetype", iTween.EaseType.linear));
        isDoorOpened = true;

      
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

        if (playDoorToggle && audioDoor != null && Time.timeSinceLevelLoad > 1)
            audioDoor.Play();
    }


}
