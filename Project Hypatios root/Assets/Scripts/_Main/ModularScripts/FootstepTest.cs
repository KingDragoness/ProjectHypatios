using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FootstepTest : MonoBehaviour
{

    public float Y_pos_footstep = 0.5f;
    public UnityEvent FootStepEvent;
    private bool triggered = false;

    public void Update()
    {
        if (transform.position.y <= Y_pos_footstep)
        {
            if (triggered == false) TriggerFootstep();
        }
        else
        {
            triggered = false;
        }
    }

    private void TriggerFootstep()
    {
        FootStepEvent?.Invoke();
        triggered = true;
    }

}
