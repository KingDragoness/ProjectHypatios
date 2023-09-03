using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMapCamera : MonoBehaviour
{

    public Camera cam;
    public Vector3 maximumExtend;
    public Vector3 center;
    public float minFOV = 2f;
    public float maxFOV = 12f;
    public float ZoomSpeed = 2f;
    public float ZoomSpeed_key = 2f;
    public float PanSpeed = 20f; 
    public Transform baitTransformCam;

    private Vector3 lastPanPosition;
    private bool isMouse = false;
    private float _currentFOV = 4f;


    private void Update()
    {
        isMouse = false;

        HandleMouse();
        CorrectPosition();
        HandlePOV();

    }
    private void HandleMouse()
    {

        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            isMouse = true;
            PanCamera(Input.mousePosition);
        }

  

    }

    private void HandlePOV()
    {
        _currentFOV -= Input.mouseScrollDelta.y * ZoomSpeed;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _currentFOV += Time.unscaledDeltaTime * ZoomSpeed_key;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _currentFOV -= Time.unscaledDeltaTime * ZoomSpeed_key;
        }

        _currentFOV = Mathf.Clamp(_currentFOV, minFOV, maxFOV);
        cam.fieldOfView = _currentFOV;
    }

    void CorrectPosition()
    {
        Vector3 correctedPosition = transform.position;

        correctedPosition.x = Mathf.Clamp(correctedPosition.x, center.x - maximumExtend.x, center.x + maximumExtend.x);
        correctedPosition.y = Mathf.Clamp(correctedPosition.y, center.y - maximumExtend.y, center.y + maximumExtend.y);
        correctedPosition.z = Mathf.Clamp(correctedPosition.z, center.z - maximumExtend.z, center.z + maximumExtend.z);

        transform.position = correctedPosition;
    }

    void PanCamera(Vector3 newPanPosition)
    {
        float panSpeedAdjusted = PanSpeed * (_currentFOV / minFOV);

        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * panSpeedAdjusted, offset.y * panSpeedAdjusted, offset.y * panSpeedAdjusted);

        Vector3 eulerCam;
        eulerCam.x = 0;
        eulerCam.y = transform.eulerAngles.y;
        eulerCam.z = 0;
        baitTransformCam.eulerAngles = eulerCam;

        // Perform the movement
        transform.Translate(move, baitTransformCam);

        // Cache the position
        lastPanPosition = newPanPosition;
    }


}
