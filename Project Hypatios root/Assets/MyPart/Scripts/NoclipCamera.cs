using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoclipCamera : MonoBehaviour
{
    public float flySpeed = 0.5f;
    private bool isEnabled;
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

    public void AssignNoclip()
    {
        instance = this;

    }

    void Update()
    {
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

