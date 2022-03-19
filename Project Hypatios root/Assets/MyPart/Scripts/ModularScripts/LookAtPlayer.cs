using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LookAtPlayer : MonoBehaviour
{

    public bool useSlowlyRotate = false;
    [ShowIf("useSlowlyRotate", true)]
    public float speedRotate = 10;

    public Transform targetLook;

    public void Update()
    {
        if (targetLook == null) return;

        if (useSlowlyRotate == false)
        {
            transform.LookAt(targetLook);
        }
        else
        {
            var q = Quaternion.LookRotation(targetLook.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);
        }
    }

}
