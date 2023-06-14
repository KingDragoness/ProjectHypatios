using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class KanonelektromagnetWeapon : GunScript
{

    public enum WeaponMode
    {
        Horizontal,
        Vertical
    }

    [FoldoutGroup("Kanonelektromagnetik")] public float X_dist = 10f;
    [FoldoutGroup("Kanonelektromagnetik")] public float Y_dist = 10f;
    [FoldoutGroup("Kanonelektromagnetik")] public Sprite horizontalCrosshair;
    [FoldoutGroup("Kanonelektromagnetik")] public Sprite verticalCrosshair;
    [FoldoutGroup("Kanonelektromagnetik")] public WeaponMode currentMode;
    [FoldoutGroup("Kanonelektromagnetik")] public Animator barrelAnimator;
    [FoldoutGroup("Kanonelektromagnetik")] public UnityEvent OnSwitchBarrelMode;

    private MainGameHUDScript hudScript;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        HandleBarrelGun();
    }

    private void OnEnable()
    {
        hudScript = MainGameHUDScript.Instance;
        CrosshairChange(currentMode);
    }

    private void HandleBarrelGun()
    {
        if (currentMode == WeaponMode.Horizontal)
        {
            barrelAnimator.SetBool("isHorizontal", true);
        }
        else if (currentMode == WeaponMode.Vertical)
        {
            barrelAnimator.SetBool("isHorizontal", false);
        }

        if (Hypatios.Input.Fire2.triggered)
        {
            if (currentMode == WeaponMode.Horizontal)
            {
                currentMode = WeaponMode.Vertical;
                CrosshairChange(WeaponMode.Vertical);
            }
            else if (currentMode == WeaponMode.Vertical)
            {
                currentMode = WeaponMode.Horizontal;
                CrosshairChange(WeaponMode.Horizontal);
            }
            OnSwitchBarrelMode?.Invoke();
        }
    }

    private void CrosshairChange(WeaponMode weaponMode)
    {
        if (currentMode == WeaponMode.Horizontal)
        {
            hudScript.SwapCrosshair(horizontalCrosshair);
        }
        else if (currentMode == WeaponMode.Vertical)
        {
            hudScript.SwapCrosshair(verticalCrosshair);
        }
    }

    public override void FireWeapon()
    {
        gunRecoil.RecoilFire();

        if (audioFire != null)
        {
            audioFire.Play();
        }

        var damageToken = new DamageToken();
        damageToken.isBurn = isBurnBullet;
        damageToken.isPoison = isPoisonBullet;
        damageToken.origin = DamageToken.DamageOrigin.Player;
        damageToken.damageType = damageType;
        OnFire?.Invoke();
        OnFireAction?.Invoke($"{weaponName}");

        if (isBurst)
        {
            muzzle1.Emit(1);
            var points = new Vector3[2];
            points[0] = bulletShooter.transform.position;
            RaycastHit hit;

            float dist = 0;

            if (currentMode == WeaponMode.Horizontal)
                dist = X_dist;
            else if (currentMode == WeaponMode.Vertical) 
                dist = Y_dist;


            int totalDist = Mathf.RoundToInt(dist * burstAmount);

            for (int i = 0; i < burstAmount; i++)
            {
                if (curAmmo <= 1) break;
                GameObject trace = Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
                if (i != 0) curAmmo--;

                var screenPos = Input.mousePosition;
                if (currentMode == WeaponMode.Horizontal)
                {
                    screenPos.x += (-totalDist/2f) + (dist * i);
                }
                else if (currentMode == WeaponMode.Vertical)
                {
                    screenPos.y += (-totalDist / 2f) + (dist * i);
                }

                Ray ray = Camera.main.ScreenPointToRay(screenPos);

                if (Physics.Raycast(ray, out hit, 1000f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
                {
                    var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
                    float variableDamage = Random.Range(0, variableAdditionalDamage);

                    if (damageReceiver != null)
                    {
                        damageToken.damage = (damage * Hypatios.Player.BonusDamageGun.Value) + variableDamage; damageToken.repulsionForce = repulsionForce;
                        UniversalDamage.TryDamage(damageToken, hit.collider.transform, transform);
                        HandleCrosshairActive(damageReceiver);
                    }

                    points[1] = hit.point;
                    trace.GetComponent<LineRenderer>().SetPositions(points);
                    Destroy(trace, .03f);

                    if (hit.transform.gameObject.layer != 13 &&
                        hit.transform.gameObject.layer != 12)
                    {
                        GameObject bulletHole = Hypatios.ObjectPool.SummonObject(bulletImpact, 10, IncludeActive: true);
                        if (bulletHole != null)
                        {
                            bulletHole.transform.position = hit.point + hit.normal * .0001f;
                            bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
                            bulletHole.transform.SetParent(hit.collider.gameObject.transform);
                        }

                    }

                    GameObject bulletSpark_ = Hypatios.ObjectPool.SummonObject(bulletSparks, 10, IncludeActive: true);
                    if (bulletSpark_ != null)
                    {
                        bulletSpark_.transform.position = hit.point;
                        bulletSpark_.transform.rotation = Quaternion.LookRotation(hit.normal);
                        bulletSpark_.DisableObjectTimer(2f);
                    }

                    currentHit = hit;
                }
                else
                {
                    points[1] = cam.ViewportToWorldPoint(new Vector3(.5f, .5f, 100f));
                    trace.GetComponent<LineRenderer>().SetPositions(points);
                    Destroy(trace, .03f);
                }

            }
        }
    }

}
