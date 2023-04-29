using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public class CustomVariable
{
    public enum Type
    {
        Init,
        FlatAdd,
        Override
    }

    public string variableName = "ParalyzeChance";
    public float value = 0.05f;
    public Type type = Type.Init;

    public CustomVariable(string variableName)
    {
        this.variableName = variableName;
    }
}

public abstract class BaseWeaponScript : MonoBehaviour
{

    public WeaponManager weaponSystem;
    public Animator anim;
    public string weaponName;
    public Image crosshairHit;


    [FoldoutGroup("Weapon Stat")] public List<string> allAttachments = new List<string>();
    [FoldoutGroup("Weapon Stat")] [ReadOnly] public List<CustomVariable> CustomVariables = new List<CustomVariable>();
    [FoldoutGroup("Weapon Stat")] public float damage;
    [FoldoutGroup("Weapon Stat")] public float variableAdditionalDamage = 4f;
    [FoldoutGroup("Weapon Stat")] public bool isAmmoUnlimited = false;
    [FoldoutGroup("Weapon Stat")] public bool isBurnBullet = false;
    [FoldoutGroup("Weapon Stat")] public bool isPoisonBullet = false;
    [FoldoutGroup("Weapon Stat")] public int totalAmmo; 
    [FoldoutGroup("Weapon Stat")] public int magazineSize;
    [FoldoutGroup("Weapon Stat")] public int curAmmo;
    [FoldoutGroup("Weapon Stat")] public int lowAmmoAlert = 2;
    [FoldoutGroup("Weapon Stat")] [Range(0f, .2f)] public float spread;
    [FoldoutGroup("Weapon Stat")] public float recoilX;
    [FoldoutGroup("Weapon Stat")] public float recoilY;
    [FoldoutGroup("Weapon Stat")] public float recoilZ;
    [FoldoutGroup("Weapon Stat")] public float recoilMultiplier = 1f;
    [FoldoutGroup("Weapon Stat")] public float bulletPerSecond; //AKA fire per second
    [FoldoutGroup("Weapon Stat")] public DamageToken.DamageType damageType = DamageToken.DamageType.Ballistic;
    [FoldoutGroup("Hint")] public bool UseTutorialHint = false;


    public virtual void FireWeapon()
    {

    }

    public virtual void FireInput()
    {

    }

    internal CustomVariable GetCustomVariable(string ID)
    {
        return CustomVariables.Find(x => x.variableName == ID);
    }

    protected internal float GetFinalValue(string ID)
    {
        var allCustomVariables = CustomVariables.FindAll(x => x.variableName == ID);
        float value = 0f;

        foreach(var variable in allCustomVariables)
        {
            value = variable.value;
        }

        return value;
    }
}
