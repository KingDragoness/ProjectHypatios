using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWeaponScript : MonoBehaviour
{

    public string weaponID;
    private bool b = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !b)
        {
            GameObject weaponHolder = GameObject.FindGameObjectWithTag("GunHolder");
            weaponHolder.GetComponent<WeaponManager>().AddWeapon(weaponID);
            Destroy(gameObject);
            b = true;
        }
    }
}
