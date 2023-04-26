using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilMirrorUI : MonoBehaviour
{

    public Recoil cameraRecoil;

    private Vector3 curRot;
    private Vector3 targetRot;
    
    [SerializeField] private float snappiness;
    [SerializeField] private float swayMultiplier = 1f;
    [SerializeField] private float smooth = 5f;
    [SerializeField] private float multiplier = 0.3f;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    void Update()
    {
        if (cameraRecoil == null)
        {
            return;
        }
        
        float x = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float y = Input.GetAxisRaw("Mouse Y") * swayMultiplier;


        targetRot = cameraRecoil.TargetRot * multiplier;
        curRot = Vector3.Slerp(curRot, targetRot, snappiness * Time.deltaTime);
        Quaternion q1 = Quaternion.Euler(curRot);

        recoilX = cameraRecoil.recoilX;
        recoilY = cameraRecoil.recoilY;
        recoilZ = cameraRecoil.recoilZ;

        //Camerasway
        {
            Quaternion rotX = Quaternion.AngleAxis(-y, Vector3.right);
            Quaternion rotY = Quaternion.AngleAxis(x, Vector3.up);

            Quaternion swayRotationTarget = rotX * rotY;

            transform.localRotation = Quaternion.Slerp(q1, swayRotationTarget, smooth * Time.deltaTime);
        }
    }
}
