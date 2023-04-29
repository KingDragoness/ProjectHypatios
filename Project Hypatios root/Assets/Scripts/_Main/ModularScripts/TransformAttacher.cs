using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformAttacher : MonoBehaviour
{

    public Transform targetCopy;
    public bool useLateUpdate = false;

    private Vector3 offsetPosition;
    private Vector3 offsetRotation;

    private void Start()
    {
        offsetPosition = transform.localPosition;
        offsetRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        if (useLateUpdate) return;
        transform.position = targetCopy.position;//+ offsetPosition;
        transform.eulerAngles = targetCopy.eulerAngles;// + offsetRotation;

    }

    private void LateUpdate()
    {
        if (!useLateUpdate) return;
        transform.position = targetCopy.position;//+ offsetPosition;
        transform.eulerAngles = targetCopy.eulerAngles;// + offsetRotation;
    }

    [ContextMenu("Copy Transform")]
    private void CopyTransform()
    {
        transform.position = targetCopy.position;
        transform.eulerAngles = targetCopy.eulerAngles;
    }


}
