using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformRagdoll : MonoBehaviour
{
    public Transform[] ragdollTarget;

    public void CopyRotationPosition(Transform[] origin)
    {
        int i = 0;

        foreach(var bone in origin)
        {
            ragdollTarget[i].transform.localPosition = bone.transform.localPosition;
            ragdollTarget[i].transform.localRotation = bone.transform.localRotation;
            i++;
        }
    }
}
