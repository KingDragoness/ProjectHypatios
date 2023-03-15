using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening_Controller : MonoBehaviour
{
    public int framerateTarget = 31;

    private void Start()
    {
        Application.targetFrameRate = framerateTarget;
    }
}
