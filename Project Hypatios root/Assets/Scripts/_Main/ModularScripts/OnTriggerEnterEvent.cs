using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class OnTriggerEnterEvent : MonoBehaviour
{
    public UnityEvent triggerEvents;
    public UnityEvent triggerStayEvents;
    public UnityEvent triggerExitEvents;
    public GameObject objectToCompare;

    public bool usePlayer = false;
    private int currentFrame = 0; 
    //"currentFrame" prevents a bug where:
    //trigger enter occurs the same time as exit
    //trigger enter triggered twice

    private void Start()
    {
        if (usePlayer)
        {
            objectToCompare = Hypatios.Player.gameObject;
        }
    }

    [Button("Trigger")]
    public void TriggerManual()
    {
        triggerEvents?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (GetCurrentFrame == currentFrame)
            return;

        if (other.gameObject == objectToCompare)
        {
            triggerEvents?.Invoke();
            currentFrame = GetCurrentFrame;
        }

    }

    void OnTriggerStay(Collider other)
    {
        int time = Mathf.RoundToInt(Time.time * 10);

        if (other.gameObject == objectToCompare && time % 5 == 0)
        {
            triggerStayEvents?.Invoke();
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (GetCurrentFrame == currentFrame)
            return;

        if (other.gameObject == objectToCompare)
        {
            triggerExitEvents?.Invoke();
        }
    }

    public int GetCurrentFrame
    {
        get { return Mathf.RoundToInt(Time.time * 10); }
    }

}
