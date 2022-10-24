using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformAttacher : MonoBehaviour
{

    public Transform targetCopy;

    private Vector3 offsetPosition;
    private Vector3 offsetRotation;

    private void Start()
    {
        offsetPosition = transform.localPosition;
        offsetRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        transform.position = targetCopy.position;//+ offsetPosition;
        transform.eulerAngles = targetCopy.eulerAngles;// + offsetRotation;

    }


}
