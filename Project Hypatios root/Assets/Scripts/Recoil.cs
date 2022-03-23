using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{

    private Vector3 curRot;
    private Vector3 targetRot;

    [SerializeField]
    private float snappiness;

    [SerializeField]
    private float returnSpeed;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public GunScript gun;
    public WeaponManager weaponSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponSystem == null)
        {
            return;
        }

        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        curRot = Vector3.Slerp(curRot, targetRot, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(curRot);
        gun = weaponSystem.currentGunHeld;

        if (gun != null)
        {
            recoilX = gun.recoilX;
            recoilY = gun.recoilY;
            recoilZ = gun.recoilZ;
        }
    }

    public void RecoilFire()
    {
        targetRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

}
