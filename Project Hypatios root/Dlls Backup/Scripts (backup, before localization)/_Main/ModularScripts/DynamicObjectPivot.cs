using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPivot : MonoBehaviour
{

    public Transform target;

    private void Start()
    {
        Initiate();
    }

    public void Initiate()
    {
        var parentObj = new GameObject();
        parentObj.transform.position = target.transform.position;
        transform.SetParent(parentObj.transform);
        parentObj.gameObject.name = $"PivotObject ({gameObject.name})";
        var taScript = parentObj.AddComponent<FollowObject>();
        taScript.target = target;
    }

}
