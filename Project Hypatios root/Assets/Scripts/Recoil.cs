using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class Recoil : MonoBehaviour
{

    public enum RecoilType
    {
        MovementLand,
        TakeDamage,
        FireWeapon
    }

    private Vector3 curRot;
    private Vector3 targetRot;
    public CharacterStat knockbackResistance;

    [SerializeField]
    private float snappiness;

    [SerializeField]
    private float returnSpeed;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public BaseWeaponScript weapon;
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
        weapon = weaponSystem.currentWeaponHeld;

        if (weapon != null)
        {
            recoilX = weapon.recoilX;
            recoilY = weapon.recoilY;
            recoilZ = weapon.recoilZ;
        }
    }

    public void RecoilFire()
    {
        var recoilRange = new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        recoilRange *= knockbackResistance.Value;
        targetRot += recoilRange;
    }

    public void CustomRecoil(Vector3 rot, float multiplier = 1, RecoilType type = RecoilType.MovementLand)
    {
        var recoilRange = new Vector3(rot.x, Random.Range(-rot.y, rot.y), Random.Range(-rot.z, rot.z)) * multiplier;
        if (type == RecoilType.TakeDamage)
        {
            recoilRange *= knockbackResistance.Value;
        }
        targetRot += recoilRange;
    }

}
