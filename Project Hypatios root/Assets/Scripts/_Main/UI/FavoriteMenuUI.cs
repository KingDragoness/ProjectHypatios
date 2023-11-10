using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class FavoriteMenuUI : MonoBehaviour
{

    public enum Mode
    {
        SelectCategory,
        SelectItems
    }

    public Image cursorImage;
    public Mode currentMode;
    public List<FavoriteRadialCategoryButton> radialButtons = new List<FavoriteRadialCategoryButton>();
    public FavoriteRadialCategoryButton currentRadial;
    public GameObject favoriteListUI;
    public GameObject itemStatUI;
    public CanvasGroup cg;
    [FoldoutGroup("Item List")] public Text title;
    [FoldoutGroup("Item List")] public Image itemIcon;
    [FoldoutGroup("Item List")] public Transform parentItems;
    [FoldoutGroup("Item List")] public FavItemButton prefabItemButton;
    [FoldoutGroup("Item List")] public Text itemStat_Title;
    [FoldoutGroup("Item List")] public Text itemStat_Description;
    [FoldoutGroup("Item List")] public float TimeToConsumePress = 2f;

    [FoldoutGroup("HP", true)] public Slider healthRestoreBar;
    [FoldoutGroup("HP", true)] public Slider healthRestore_BorderBar;
    [ReadOnly] public float currentAngle = 0;

    private Vector3 mousePos;
    private Vector3 middlePos;
    private ItemInventory.SubiconCategory _currentSubCategory;
    private List<FavItemButton> allFavButtons = new List<FavItemButton>();
    private FavItemButton _currentFavItemButton;

    private float _timeSlider = 0f;
    private float _timePressConsume = 0f;
    private ItemInventory prevItemClass;

    private void OnEnable()
    {
        ResetUI();
        HideHealthRestore();

    }

    private void ResetUI()
    {
        ChangeMode(Mode.SelectCategory);
        favoriteListUI.gameObject.SetActive(false);
        itemStatUI.gameObject.SetActive(false);
        cursorImage.gameObject.SetActive(true);
        cg.alpha = 1f;

        itemStat_Title.text = "";
        itemStat_Description.text = "";
    }

    public void ChangeMode(Mode _mode)
    {
        if (currentMode != _mode)
        {
            if (_mode == Mode.SelectCategory)
            {
                cursorImage.gameObject.SetActive(true);
                favoriteListUI.gameObject.SetActive(false);
                cg.alpha = 1f;
            }
            else if (_mode == Mode.SelectItems)
            {
                cursorImage.gameObject.SetActive(false);
                favoriteListUI.gameObject.SetActive(true);
                cg.alpha = 0.4f;
            }
        }
        _currentFavItemButton = null;
        currentMode = _mode;
    }

    private void Update()
    {
        if (currentMode == Mode.SelectCategory)
        {
            mousePos = Input.mousePosition;
            middlePos = new Vector3(Screen.width / 2f, Screen.height / 2f);
            Vector3 up = Vector3.up;
            Vector3 dir = mousePos - middlePos;

            currentAngle = Vector3.SignedAngle(up, dir, Vector3.forward);

            UpdateRadialCursor();
        }
        else if (currentMode == Mode.SelectItems)
        {
            HandlePreview();
            HandleConsume();
        }

        if (Hypatios.Input.Fire2.triggered)
        {
            ChangeMode(Mode.SelectCategory);
        }
    }

    private void HandlePreview()
    {
        if (_currentFavItemButton == null) return;
        if (healthRestoreBar.gameObject.activeInHierarchy == false) return;
        var itemClass = _currentFavItemButton.GetItemInventory();

        if (itemClass == null) return;
        if (itemClass.category != ItemInventory.Category.Consumables) return;
        if (prevItemClass != itemClass) _timeSlider = 0f;

        float healSpeed = itemClass.consume_HealAmount / itemClass.consume_HealTime;
        float targetHeal = Hypatios.Player.Health.targetHealth + _timeSlider;

        _timeSlider += Time.unscaledDeltaTime * healSpeed * (Hypatios.Player.Health.digestion.Value);

        if (_timeSlider > itemClass.consume_HealAmount)
        {
            _timeSlider = 0;
        }

        if (Hypatios.Player.Health.targetHealth + _timeSlider > Hypatios.Player.Health.maxHealth.Value)
        {
            _timeSlider = 0f;
        }

        healthRestoreBar.maxValue = Hypatios.Player.Health.maxHealth.Value;
        healthRestoreBar.value = targetHeal;
        healthRestore_BorderBar.maxValue = Hypatios.Player.Health.maxHealth.Value;
        healthRestore_BorderBar.value = Hypatios.Player.Health.targetHealth + itemClass.consume_HealAmount;

        prevItemClass = itemClass;

    }

    private void HandleConsume()
    {
        {
            bool isFailed = false;

            if (_currentFavItemButton == null)
            {
                _timePressConsume = 0f;
                return;
            }

            if (Hypatios.Input.Fire1.IsPressed() == false) isFailed = true;

            var itemDat = _currentFavItemButton.GetItemData();

            if (itemDat.category != ItemInventory.Category.Consumables)
            {
                isFailed = true;
            }

            if (isFailed)
            {
                _timePressConsume = 0f;
                return;
            }
        }

        _timePressConsume += Time.unscaledDeltaTime;
        _currentFavItemButton.consumeProgress_slider.value = (_timePressConsume / TimeToConsumePress);

        if (_timePressConsume >= TimeToConsumePress)
        {
            UseItem(_currentFavItemButton);
            _timePressConsume = 0;
        }
    }

    private void UpdateSelectItem()
    {

    }

    private void UpdateRadialCursor()
    {
        currentRadial = null;
        float pieAngle = 360f * cursorImage.fillAmount;
        float offset = pieAngle / 2f;
        Vector3 rot = new Vector3(0f, 0f, currentAngle + offset);
        cursorImage.transform.localEulerAngles = rot;

        foreach(var button in radialButtons)
        {
            button.UpdateButton(currentAngle);
            if (button.IsSelected(currentAngle) && currentRadial == null)
            {
                currentRadial = button;
            }
        }

        if (Hypatios.Input.Fire1.triggered && currentRadial != null)
        {
            OpenUIFavorite(currentRadial.subCategory);
        }
    }

    //duplicated code
    public void UseItem(FavItemButton button)
    {
        var itemCLass = button.GetItemInventory();
        var itemData = button.GetItemData();


        Hypatios.RPG.UseItem(itemCLass, itemData);

        RefreshUIContainer();
    }

    public void OpenUIFavorite(ItemInventory.SubiconCategory subCategory)
    {
        ChangeMode(Mode.SelectItems);
        _currentSubCategory = subCategory;

        if (subCategory != ItemInventory.SubiconCategory.Default)
        {
            title.text = subCategory.ToString().ToUpper();
        }
        else
        {
            title.text = "FAVORITED";
        }

        RefreshUIContainer();
    }

    public void RefreshUIContainer()
    {
        foreach (var button in allFavButtons)
        {
            Destroy(button.gameObject);
        }
        allFavButtons.Clear();

        //Refresh inventories
        {
            List<int> indexes = new List<int>();
            var All_Items = Hypatios.Player.Inventory.allItemDatas;


            for (int x = 0; x < All_Items.Count; x++)
            {
                var itemData = All_Items[x];
                var itemClass = Hypatios.Assets.GetItem(itemData.ID);
                if (itemClass == null) continue;

                if (itemClass.subCategory == _currentSubCategory && _currentSubCategory != ItemInventory.SubiconCategory.Default && _currentSubCategory != ItemInventory.SubiconCategory.Alcohol)
                {
                    indexes.Add(x);
                }
                else if (_currentSubCategory == ItemInventory.SubiconCategory.Alcohol && itemClass.IsAlcoholic)
                {
                    indexes.Add(x);
                }
                else if (_currentSubCategory == ItemInventory.SubiconCategory.Default)
                {
                    if (itemData.IsFavorite)
                    {
                        indexes.Add(x);
                    }
                }
            }

            foreach (var index in indexes)
            {
                var itemDat = Hypatios.Player.Inventory.allItemDatas[index];
                var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                AssetStorageDatabase.SubiconIdentifier subicon = Hypatios.Assets.GetSubcategoryItemIcon(itemClass.subCategory);
                var newButton = Instantiate(prefabItemButton, parentItems);
                newButton.gameObject.SetActive(true);
                newButton.index = index;
                newButton.Refresh();
                newButton.imageIcon.sprite = itemClass.GetSprite(); //subicon.sprite;
                allFavButtons.Add(newButton);
            }

        }
    }

    public void HighlightButton(FavItemButton button)
    {
        _currentFavItemButton = button;
        var itemDat = Hypatios.Player.Inventory.allItemDatas[button.index];
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);
        if (itemClass == null) return;

        itemStatUI.gameObject.SetActive(true);
        itemIcon.sprite = itemClass.GetSprite();
        itemStat_Title.text = Hypatios.RPG.GetPreviewFav_Title(itemClass, itemDat);
        itemStat_Description.text = Hypatios.RPG.GetPreviewFav_Description(itemClass, itemDat); 
        ShowPreviewHealthRestore();
    }

    public void ShowPreviewHealthRestore()
    {
        healthRestoreBar.gameObject.SetActive(true);
        healthRestore_BorderBar.gameObject.SetActive(true);


    }

    public void HideHealthRestore()
    {
        healthRestoreBar.gameObject.SetActive(false);
        healthRestore_BorderBar.gameObject.SetActive(false);


    }
}
