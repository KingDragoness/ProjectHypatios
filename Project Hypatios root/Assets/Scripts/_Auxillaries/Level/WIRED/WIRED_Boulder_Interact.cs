using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class WIRED_Boulder_Interact : MonoBehaviour
{

    public float MoveTime = 0.4f;
    public float MoveSpeed = 10f;
    public float RotationSpeed = 5f;
    public float randomPitchDeviation = 0.05f;
    public AudioSource audio_RockPush;
    public UnityEvent OnPushEvent;
    public Vector3 dirRotation = new Vector3(0, 0, 1f);
    public Transform targetEndPoint; //translate towards

    private float _timerMove = 0f;

    private void Update()
    {
        if (_timerMove > 0f)
        {
            var step = MoveSpeed * Time.deltaTime; // 
            Vector3 dir =  targetEndPoint.position - transform.position;
            dir.Normalize();
            _timerMove -= Time.deltaTime;

            transform.Rotate(dirRotation.normalized, RotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetEndPoint.position, step);
        }
    }


    public void PushRock()
    {
        _timerMove = MoveTime;
        audio_RockPush?.Play();
        OnPushEvent?.Invoke();
    }

}
