using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber8_Gravity : MonoBehaviour
{

    public float targetGravityValue = 2f;
    private float defaultGravity = -9.81f;

    private void Start()
    {
        defaultGravity = Physics.gravity.y;
        Physics.gravity = new Vector3(0, targetGravityValue, 0);
    }

    private void OnDestroy()
    {
        Physics.gravity = new Vector3(0, defaultGravity, 0);

    }

}
