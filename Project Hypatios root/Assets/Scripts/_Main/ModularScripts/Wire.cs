using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wire : MonoBehaviour
{

    public LineRenderer linerenderer;
    public Transform origin;
    public Transform target;

    void Update()
    {
        if (linerenderer == null | origin == null | target == null) return;

        linerenderer.SetPosition(0, origin.transform.position);
        linerenderer.SetPosition(1, target.transform.position);

    }
}
