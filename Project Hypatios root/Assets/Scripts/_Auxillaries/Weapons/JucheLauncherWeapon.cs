using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class JucheLauncherWeapon : GunScript
{

    [FoldoutGroup("Nuke Launcher")] public ThrowProjectileEnemy throwProjectile;
    [FoldoutGroup("Nuke Launcher")] public Transform origin;

    public override void FireWeapon()
    {
        //launch juche missile
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        throwProjectile.FireProjectile(origin.forward);

        origin.transform.position = cam.transform.position;
        origin.transform.rotation = cam.transform.rotation;

        OnFire?.Invoke();
        OnFireAction?.Invoke($"{weaponName}");



        //base.FireWeapon();
    }

}
