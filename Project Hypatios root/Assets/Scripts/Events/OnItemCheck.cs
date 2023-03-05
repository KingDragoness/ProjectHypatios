using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnItemCheck : MonoBehaviour
{

    public UnityEvent ConditionSuccess;
    public UnityEvent ConditionFailed;
    public ItemInventory item;
    [Range(1,99)] public int amount = 1;


    private bool isExecuted = false;

    private void Start()
    {
        IsSuccess();
    }

    public bool IsSuccess()
    {
        var deFUCK = item;
        deFUCK.GetInstanceID();
        int count = Hypatios.Player.Inventory.Count(deFUCK.name);

        if (count >= amount)
        {
            ConditionSuccess?.Invoke();

            return true;
        }

        ConditionFailed?.Invoke();
        return false;
    }

}
