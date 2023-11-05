using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class WIRED_Boulder_Tetraroid : MonoBehaviour
{

    public NavMeshAgent agent;
    public Animator anim;
    public Transform rockTarget;
    public float thresholdDist_Push = 1.5f;

    private bool _isPushingRock = false;

    private void Update()
    {
        if (Time.timeScale <= 0f) return;


        float dist = Vector3.Distance(transform.position, rockTarget.position);

        if (dist < thresholdDist_Push)
        {
            _isPushingRock = true;
        }
        else
        {
            _isPushingRock = false;
        }

        if (_isPushingRock)
        {
            anim.SetBool("Push", true);
        }
        else
        {
            anim.SetBool("Push", false);
            agent.SetDestination(rockTarget.position);
        }


    }


}
