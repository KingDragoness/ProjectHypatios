using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [SerializeField] float swayMultiplier = 1f;
    [SerializeField] float smooth = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float y = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        Quaternion rotX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotY = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion targetRot = rotX * rotY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, smooth * Time.deltaTime);
    }
}
