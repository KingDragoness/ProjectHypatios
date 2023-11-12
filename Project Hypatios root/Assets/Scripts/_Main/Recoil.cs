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

    public Vector3 playerKnockPhysics = new Vector3(0,0,-10);
    public float hurtKnockMultiplier = 2f;
    private Vector3 curRot;
    private Vector3 targetRot;
    public CharacterStat knockbackResistance;
    public CharacterStat baseRecoil;

    [SerializeField]
    private float snappiness;

    [SerializeField]
    private float returnSpeed;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public BaseWeaponScript weapon;
    public WeaponManager weaponSystem;

    public Vector3 TargetRot { get => targetRot; set => targetRot = value; }

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
            recoilX = weapon.recoilX * weapon.recoilMultiplier;
            recoilY = weapon.recoilY * weapon.recoilMultiplier;
            recoilZ = weapon.recoilZ * weapon.recoilMultiplier;
        }
    }

    private float FinalValue(float value)
    {
        if (value <= 0)
            return 0;

        return value;
    }

    public void RecoilFire()
    {
        var recoilRange = new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        var magnitude = recoilRange.magnitude * (1f / Hypatios.ExtraAttackSpeedModifier());
        recoilRange *= FinalValue(baseRecoil.Value);
        targetRot += recoilRange;
       
        Hypatios.Player.rb.AddRelativeForce(FinalValue(knockbackResistance.Value) * NoGravityMultiplier * playerKnockPhysics * magnitude);
    }

    public void AddCustomKnockbackForce(Vector3 dir, float multiplier)
    {
        Hypatios.Player.rb.AddRelativeForce(FinalValue(knockbackResistance.Value) * dir * NoGravityMultiplier * multiplier * 60f);
    }

    public void CustomRecoil(Vector3 rot, float multiplier = 1, RecoilType type = RecoilType.MovementLand)
    {
        var recoilRange = new Vector3(rot.x, Random.Range(-rot.y, rot.y), Random.Range(-rot.z, rot.z)) * multiplier;
        var magnitude = recoilRange.magnitude;

        if (type == RecoilType.TakeDamage)
        {
            recoilRange *= FinalValue(knockbackResistance.Value);
            Hypatios.Player.rb.AddRelativeForce(FinalValue(knockbackResistance.Value) * playerKnockPhysics * NoGravityMultiplier * magnitude * hurtKnockMultiplier);
        }
        targetRot += recoilRange;
    }

    public float NoGravityMultiplier
    {
        get
        {
            if (Hypatios.Player.isNoGravity)
            {
                return 3.5f;
            }

            return 1f;
        }
    }

}
