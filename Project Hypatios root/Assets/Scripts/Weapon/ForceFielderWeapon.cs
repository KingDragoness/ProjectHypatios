using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ForceFielderWeapon : GunScript
{
    [FoldoutGroup("Force field Weapon")] public GameObject forceFielderPrefab;
    [FoldoutGroup("Force field Weapon")] public UnityEvent OnChargeReady;
    [FoldoutGroup("Force field Weapon")] public Transform origin;
    [FoldoutGroup("Force field Weapon")] public DissolveMaterialScript dissolveScript;
    [FoldoutGroup("Force field Weapon")] public GameObject text_ChargingField;
    [FoldoutGroup("Force field Weapon")] public GameObject text_Ready;
    [FoldoutGroup("Force field Weapon")] public float FieldRevTimer = 5f;

    private float cooldownFire = 0.1f;
    private float fieldReadyTimer = 5f;
    private bool isReadyField = false;

    private void OnEnable()
    {

        AmmoRefresh();
    }

    public override void Start()
    {
        base.Start();
        if (curAmmo == 0)
            fieldReadyTimer = -1f;

        AmmoRefresh();
    }

    public override void Update()
    {
        base.Update();
        cooldownFire -= Time.deltaTime;
        if (fieldReadyTimer >= 0f)
        {
            text_ChargingField.gameObject.SetActive(true);
            text_Ready.gameObject.SetActive(false);
            fieldReadyTimer -= Time.deltaTime;
            isReadyField = false;
        }
        else if (curAmmo > 0)
        {
            if (isReadyField == false)
            {
                OnChargeReady?.Invoke();
                isReadyField = true;
            }
            text_ChargingField.gameObject.SetActive(false);
            text_Ready.gameObject.SetActive(true);
        }
    }

    public override void FireInput()
    {


        if (Hypatios.Input.Fire1.WasReleasedThisFrame() && curAmmo > 0 && !isReloading && IsRecentlyPaused())
        {
            if (cooldownFire < 0f && fieldReadyTimer < 0f)
            {
                CreateForceField();
            }
        }


        if (Hypatios.Input.Fire1.IsPressed() && curAmmo > 0 && !isReloading)
        {
            if (!isFiring)
            {
                isFiring = true;
            }

        }
        else
        {
            if (isFiring)
            {
                isFiring = false;
            }

        }
    }

    private void AmmoRefresh()
    {
        if (curAmmo == 0)
        {
            dissolveScript.gameObject.SetActive(false);
            text_Ready.gameObject.SetActive(false);
            text_ChargingField.gameObject.SetActive(false);
            dissolveScript.ResetMaterial();
        }
        else
        {
            dissolveScript.gameObject.SetActive(true);

        }
    }

    public override void OnReloadCompleted()
    {
        AmmoRefresh();
        fieldReadyTimer = FieldRevTimer;
        dissolveScript.ResetMaterial();
        base.OnReloadCompleted();
    }


    private void CreateForceField()
    {
        var _newHit = GetHit(50f);
        Vector3 spawnForceFieldLoc = _newHit.point;
        EnemyScript enemyScript = null;

        if (_newHit.collider != null)
        {
            spawnForceFieldLoc += _newHit.normal * 1f;
            enemyScript = _newHit.collider.GetComponentInParent<EnemyScript>();
        }

        if (enemyScript != null)
        {
            spawnForceFieldLoc = enemyScript.OffsetedBoundWorldPosition;
        }

        {
            SpawnTracer(_newHit);
        }

        muzzle1.Play();
        var prefabChain = Instantiate(forceFielderPrefab, spawnForceFieldLoc, forceFielderPrefab.transform.rotation);
        gunRecoil.RecoilFire();
        cooldownFire = 1f / bulletPerSecond + 0.05f;
        curAmmo--;
        AmmoRefresh();

    }

    private void SpawnTracer(RaycastHit _newHit)
    {
        var points = new Vector3[2];
        GameObject trace = Instantiate(bulletTracer, bulletShooter.transform.position, Quaternion.identity);
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        points[0] = bulletShooter.transform.position;

        if (_newHit.collider == null)
        {
            points[1] = cam.ViewportToWorldPoint(new Vector3(.5f + spreadX, .5f + spreadY, 100f));
            trace.GetComponent<LineRenderer>().SetPositions(points);
            Destroy(trace, .03f);
        }
        else
        {
            points[1] = _newHit.point;
            trace.GetComponent<LineRenderer>().SetPositions(points);
            Destroy(trace, .03f);
        }
    }
}
