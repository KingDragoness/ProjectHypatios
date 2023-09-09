using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public bool lockY = false;
    [ShowIf("lockY")] public float yPosition = 0f;
    public bool useRotation = false;

    private void Update()
    {
        if (target == null) return;

        if (lockY == false)
        {
            transform.position = target.position;
        }
        else
        {
            Vector3 pos = target.position;
            pos.y = yPosition;
            transform.position = pos;
        }
        
        if (useRotation) transform.rotation = target.rotation;
    }
}
