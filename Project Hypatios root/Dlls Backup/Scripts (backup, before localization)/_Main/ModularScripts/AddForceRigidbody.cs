using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceRigidbody : MonoBehaviour
{

    public Vector3 force;
    public Vector3 relativeForce;
    public float multiplier = 1;
    public Rigidbody Rigidbody;

    public void AddForce()
    {
        Rigidbody.AddForce(force * multiplier);
        Rigidbody.AddRelativeForce(relativeForce * multiplier);
    }

}
