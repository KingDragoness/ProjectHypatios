using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NoclipCamera : MonoBehaviour
{
    public float flySpeed = 0.5f;
    public UI_Modular_ShowTempCanvas canvasNoclip;
    public Text label_LockCamera;
    public MouseLook mouselook;
    public Camera noClipCam;

    private bool isEnabled = true;
    private bool shift;
    private bool ctrl;
    public float accelerationAmount = 3f;
    public float accelerationRatio = 1f;
    public float slowDownRatio = 0.5f;

    public static NoclipCamera instance;

    private void Awake()
    {
        AssignNoclip();
    }

    private void OnEnable()
    {
        var playerCam = Hypatios.MainCamera;
        noClipCam.farClipPlane = playerCam.farClipPlane;
        noClipCam.fieldOfView = playerCam.fieldOfView;
        noClipCam.clearFlags = playerCam.clearFlags;
        noClipCam.backgroundColor = playerCam.backgroundColor;
    }

    public void AssignNoclip()
    {
        instance = this;

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            isEnabled = !isEnabled;
            label_LockCamera.text = $"Lock camera control: {isEnabled.ToString()}";
            canvasNoclip.Show(4f);
            mouselook.enabled = isEnabled;
        }

        if (isEnabled == false)
        { return; }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            shift = true;
            flySpeed *= accelerationRatio;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            shift = false;
            flySpeed /= accelerationRatio;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ctrl = true;
            flySpeed *= slowDownRatio;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            ctrl = false;
            flySpeed /= slowDownRatio;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(-Vector3.forward * flySpeed * Input.GetAxis("Vertical") * Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate(-Vector3.right * flySpeed * Input.GetAxis("Horizontal") * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * flySpeed * 0.5f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(-Vector3.up * flySpeed * 0.5f * Time.deltaTime);
        }
    }
}

