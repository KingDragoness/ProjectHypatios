using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiusLookWeaponLimit : MonoBehaviour
{

    public Transform player;
    public Transform XControllerTransform;

    public float rotateSpeed = 10;
    public float maxYRot = 20f;
    public float minYRot = -20f;
    public float maxXRot = 20f;
    public float minXRot = -20f;

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation((player.position + new Vector3(0,1,0)) - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        XControllerTransform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        LimitRot();
    }

    private void LimitRot()
    {
        float rotationY = Mathf.Clamp(transform.localEulerAngles.y, minYRot, maxYRot);
        float rotationX = Mathf.Clamp(transform.localEulerAngles.y, minXRot, maxXRot);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, transform.localEulerAngles.z);
        XControllerTransform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);

    }
}
