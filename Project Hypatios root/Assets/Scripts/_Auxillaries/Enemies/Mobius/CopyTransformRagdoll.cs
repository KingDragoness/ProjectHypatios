using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class CopyTransformRagdoll : MonoBehaviour
{
    public Transform[] ragdollTarget;
    [FoldoutGroup("DEBUG")] public CopyTransformRagdoll otherCopyRagdoll;

    [ContextMenu("Find Rigidbodies")]
    public void FindRigidbodyJoints()
    {
        List<Transform> allTs = new List<Transform>();
        var rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            allTs.Add(rb.transform);
        }
        ragdollTarget = allTs.ToArray();
    }


    [ContextMenu("Find same names")]
    public void FindSameNames()
    {
        List<Transform> allTs = new List<Transform>();
        var otherList = otherCopyRagdoll.ragdollTarget.ToList();
        var everyT = gameObject.GetComponentsInChildren<Transform>();
        foreach (var t in everyT)
        {
            if (otherList.Find(x => x.gameObject.name == t.gameObject.name) != null)
                allTs.Add(t.transform);
        }
        ragdollTarget = allTs.ToArray();
    }


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

    public void CopyRotationPosition(CopyTransformRagdoll originCopy)
    {
        int i = 0;

        foreach (var bone in originCopy.ragdollTarget)
        {
            ragdollTarget[i].transform.localPosition = bone.transform.localPosition;
            ragdollTarget[i].transform.localRotation = bone.transform.localRotation;
            i++;
        }
    }

    public void CopyShadow(CopyTransformRagdoll origin)
    {
        var copy1 = Instantiate(origin, transform.position, transform.rotation);
        int i = 0;
        copy1.gameObject.SetActive(true);

        foreach (var bone in copy1.ragdollTarget)
        {
            bone.transform.localPosition = ragdollTarget[i].transform.localPosition;
            bone.transform.localRotation = ragdollTarget[i].transform.localRotation;
            i++;
        }
    }
    
}
