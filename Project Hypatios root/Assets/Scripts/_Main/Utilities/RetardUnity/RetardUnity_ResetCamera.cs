using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetardUnity_ResetCamera : MonoBehaviour
{

    public Camera targetCam;



    private void OnDrawGizmos()
    {
        targetCam.transform.localPosition = Vector3.zero;
        targetCam.transform.localEulerAngles = Vector3.zero;
    }

}
