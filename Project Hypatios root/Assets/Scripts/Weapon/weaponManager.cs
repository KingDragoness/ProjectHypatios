using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FPSGame;

public class weaponManager : MonoBehaviour
{

    public int selectedWeapon = 0;
    public GameObject weaponHolder;
    public GameObject weaponNumToSwap;
    public GameObject weaponToSwap;
    public ModularWeapon meleeWeapon;
    public Animator anim;
    public bool IsOnMeleeAttack = false;
    public int previousWeapon;
    public gunScript gun;
    Camera cam;

    public List<WeaponItem> weapons = new List<WeaponItem>();
    private List<gunScript> currentlyHeldGuns = new List<gunScript>();

    public static weaponManager Instance;

    public Recoil gunRecoil;

    public List<gunScript> CurrentlyHeldGuns { get => currentlyHeldGuns;}

    #region Weapon

    public gunScript GetGunScript(string name)
    {
        return CurrentlyHeldGuns.Find(x => x.weaponName == name);
    }

    public gunScript GetRandomGun()
    {
        int size = CurrentlyHeldGuns.Count - 1;
        return CurrentlyHeldGuns[Random.Range(0, size)];
    }

    public WeaponItem GetWeaponItemData(gunScript gun)
    {
        return weapons.Find(x => x.nameWeapon == gun.weaponName);
    }

    public WeaponItem GetWeaponItemData(string s)
    {
        return weapons.Find(x => x.nameWeapon == s);
    }

    public void RefillAmmo(gunScript gunScript, int amount)
    {
        gunScript.totalAmmo += amount;
        soundManagerScript.instance.Play("reward");
        MainGameHUDScript.Instance.ShowAmmo(gunScript.weaponName, amount);
    }


    #endregion



    private void Awake()
    {
        Instance = this;
    }

    public void LoadGame_InitializeGameSetup()
    {
        currentlyHeldGuns = this.gameObject.GetComponentsInChildren<gunScript>(true).ToList();
        var weaponDatas = FPSMainScript.savedata.Game_WeaponStats;

        foreach(var weaponDat in weaponDatas)
        {
            var gun1 = GetGunScript(weaponDat.weaponID);

            if (weaponDat.removed)
            {
                continue;
            }    

            if (gun1 == null)
            {
                gun1 = AddWeapon(weaponDat.weaponID, false);
            }

            gun1.totalAmmo = weaponDat.totalAmmo;
            gun1.curAmmo = weaponDat.currentAmmo;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentlyHeldGuns = this.gameObject.GetComponentsInChildren<gunScript>(true).ToList();
        switchWeapon();

        WeaponItem weaponItem = weapons.Find(x => x.nameWeapon == gun.weaponName);
        MainGameHUDScript hudScript = MainGameHUDScript.Instance;

        hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];

        SetWeaponSettings(gun);


    }

    // Update is called once per frame
    void Update()
    {
        if (gun == null && CurrentlyHeldGuns.Count <= 0)
        {
            return;
        }

        MainGameHUDScript.Instance.currentAmmo.text = "" + gun.curAmmo;
        MainGameHUDScript.Instance.maximumAmmo.text = "" + gun.totalAmmo;
        previousWeapon = selectedWeapon;

        //IsOnMeleeAttack = meleeWeapon.IsAnimationPlaying();


        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
            
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }

        if (previousWeapon != selectedWeapon)
        {
            gun.isReloading = false;
            gun.curReloadTime = gun.reloadTime;
            gun.isScoping = false;
            
            switchWeapon();
        }

        findEquippedWeapon();

        
    }

    public void unequipWeapon()
    {
        foreach (var weapon in CurrentlyHeldGuns)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    public void switchWeapon()
    {
        int i = 0;

        foreach (var weapon in CurrentlyHeldGuns)
        {
            if (i == selectedWeapon)
            {
                MainGameHUDScript hudScript = MainGameHUDScript.Instance;
                WeaponItem weaponItem = weapons.Find(x => x.nameWeapon == weapon.weaponName);

                hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];
                hudScript.SwapCrosshair(weaponItem.overrideCrosshair_Sprite);
                weapon.gameObject.SetActive(true);
                anim = weapon.GetComponent<Animator>();
                weapon.crosshairHit = hudScript.crosshairHit;

                gun = weapon;
                SetWeaponSettings(gun);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
        
    }


    public gunScript AddWeapon(string weaponID, bool shouldSwitch = true)
    {
        var weaponTarget = weapons.Find(x => x.nameWeapon == weaponID);

        var gun = Instantiate(weaponTarget.prefab, transform);
        selectedWeapon = transform.childCount - 1;

        var weapon_ = gun.GetComponentInChildren<gunScript>();
        CurrentlyHeldGuns.Add(weapon_);
        
        if (shouldSwitch) switchWeapon();

        SetWeaponSettings(weapon_);
        return weapon_;
    }

    public void SetWeaponSettings(gunScript gunScript)
    {
        FPSMainScript.instance.NewWeaponStat(gunScript);
        var weaponStat = FPSMainScript.instance.GetWeaponSave(gunScript.weaponName);

        var weapon1 = GetWeaponItemData(gunScript);
        weaponStat.removed = false;

        if (weaponStat == null)
        {
            gunScript.damage = weapon1.defaultDamage;
            gunScript.magazineSize = weapon1.defaultMagazineSize;
            return;
        }

        gunScript.damage = weapon1.levels_Damage[weaponStat.level_Damage];
        gunScript.magazineSize = weapon1.levels_MagazineSize[weaponStat.level_MagazineSize];
        gunScript.bulletPerSecond = weapon1.levels_Cooldown[weaponStat.level_Cooldown];
    }

    void findEquippedWeapon()
    {
        weaponNumToSwap = weaponHolder.transform.GetChild(selectedWeapon).gameObject;
        weaponToSwap = weaponNumToSwap.transform.GetChild(0).gameObject;
    }
}
