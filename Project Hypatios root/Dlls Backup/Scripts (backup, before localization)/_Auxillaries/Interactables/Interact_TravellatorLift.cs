using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WaypointsFree;

public class Interact_TravellatorLift : MonoBehaviour
{

    public WaypointsTraveler waypoint;
    public AudioSource audio_LiftSound;

    private float _timerCheck = 0.1f;

    private void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        _timerCheck -= Time.deltaTime;

        if (_timerCheck < 0f)
        {
            if (waypoint.isObjectMoving)
            {
                if (audio_LiftSound.isPlaying == false) audio_LiftSound.Play();
            }
            else 
            {
                if (audio_LiftSound.isPlaying == true) audio_LiftSound.Stop();
            }
            _timerCheck = 0.1f;
        }
    }

    public void CallLift(bool isEndPoint = false)
    {
        ResumeLift();
        if (isEndPoint)
        {
            waypoint.SetIndexCounter(-1);
        }
        else
        {
            waypoint.SetIndexCounter(1);
        }
    }

    public void ToggleLift()
    {
        ResumeLift();
        waypoint.SetInverseIndex();
    }


    public void ResumeLift()
    {
        waypoint.enabled = true;
    }
    public void StopLift()
    {
        waypoint.enabled = false;
    }

}
