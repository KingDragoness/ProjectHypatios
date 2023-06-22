using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCopyMain : MonoBehaviour
{

    public Camera mainCamera;
    [SerializeField] private Camera _currentCamera;


    private void Update()
    {
        if (Time.timeScale <= 0) return;
        _currentCamera.fieldOfView = mainCamera.fieldOfView;
    }

}
