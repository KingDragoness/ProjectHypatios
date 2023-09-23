using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level5_ConfiscateWeapon : MonoBehaviour
{

    public UnityEvent OnEventTriggered;

    public void TriggerConfiscate()
    {
        Hypatios.UI.mainHUDScript.FadeIn();
        Hypatios.Player.Weapon.ConsficateAllWeapon();
        Hypatios.Player.Inventory.RemoveAllItemByType(ItemInventory.Category.Weapon);
        OnEventTriggered?.Invoke();
    }

}
