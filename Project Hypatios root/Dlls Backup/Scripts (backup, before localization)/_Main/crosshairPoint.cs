using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crosshairPoint : MonoBehaviour
{

    Ray crossRay;
    RaycastHit hitInfo;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        crossRay.origin = cam.transform.position;
        crossRay.direction = cam.transform.forward;
        Physics.Raycast(crossRay, out hitInfo, Mathf.Infinity);
        transform.position = hitInfo.point;
    }
}
