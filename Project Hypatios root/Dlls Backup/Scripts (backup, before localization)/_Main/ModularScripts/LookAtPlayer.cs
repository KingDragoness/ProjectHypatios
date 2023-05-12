using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LookAtPlayer : MonoBehaviour
{

    public bool useSlowlyRotate = false;
    [ShowIf("useSlowlyRotate", true)]  public float speedRotate = 10;
    [ShowIf("useSlowlyRotate", true)] public float errorMargin = 0f;

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
            Vector3 target = targetLook.position;
            target.x += Random.Range(-errorMargin, errorMargin);
            target.y += Random.Range(-errorMargin, errorMargin);
            target.z += Random.Range(-errorMargin, errorMargin);
            var q = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);
        }
    }

    [ContextMenu("Look At Target")]
    public void LookNow()
    {
        transform.LookAt(targetLook);

    }

}
