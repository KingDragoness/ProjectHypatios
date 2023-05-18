using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_PlayModeCamera : MonoBehaviour
{

    public MainMenuTitleScript titleScript;
    public float acceleration = 2f;
    public float decceleration = 2f;
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    private float _curSpeedX = 0f;
    private float _curSpeedZ = 0f;
    private LevelSelect_Legend currentLegend;

    private void Update()
    {
        HandleCameraPanning();
    }

    private void HandleCameraPanning()
    {
        Vector3 pos = transform.position;
        var dir = transform.forward;
        float targetX = 0f;
        float targetZ = 0f;

        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            targetZ = 1f;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            targetZ = -1f;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            targetX = 1f;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            targetX = -1f;
        }

        pos.x += panSpeed * Time.deltaTime * _curSpeedX;
        pos.z += panSpeed * Time.deltaTime * _curSpeedZ;

        if (Mathf.Abs(targetX) > 0.01f) _curSpeedX = Mathf.MoveTowards(_curSpeedX, targetX, Time.deltaTime * 0.1f * acceleration);
        else _curSpeedX = Mathf.MoveTowards(_curSpeedX, targetX, Time.deltaTime * 0.1f * decceleration);

        if (Mathf.Abs(targetZ) > 0.01f) _curSpeedZ = Mathf.MoveTowards(_curSpeedZ, targetZ, Time.deltaTime * 0.1f * acceleration);
        else _curSpeedZ = Mathf.MoveTowards(_curSpeedZ, targetZ, Time.deltaTime * 0.1f * decceleration);

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;
    }

    private void FixedUpdate()
    {
        RaycastMouse();
    }

    private void RaycastMouse()
    {
        bool noInteract = true;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isResuming = titleScript.IsTriggeringResume;

        if (isResuming == false)
        {
            if (Physics.Raycast(ray, out hit, 61f))
            {
                var legend = hit.collider.GetComponent<LevelSelect_Legend>();

                if (legend == null)
                {
                    legend = hit.collider.GetComponentInParent<LevelSelect_Legend>();
                }

                if (legend != null)
                {
                    if (currentLegend != null) { if (currentLegend != legend) currentLegend.StopHovering(); }
                    currentLegend = legend;
                    currentLegend.OnHovering();

                    if (Input.GetMouseButtonUp(0))
                    {
                        currentLegend.Click();
                    }

                    noInteract = false;
                }
            }


            if (noInteract)
            {
                if (currentLegend != null)
                {
                    currentLegend.StopHovering();
                }
                currentLegend = null;
            }
        }
        else
        {
            if (currentLegend != null)
            {
                currentLegend.StopHovering();
            }
            currentLegend = null;

        }
    }



}
