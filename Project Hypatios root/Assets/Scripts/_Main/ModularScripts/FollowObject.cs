﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;
    public bool useRotation = false;

    private void Update()
    {
        if (target == null) return;
        transform.position = target.position;
        if (useRotation) transform.rotation = target.rotation;
    }
}