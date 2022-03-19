using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWeaponScript : MonoBehaviour
{

    public string weaponID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject weaponHolder = GameObject.FindGameObjectWithTag("GunHolder");
            weaponHolder.GetComponent<weaponManager>().AddWeapon(weaponID);
            Destroy(gameObject);
        }
    }
}
