using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Basketball : InteractableObject
{

    public string s_Description_Grab = "Grab";
    public string s_Description_Throw = "Throw";
    public float throwForce = 1000f;
    public float variableForce = 1000f;
    public float minHeightFromPlayer = 0.6f;
    public Vector3 offsetCam = new Vector3(0, 0, 1f);
    public Rigidbody rb;

    private bool isGrabbing = false;
    private Vector3 _originPos;

    private void Start()
    {
        _originPos = transform.position;
    }


    public override string GetDescription()
    {
        if (isGrabbing == false)
        {
            return s_Description_Grab;
        }
        else
        {
            return s_Description_Throw;
        }
    }

    public override void Interact()
    {
        if (isGrabbing)
        {
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * (throwForce + Random.Range(0f, variableForce)));

            //throw
        }

        isGrabbing = !isGrabbing;
    }

    private void FixedUpdate()
    {
       
    }

    private void Update()
    {
        if (Time.timeScale <= 0f) return;

        if (isGrabbing)
        {
            Vector3 pos = Camera.main.transform.position;
            pos += Camera.main.transform.TransformDirection(offsetCam);
            float delta = pos.y - Hypatios.Player.transform.position.y;
            if (delta < minHeightFromPlayer)
            {
                pos.y = Hypatios.Player.transform.position.y + minHeightFromPlayer;
            }

            transform.position = pos;
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }

        if (transform.position.y < -99f)
        {
            transform.position = _originPos;
        }
    }

}
