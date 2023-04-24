using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCurrentWeaponCheck : MonoBehaviour
{

    public ItemInventory weaponItem;
    public UnityEvent OnWeaponMatched;
    public bool disableCheck = false;
    private bool hasTriggered = false;

    public void DisableCheck(bool disable)
    {
        disableCheck = disable;
    }

    public void Check()
    {
        if (disableCheck) return;
        bool isMatch = weaponItem.GetID() == Hypatios.Player.Weapon.currentGunHeld.weaponName;

        if (isMatch)
        {
            OnWeaponMatched?.Invoke();
            hasTriggered = true;
        }
    }

}
