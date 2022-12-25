using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelDisplay : MonoBehaviour
{
    
    [System.Serializable]
    public class DisplayWeapon
    {
        public WeaponItem weaponItem;
        public GameObject weaponDisplay;
    }

    public List<DisplayWeapon> displays = new List<DisplayWeapon>();
    public WeaponItem currentWeaponDisplay;

    private void OnEnable()
    {
        ActivateWeapon(displays.Find(x => x.weaponItem == currentWeaponDisplay));
    }

    public void ActivateWeapon(DisplayWeapon weapon)
    {
        foreach(var display1 in displays)
        {
            display1.weaponDisplay.gameObject.SetActive(false);
        }

        weapon.weaponDisplay.gameObject.SetActive(true);
    }

}
