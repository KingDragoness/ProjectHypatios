using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public abstract class BaseWeaponScript : MonoBehaviour
{

    public WeaponManager weaponSystem;
    public Animator anim;
    public LayerMask layerMask;
    public string weaponName;
    public Image crosshairHit;

    [Header("Weapon Status")]
    [FoldoutGroup("Weapon Stat")] public float damage;
    [FoldoutGroup("Weapon Stat")] public float variableAdditionalDamage = 4f;
    [FoldoutGroup("Weapon Stat")] public bool isAmmoUnlimited = false;
    [FoldoutGroup("Weapon Stat")] public int totalAmmo; 
    [FoldoutGroup("Weapon Stat")] public int magazineSize;
    [FoldoutGroup("Weapon Stat")] public int curAmmo;
    [FoldoutGroup("Weapon Stat")] [Range(0f, .2f)] public float spread;
    [FoldoutGroup("Weapon Stat")] public float recoilX;
    [FoldoutGroup("Weapon Stat")] public float recoilY;
    [FoldoutGroup("Weapon Stat")] public float recoilZ;

    public virtual void FireWeapon()
    {

    }

    public virtual void FireInput()
    {

    }
}
