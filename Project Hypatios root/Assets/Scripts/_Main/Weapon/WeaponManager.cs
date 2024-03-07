using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Player
{
    Aldrich,
    Elena
}



public class WeaponManager : MonoBehaviour
{



    public bool disableInput = false;
    public int selectedWeapon = 0;
    public Player playerMode = Player.Aldrich;
    public GameObject weaponHolder;
    public GameObject weaponNumToSwap;
    public GameObject weaponToSwap;
    public GameEvent event_currentSwitch;
    public GameEvent event_reloadWeapon;
    public bool IsOnMeleeAttack = false;
    public int previousWeapon;
    public GunScript currentGunHeld;
    public BaseWeaponScript currentWeaponHeld;
    public LayerMask defaultLayerMask;
    [Tooltip("Ban guns.")] public bool allowWeaponSelect = true; //ONLY FOR LEVELS THAT BAN GUNS

    Camera cam;

    [SerializeField] private List<BaseWeaponScript> currentlyHeldWeapons = new List<BaseWeaponScript>();

    public Recoil Recoil;

    public List<BaseWeaponScript> CurrentlyHeldWeapons { get => currentlyHeldWeapons;}

    public Animator anim
    {
        get
        {
            return currentWeaponHeld.anim;
        }
    }

    #region Weapon
    public BaseWeaponScript GetWeaponScript(string name)
    {
        return CurrentlyHeldWeapons.Find(x => x.weaponName == name);
    }


    public GunScript GetGunScript(string name)
    {
        return CurrentlyHeldWeapons.Find(x => x.weaponName == name) as GunScript;
    }

    public GunScript GetRandomGun()
    {
        List<BaseWeaponScript> validWeapons = new List<BaseWeaponScript>();
        validWeapons.AddRange(currentlyHeldWeapons);
        validWeapons.RemoveAll(x => x.isAmmoUnlimited == true);
        int size = validWeapons.Count;
        return validWeapons[Random.Range(0, size)] as GunScript;
    }


    [FoldoutGroup("Debug")]
    [Button("Debug-Consficate Weapon")]
    public void ConsficateAllWeapon()
    {
        int index1 = 0;
        var weaponToRemove = new List<BaseWeaponScript>();
        weaponToRemove.AddRange(currentlyHeldWeapons);

        foreach (var weapon1 in weaponToRemove)
        {
            if (index1 == 0)
            {
                index1++;
                continue;
            }

            Hypatios.Player.Weapon.RemoveWeapon(weapon1);
            index1++;
        } 
    }

    public void RefillAmmo(GunScript gunScript, int amount)
    {
        gunScript.totalAmmo += amount;
        soundManagerScript.instance.Play("reward");
        MainGameHUDScript.Instance.ShowAmmo(gunScript.weaponName, amount);
    }

    public BaseWeaponScript AddWeapon(string weaponID, bool shouldSwitch = true)
    {
        var weaponTarget = Hypatios.Assets.Weapons.Find(x => x.nameWeapon == weaponID);

        if (weaponTarget == null | GetGunScript(weaponID) != null)
        {
            Debug.LogError("Weapon already exist.");
            return null;
        }

        var gun = Instantiate(weaponTarget.prefab, transform);

        var weapon_ = gun.GetComponentInChildren<GunScript>();
        CurrentlyHeldWeapons.Add(weapon_);

        if (shouldSwitch)
        {
            selectedWeapon = CurrentlyHeldWeapons.Count - 1;
            switchWeapon();
        }

        MainGameHUDScript.Instance.ReloadAmmoIcons();
        SetWeaponSettings(weapon_);
        return weapon_;
    }

    [FoldoutGroup("Debug")]
    [Button("Remove Weapon")]
    public void RemoveWeapon(BaseWeaponScript targetWeapon)
    {
        var weaponDat = Hypatios.Game.currentWeaponStat.Find(x => x.weaponID == targetWeapon.weaponName);
        //weaponDat.removed = true;
        Hypatios.Game.currentWeaponStat.Remove(weaponDat);

        Destroy(targetWeapon.gameObject);
        if (targetWeapon.transform.parent != this.gameObject) Destroy(targetWeapon.transform.parent.gameObject);
        CurrentlyHeldWeapons.Remove(targetWeapon);
        MainGameHUDScript.Instance.ReloadAmmoIcons();
        selectedWeapon = 0;
        switchWeapon();
    }

    #endregion



    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        switchWeapon();

        MainGameHUDScript hudScript = MainGameHUDScript.Instance;

