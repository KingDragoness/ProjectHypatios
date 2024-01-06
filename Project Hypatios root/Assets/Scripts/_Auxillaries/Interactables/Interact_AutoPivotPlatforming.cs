using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_AutoPivotPlatforming : MonoBehaviour
{

    public DynamicObjectPivot cachedObjectPivot;

    public void StartPivot()
    {
        if (cachedObjectPivot != null)
        {
            return;
        }

        var newGO = new GameObject();
        newGO.gameObject.name = $"Pivot-{transform.parent.gameObject.name}";
        cachedObjectPivot = newGO.AddComponent<DynamicObjectPivot>();
        cachedObjectPivot.target = transform;
        Hypatios.Player.transform.SetParent(cachedObjectPivot.transform);
    }

    public void EndPivot()
    {
        if (cachedObjectPivot == null)
        {
            return;
        }

        Hypatios.Player.transform.SetParent(null);
        //Hypatios.Player.MaintainMomentum(); //maintain momentum //temporarily removed because the bug 

        Destroy(cachedObjectPivot.gameObject);
        Destroy(cachedObjectPivot.transform.parent.gameObject);
    }

}
