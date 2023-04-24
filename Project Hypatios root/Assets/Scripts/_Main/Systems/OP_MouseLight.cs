using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OP_MouseLight : MonoBehaviour
{
    public GameObject mouseLight;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 999f))
        {
            Vector3 hitTarget = hit.point;
            hitTarget += hit.normal * 0.2f;
            mouseLight.transform.position = hitTarget;
            mouseLight.gameObject.SetActive(true);
        }
        else
        {
            mouseLight.gameObject.SetActive(false);
        }
    }
}
