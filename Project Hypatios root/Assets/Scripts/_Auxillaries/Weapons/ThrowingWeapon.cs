using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : BaseWeaponScript
{

    public float cooldownAttack = 1f;
    public bool isFiring = false;

    private float nextAttackTime = 0f;

    private void Update()
    {
        FireInput();
    }

    public override void FireInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isFiring)
            {
                isFiring = true;
                anim.SetBool("isFiring", isFiring);
            }

            if (Time.time >= nextAttackTime)
            {
                FireWeapon();
                nextAttackTime = Time.time + cooldownAttack;
                curAmmo--;
            }
        }
    }

    public override void FireWeapon()
    {
        //instantiate projectile
        Debug.Log("Spawn projectile");

    }

}
