using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EastriaGuardTest : MonoBehaviour
{

    public Transform target;
    public float speed;

    private void Update()
    {
        // direction towards target
        Vector3 relativePos = target.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
    }


}
