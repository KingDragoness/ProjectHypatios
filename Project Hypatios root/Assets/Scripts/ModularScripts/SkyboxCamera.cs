using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{

    public Camera originCamera;
    public Camera theCamera;

    public float parallaxEffect = 250f;

    private void Update()
    {
        theCamera.transform.rotation = originCamera.transform.rotation;
        theCamera.fieldOfView = originCamera.fieldOfView;

        theCamera.transform.localPosition = originCamera.transform.position/ parallaxEffect;
    }

}
