using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FPSGame;

public class WeaponManager : MonoBehaviour
{

    public int selectedWeapon = 0;
    public GameObject weaponHolder;
    public GameObject weaponNumToSwap;
    public GameObject weaponToSwap;
    public ModularWeapon meleeWeapon;
    public bool IsOnMeleeAttack = false;
    public int previousWeapon;
    public GunScript currentGunHeld;
    public BaseWeaponScript currentWeaponHeld;
    Camera cam;

    public List<WeaponItem> weapons = new List<WeaponItem>();
    [SerializeField] private List<BaseWeaponScript> currentlyHeldWeapons = new List<BaseWeaponScript>();

    public static WeaponManager Instance;

    public Recoil gunRecoil;

    public List<BaseWeaponScript> CurrentlyHeldWeapons { get => currentlyHeldWeapons;}

    public Animator anim
    {
        get
        {
            return currentWeaponHeld.anim;
        }
    }

    #region Weapon

    public GunScript GetGunScript(string name)
    {
        return CurrentlyHeldWeapons.Find(x => x.weaponName == name) as GunScript;
    }

    public GunScript GetRandomGun()
    {
        int size = CurrentlyHeldWeapons.Count - 1;
        return CurrentlyHeldWeapons[Random.Range(0, size)] as GunScript;
    }

    public WeaponItem GetWeaponItemData(GunScript gun)
    {
        return weapons.Find(x => x.nameWeapon == gun.weaponName);
    }

    public WeaponItem GetWeaponItemData(string s)
    {
        return weapons.Find(x => x.nameWeapon == s);
    }

    public void RefillAmmo(GunScript gunScript, int amount)
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

    // Start is called before the first frame update
    void Start()
    {
        switchWeapon();

        WeaponItem weaponItem = weapons.Find(x => x.nameWeapon == currentGunHeld.weaponName);
        MainGameHUDScript hudScript = MainGameHUDScript.Instance;

        hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];

        SetWeaponSettings(currentGunHeld);


    }

    public void LoadGame_InitializeGameSetup()
    {
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


    #region Updates

    // Update is called once per frame
    void Update()
    {
        if (currentWeaponHeld == null && CurrentlyHeldWeapons.Count <= 0)
        {
            return;
        }

        WeaponUI();

        InputSwitchWeapon();

        if (previousWeapon != selectedWeapon)
        {
            if (currentGunHeld != null)
            {
                currentGunHeld.isReloading = false;
                currentGunHeld.curReloadTime = currentGunHeld.reloadTime;
                currentGunHeld.isScoping = false;
            }

            switchWeapon();
        }

        //-1 is melee
        if (selectedWeapon != -1) findEquippedWeapon();    
    }

    private void WeaponUI()
    {

        if (currentGunHeld != null && currentGunHeld == currentWeaponHeld)
        {
            MainGameHUDScript.Instance.currentAmmo.text = "" + currentGunHeld.curAmmo;
            MainGameHUDScript.Instance.maximumAmmo.text = "" + currentGunHeld.totalAmmo;
        }

        if (currentWeaponHeld.isAmmoUnlimited)
        {
            MainGameHUDScript.Instance.currentAmmo.text = "∞";
            MainGameHUDScript.Instance.maximumAmmo.text = "∞";
        }
    }

    private void InputSwitchWeapon()
    {
        previousWeapon = selectedWeapon;

        //IsOnMeleeAttack = meleeWeapon.IsAnimationPlaying();


        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= CurrentlyHeldWeapons.Count - 1)
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
                selectedWeapon = CurrentlyHeldWeapons.Count - 1;
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
        else if (Input.GetKeyDown(KeyCode.Alpha2) && CurrentlyHeldWeapons.Count >= 2)
        {
            selectedWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && CurrentlyHeldWeapons.Count >= 3)
        {
            selectedWeapon = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && CurrentlyHeldWeapons.Count >= 4)
        {
            selectedWeapon = 3;
        }
    }

#endregion

    public void unequipWeapon()
    {
        foreach (var weapon in CurrentlyHeldWeapons)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    public void switchWeapon()
    {
        int i = 0;

        currentlyHeldWeapons.RemoveAll(x => x == null);

        foreach (var weapon in CurrentlyHeldWeapons)
        {
            if (i == selectedWeapon)
            {
                MainGameHUDScript hudScript = MainGameHUDScript.Instance;
                WeaponItem weaponItem = weapons.Find(x => x.nameWeapon == weapon.weaponName);

                hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];
                hudScript.SwapCrosshair(weaponItem.overrideCrosshair_Sprite);
                weapon.gameObject.SetActive(true);
                weapon.anim = weapon.GetComponent<Animator>();
                weapon.crosshairHit = hudScript.crosshairHit;

                var gunScript = weapon as GunScript;

                currentWeaponHeld = weapon;

                if (gunScript != null)
                {
                    currentGunHeld = gunScript;
                    SetWeaponSettings(currentGunHeld);
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
        
    }


    public GunScript AddWeapon(string weaponID, bool shouldSwitch = true)
    {
        var weaponTarget = weapons.Find(x => x.nameWeapon == weaponID);

        var gun = Instantiate(weaponTarget.prefab, transform);

        var weapon_ = gun.GetComponentInChildren<GunScript>();
        CurrentlyHeldWeapons.Add(weapon_);
        selectedWeapon = CurrentlyHeldWeapons.Count - 1;

        if (shouldSwitch) switchWeapon();

        SetWeaponSettings(weapon_);
        return weapon_;
    }

    public void SetWeaponSettings(GunScript gunScript)
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
