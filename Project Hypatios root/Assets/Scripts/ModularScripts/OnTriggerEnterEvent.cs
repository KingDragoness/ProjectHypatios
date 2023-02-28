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
        if (other.gameObject == objectToCompare)
        {
            triggerEvents?.Invoke();
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
        if (other.gameObject == objectToCompare)
        {
            triggerExitEvents?.Invoke();
        }
    }

}
