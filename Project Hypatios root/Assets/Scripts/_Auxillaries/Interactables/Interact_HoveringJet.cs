using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_HoveringJet : MonoBehaviour
{

    public bool IsControlled = false;
    public Transform lockPlayerTransform;
    public float moveSpeed = 12f;
    public float rotationSpeed = 100f;
    public float floatingSpeed = 50f;

    private float xMovement;
    private float yMovement;
    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsControlled)
        {
            Vector3 desiredEulerRot = Hypatios.Player.transform.eulerAngles;
            desiredEulerRot.x = lockPlayerTransform.transform.eulerAngles.x;
            desiredEulerRot.z = lockPlayerTransform.transform.eulerAngles.z;

            Quaternion playerRot = Quaternion.Euler(desiredEulerRot);

            Hypatios.Player.transform.position = lockPlayerTransform.transform.position;
            Hypatios.Player.transform.rotation = playerRot;

            var moveVector = Hypatios.Input.Move.ReadValue<Vector2>();
            xMovement = moveVector.x;
            yMovement = moveVector.y;

            if (yMovement < 0f)
            {
                yMovement /= 1.5f;
            }

            float rotPowerNet = xMovement * rb.velocity.magnitude * rotationSpeed * Time.deltaTime;
            Quaternion q = transform.rotation; 
            q *= Quaternion.Euler(Vector3.up * rotPowerNet);

            transform.rotation = q;

            //rb.AddForce(transform.right * xMovement * moveSpeed * Time.deltaTime * 10f);
            rb.AddForce(transform.forward * yMovement * moveSpeed * Time.deltaTime * 25f);
            rb.AddForce(Vector3.up * floatingSpeed * rb.velocity.magnitude * Time.deltaTime * 25f);

        }
        else
        {

        }
    }

    [Button("DEBUG_Control Test")]
    public void TakeControl()
    {
        IsControlled = !IsControlled;
        if (IsControlled)
        {
            Hypatios.Player.isVehicleMode = true;
        }
        else
        {
            Hypatios.Player.isVehicleMode = false;
            Hypatios.Player.transform.SetParent(null);
            Hypatios.Player.transform.eulerAngles = new Vector3();
        }
    }

}
