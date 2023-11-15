using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShopScript : MonoBehaviour
{

    public Transform movePlayerHere;
    public WeaponModelDisplay weaponBuyable_1;
    public WeaponModelDisplay weaponBuyable_2;
    public WeaponModelDisplay weaponBuyable_3;
    public Inventory storage;
    public LootTable lootTable;
    public int currentHighlight = 0; //-1 
    public GameObject previewNormalView;
    public GameObject previewCam1;
    public GameObject previewCam2;
    public GameObject previewCam3;

    private bool isShopOpened = false;
    public static int ShopScriptIndex = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        ShopScriptIndex = 0;
    }


    private void Start()
    {
        ShopScriptIndex++;
        OnTriggerEnterEvent enterEvent = GetComponent<OnTriggerEnterEvent>();
        enterEvent.objectToCompare = Hypatios.Player.gameObject;
        ResetShop();
    }

    public void ResetShop()
    {
        GenerateBuyableWeapons();
        RefreshWeaponModels();
    }

    private void GenerateBuyableWeapons()
    {
        storage.allItemDatas.Clear();
        int itemSellCount = 3;

        if (Hypatios.Game.TotalRuns <= 10)
            itemSellCount = 1;
        else if (Hypatios.Game.TotalRuns <= 20)
            itemSellCount = 2;

        for (int x = 0; x < itemSellCount; x++)
        {
            bool valid = false;
            int count = 0;

            while (valid == false)
            {
                var weapon1 = lootTable.GetEntry(Application.loadedLevel + ShopScriptIndex + count + Hypatios.StaticGauntletSeed()).item;
                if (IsDuplicate(weapon1.attachedWeapon) == false)
                {
                    storage.AddItem(weapon1);
                    RefreshAssignWeapon();
                    valid = true;
                }
                if (count > 999) break;
                count++;
            }
        }
    }


    private void RefreshAssignWeapon()
    {

        int index = 0;
        weaponBuyable_1.currentWeaponDisplay = null;
        weaponBuyable_2.currentWeaponDisplay = null;
        weaponBuyable_3.currentWeaponDisplay = null;

        for (int x = 0; x < storage.allItemDatas.Count; x++)
        {
            index = x;
            var weaponItem = Hypatios.Assets.GetWeapon(storage.allItemDatas[x].weaponData.weaponID);

            if (x == 0)
            {
                weaponBuyable_1.currentWeaponDisplay = weaponItem;
            }
            if (x == 1)
            {
                weaponBuyable_2.currentWeaponDisplay = weaponItem;
            }
            if (x == 2)
            {
                weaponBuyable_3.currentWeaponDisplay = weaponItem;

            }
        }
    }

    public bool IsDuplicate(WeaponItem weaponItem)
    {
        if (weaponBuyable_1.currentWeaponDisplay == weaponItem)
            return true;
        if (weaponBuyable_2.currentWeaponDisplay == weaponItem)
            return true;
        if (weaponBuyable_3.currentWeaponDisplay == weaponItem)
            return true;

        return false;
    }

    public void RefreshWeaponModels()
    {
        RefreshAssignWeapon();
        if (weaponBuyable_1.currentWeaponDisplay == null)
        {
            weaponBuyable_1.gameObject.SetActive(false);
        }
        else
        {
            weaponBuyable_1.gameObject.SetActive(true);
            weaponBuyable_1.ActivateWeapon(weaponBuyable_1.displays.Find(x => x.weaponItem == weaponBuyable_1.currentWeaponDisplay));
        }

        if (weaponBuyable_2.currentWeaponDisplay == null)
        {
            weaponBuyable_2.gameObject.SetActive(false);
        }
        else
        {
            weaponBuyable_2.gameObject.SetActive(true);
            weaponBuyable_2.ActivateWeapon(weaponBuyable_2.displays.Find(x => x.weaponItem == weaponBuyable_2.currentWeaponDisplay));
        }

        if (weaponBuyable_3.currentWeaponDisplay == null)
        {
            weaponBuyable_3.gameObject.SetActive(false);
        }
        else
        {
            weaponBuyable_3.gameObject.SetActive(true);
            weaponBuyable_3.ActivateWeapon(weaponBuyable_3.displays.Find(x => x.weaponItem == weaponBuyable_3.currentWeaponDisplay));
        }
    }

    public void OpenShop()
    {
        var weaponUI = MainGameHUDScript.Instance.chargeStationUI;
        isShopOpened = true;
        Hypatios.Player.transform.position = movePlayerHere.transform.position;
        weaponUI.SetShopScript(this);
        MainUI.Instance.ChangeCurrentMode(MainUI.UIMode.Weapon);
    }

    private void Update()
    {
        if (isShopOpened)
        {
            if (currentHighlight == 0)
            {
                previewCam1.gameObject.SetActive(true);
                previewCam2.gameObject.SetActive(false);
                previewCam3.gameObject.SetActive(false);
                previewNormalView.gameObject.SetActive(false);

            }
            else if (currentHighlight == 1)
            {
                previewCam1.gameObject.SetActive(false);
                previewCam2.gameObject.SetActive(true);
                previewCam3.gameObject.SetActive(false);
                previewNormalView.gameObject.SetActive(false);

            }
            else if (currentHighlight == 2)
            {
                previewCam1.gameObject.SetActive(false);
                previewCam2.gameObject.SetActive(false);
                previewCam3.gameObject.SetActive(true);
                previewNormalView.gameObject.SetActive(false);

            }
            else
            {
                previewCam1.gameObject.SetActive(false);
                previewCam2.gameObject.SetActive(false);
                previewCam3.gameObject.SetActive(false);
                previewNormalView.gameObject.SetActive(true);
            }

            if (Hypatios.UI.CurrentUI != MainUI.UIMode.Weapon)
            {
                CloseShop();
            }
        }
    }

    public void CloseShop()
    {
        isShopOpened = false;
        previewCam1.gameObject.SetActive(false);
        previewCam2.gameObject.SetActive(false);
        previewCam3.gameObject.SetActive(false);
        previewNormalView.gameObject.SetActive(false);

    }
}