        WeaponItem weaponItem = Hypatios.Assets.Weapons.Find(x => x.nameWeapon == currentWeaponHeld.weaponName);
        hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];

        if (currentGunHeld != null)
        {
            SetWeaponSettings(currentGunHeld);
        }

    }

    public void LoadGame_InitializeGameSetup()
    {
        var weaponDatas = FPSMainScript.savedata.Game_WeaponStats;

        foreach(var weaponDat in weaponDatas)
        {
            var gun1 = GetWeaponScript(weaponDat.weaponID);

            if (weaponDat.removed)
            {
                continue;
            }    

            if (gun1 == null)
            {
                gun1 = AddWeapon(weaponDat.weaponID, false);
            }

            if (gun1 == null)
            {
                continue;
            }

            gun1.totalAmmo = weaponDat.totalAmmo;
            gun1.curAmmo = weaponDat.currentAmmo;

        }
    }

    public void RefreshWeaponLoadout(string onlyRefreshID)
    {
        var weaponDatas = Hypatios.Game.currentWeaponStat;

        foreach (var weaponDat in weaponDatas)
        {
            var gun1 = GetWeaponScript(weaponDat.weaponID);

            if (weaponDat.removed)
            {
                continue;
            }

            if (gun1 == null)
            {
                gun1 = AddWeapon(weaponDat.weaponID, false);
            }

            if (gun1 == null)
            {
                continue;
            }
            else if (gun1.weaponName == onlyRefreshID)
            {
                gun1.totalAmmo = weaponDat.totalAmmo;
                gun1.curAmmo = weaponDat.currentAmmo;
                //transfer ammos
                selectedWeapon = 0;
                switchWeapon();
            }

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

        if (disableInput == false) InputSwitchWeapon();

        if (previousWeapon != selectedWeapon)
        {
            if (currentGunHeld != null)
            {
                //currentGunHeld.isReloading = false;
                //currentGunHeld.curReloadTime = currentGunHeld.ReloadTime;
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
        if (Time.timeScale == 0)
        {
            return;
        }

        if (allowWeaponSelect == false)
        {
            return;
        }


        previousWeapon = selectedWeapon;

        //IsOnMeleeAttack = meleeWeapon.IsAnimationPlaying();

        var mouseVector = Hypatios.Input.SwitchWeapon.ReadValue<float>();
        bool isInteractContainer = InteractableCamera.instance.IsInteractContainer();


        if (mouseVector > 0f && Hypatios.Input.SwitchWeapon.triggered && !isInteractContainer)
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
        if (mouseVector < 0f && Hypatios.Input.SwitchWeapon.triggered && !isInteractContainer)
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
            var gunScript = weapon as GunScript;


            if (i == selectedWeapon)
            {
                MainGameHUDScript hudScript = MainGameHUDScript.Instance;
                WeaponItem weaponItem = Hypatios.Assets.Weapons.Find(x => x.nameWeapon == weapon.weaponName);

                hudScript.weaponUI.sprite = weaponItem.weaponIcon; //hudScript.weaponSprite[i];
                hudScript.SwapCrosshair(weaponItem.overrideCrosshair_Sprite);
                weapon.gameObject.SetActive(true);
                weapon.anim = weapon.GetComponent<Animator>();
                weapon.crosshairHit = hudScript.crosshairHit;


                currentWeaponHeld = weapon;

                if (gunScript != null)
                {
                    currentGunHeld = gunScript;
                    SetWeaponSettings(currentGunHeld);
                }

                TransferAllInventoryAmmoToCurrent();
            }
            else
            {

         


                weapon.gameObject.SetActive(false);
            }
            i++;
        }
        event_currentSwitch.Raise();

    }

    private IEnumerator CrosshairCoroutine;

    public void ActivateCrosshairHit()
    {
        if (CrosshairCoroutine !=null)
        {
            StopCoroutine(SetCrosshairHitActive());
        }
        CrosshairCoroutine = SetCrosshairHitActive();
        StartCoroutine(CrosshairCoroutine);
    }

    private IEnumerator SetCrosshairHitActive()
    {
        MainGameHUDScript hudScript = MainGameHUDScript.Instance;
        hudScript.crosshairHit.gameObject.SetActive(false);
        hudScript.crosshairHit.gameObject.SetActive(true);
        MainGameHUDScript.Instance.audio_CrosshairClick.Play();
        yield return new WaitForSeconds(.2f);
        hudScript.crosshairHit.gameObject.SetActive(false);
    }

    private void TransferAllInventoryAmmoToCurrent()
    {
        foreach(var itemDat in Hypatios.Player.Inventory.allItemDatas)
        {
            var weaponClass = Hypatios.Assets.GetItem(itemDat.ID).attachedWeapon;
            if (itemDat.weaponData == null) continue;
            if (currentGunHeld.weaponName == weaponClass.nameWeapon && weaponClass.nameWeapon != "")
            {
                currentWeaponHeld.totalAmmo += itemDat.weaponData.totalAmmo;
                itemDat.weaponData.totalAmmo = 0;
                continue;
            }

        }
    }

    public void TransferAllInventoryAmmoToOneItemData(ref HypatiosSave.ItemDataSave currentItemDat)
    {
        foreach (var itemDat in Hypatios.Player.Inventory.allItemDatas)
        {
            var weaponClass = Hypatios.Assets.GetItem(itemDat.ID).attachedWeapon;
            if (itemDat.weaponData == null) continue;
            if (currentItemDat == itemDat) continue;
            if (currentItemDat.weaponData.weaponID == itemDat.ID)
            {
                currentItemDat.weaponData.totalAmmo += itemDat.weaponData.totalAmmo;
                itemDat.weaponData.totalAmmo = 0;
                continue;
            }

        }
    }

    public void TransferAmmo_PrepareDelete(HypatiosSave.ItemDataSave weaponDataToBeDeleted)
    {
        HypatiosSave.ItemDataSave similarWeaponType_ItemData = null;
        foreach (var itemDat in Hypatios.Player.Inventory.allItemDatas)
        {
            var weaponClass = Hypatios.Assets.GetItem(itemDat.ID).attachedWeapon;
            if (itemDat.weaponData == null) continue;
            if (weaponDataToBeDeleted == itemDat) continue;
            if (weaponDataToBeDeleted.weaponData.weaponID == itemDat.ID)
            {
                similarWeaponType_ItemData = itemDat;
                break;
            }

        }

        if (similarWeaponType_ItemData != null)
        {
            similarWeaponType_ItemData.weaponData.totalAmmo += weaponDataToBeDeleted.weaponData.totalAmmo;
        }
    }

    public int GetTotalAmmoOfWeapon(string weaponID)
    {
        int total = 0;

        foreach (var itemDat in Hypatios.Player.Inventory.allItemDatas)
        {
            var weaponClass = Hypatios.Assets.GetItem(itemDat.ID).attachedWeapon;
            if (itemDat.weaponData == null) continue;
            if (weaponID == weaponClass.nameWeapon && weaponClass.nameWeapon != "")
            {
                total += itemDat.weaponData.totalAmmo;
            }
        }

        //also currently equipped
        foreach(var currentWeapon in currentlyHeldWeapons)
        {
            if (currentWeapon.weaponName == weaponID)
                total += currentWeapon.totalAmmo;
        }

        return total;
    }



    public void SetWeaponSettings(BaseWeaponScript weaponScript)
    {
        Hypatios.Game.NewWeaponStat(weaponScript);
        var weaponsave = Hypatios.Game.GetWeaponSave(weaponScript.weaponName);

        var weapon1 = Hypatios.Assets.GetWeapon(weaponScript.weaponName);
        weaponsave.removed = false;

        if (weaponsave == null)
        {
            weaponScript.damage = weapon1.defaultDamage;
            weaponScript.magazineSize = weapon1.defaultMagazineSize;
            return;
        }

        var weaponStat = weapon1.GetFinalStat(weaponsave.allAttachments);

        weaponScript.damage = weaponStat.damage;
        weaponScript.bulletPerSecond = weaponStat.cooldown;
        weaponScript.magazineSize = weaponStat.magazineSize;
        weaponScript.allAttachments = weaponsave.allAttachments;
        weaponScript.recoilMultiplier = weaponStat.recoilMultiplier;
        weaponScript.isBurnBullet = weaponStat.isBurn;
        weaponScript.isPoisonBullet = weaponStat.isPoison;
        weaponScript.CustomVariables = weaponStat.customVariable;

        var attachmentVisuals = GetComponentsInChildren<WeaponAttachmentVisuals>();

        foreach (var attach in attachmentVisuals)
        {
            attach.visual.gameObject.SetActive(false);
        }

        int order = 0;

        foreach (var attach in attachmentVisuals)
        {
            var attachModID = weaponScript.allAttachments.Find(x => x == attach.ID);
            bool exists = attachModID != null ? true : false;

            var att11 = weapon1.GetAttachmentWeaponMod(attachModID); //i dont know what to name this variable
            if (att11 != null)
            {
                if (order > att11.order)
                    continue;
            }

            if (exists)
            {
                attach.RefreshVisuals(attachModID);
                attach.TriggerRequirements(true);
            }
            else
            {
                attach.TriggerRequirements(false);
            }
        }
    }

    void findEquippedWeapon()
    {
        weaponNumToSwap = weaponHolder.transform.GetChild(selectedWeapon).gameObject;
        weaponToSwap = weaponNumToSwap.transform.GetChild(0).gameObject;
    }
}
