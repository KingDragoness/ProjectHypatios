using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeWeaponType
{
    Damage,
    MagazineSize,
    Cooldown
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponItem : ScriptableObject
{
    
    public enum Category
    {
        Melee = -1,
        Pistol = 0,
        Shotgun,
        SMG,
        Rifle,
        Katana = 1000,
        ThrowingKnife
    }

    [System.Serializable]
    public class Upgradeable
    {
        //effects
        public string name = "SuperiorReceiver";
    }

    public string nameWeapon = "Pistol";
    public GameObject prefab;
    public Sprite weaponIcon;
    public Sprite overrideCrosshair_Sprite;
    public List<int> levels_Damage;
    public List<int> levels_MagazineSize;
    public List<float> levels_Cooldown;

    public int defaultDamage;
    public int defaultMagazineSize;
    public float defaultCooldown;

    //t: 0 - 1
    //value: 0 - anything beyond 0
    public AnimationCurve rewardRate;

}
