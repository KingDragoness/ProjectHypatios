using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using WaypointsFree;

public class Interact_TravellatorLift : MonoBehaviour
{

    public WaypointsTraveler waypoint;
    public UnityEvent OnMoving;
    public UnityEvent OnNotMoving;
    public AudioSource audio_LiftSound;

    private float _timerCheck = 0.1f;
    private bool _bCloseCheck = false;


    private void Start()
    {
        OnNotMoving?.Invoke();
    }

    private void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        _timerCheck -= Time.deltaTime;

        if (_timerCheck < 0f)
        {
            if (waypoint.isObjectMoving && waypoint.enabled == true)
            {
                if (audio_LiftSound.isPlaying == false) audio_LiftSound.Play();
                if (_bCloseCheck == false)
                {
                    OnMoving?.Invoke();
                }

                _bCloseCheck = true;
            }
            else 
            {
                if (audio_LiftSound.isPlaying == true) audio_LiftSound.Stop();
                if (_bCloseCheck == true)
                {
                    OnNotMoving?.Invoke();
                }

                _bCloseCheck = false;
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
