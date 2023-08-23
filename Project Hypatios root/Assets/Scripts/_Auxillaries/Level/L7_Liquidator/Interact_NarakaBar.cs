using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_NarakaBar : MonoBehaviour
{

    [System.Serializable]
    public class DrinkList
    {
        public ItemInventory item;
        public GameObject modelObject;
    }

    public List<DrinkList> allDrinkList = new List<DrinkList>();
    public Interact_NarakaBar_Button narakaButtonPrefab;
    public int limitButton = 10;
    public float buttonDistance = 0.6f;
    public Transform pivot_button;
    public Transform parent_button;
    public GameObject screen_Menus;
    public GameObject screen_Cocktail;
    public TextMesh label_cocktailProgress;
    [FoldoutGroup("Cocktail Machine")] public AnimatorSetBool animationScript;
    [FoldoutGroup("Cocktail Machine")] public float animationTime = 12f;


    private List<Interact_NarakaBar_Button> _allButtons = new List<Interact_NarakaBar_Button>();
    [ShowInInspector] private int _positionMenu = 0;
    private float timer_Cocktail = 0f;
    private int selectedDrinkIndex = 0;
    private bool isMakingCocktail = false;


    private void Start()
    {
        narakaButtonPrefab.gameObject.SetActive(false);

        foreach(var drinkItem in allDrinkList)
        {
            if (drinkItem.modelObject != null) drinkItem.modelObject.gameObject.SetActive(false);
        }

        Refresh_Buttons();
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        UpdateUI();
        if (isMakingCocktail == true) MakingCocktail();
    }

    #region UIs

    private void UpdateUI()
    {
        if (isMakingCocktail)
        {
            if (screen_Menus.activeSelf == true) screen_Menus.gameObject.SetActive(false);
            if (screen_Cocktail.activeSelf == false) screen_Cocktail.gameObject.SetActive(true);
            float percent = 1f - (timer_Cocktail / animationTime);
            percent *= 100f;
            percent = Mathf.Round(percent);
            label_cocktailProgress.text = $"MAKING \nCOCKTAIL... ({percent}%)";
        }
        else
        {
            if (screen_Menus.activeSelf == false) screen_Menus.gameObject.SetActive(true);
            if (screen_Cocktail.activeSelf == true) screen_Cocktail.gameObject.SetActive(false);
        }
    }

    private void MakingCocktail()
    {
        timer_Cocktail -= Time.deltaTime;

        if (timer_Cocktail < 0f)
        {
            //add item
            var drinkList = allDrinkList[selectedDrinkIndex];
            Hypatios.Player.Inventory.AddItem(drinkList.item, 1);
            MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(drinkList.item, 1);
            animationScript.SetBool(false);

            timer_Cocktail = 1f;
            isMakingCocktail = false;
        }
    }

    #endregion

    [Button("Reorder List")]
    public void ReorderDrinkList()
    {
        allDrinkList = allDrinkList.OrderBy(x => x.item.subCategory).ThenBy(x => x.item.GetDisplayText()).ToList();
    }

    public void CycleButtonPos(bool isUp)
    {
        int totalPage = Mathf.CeilToInt((float)allDrinkList.Count / (float)limitButton); //FUCK YOU FOR DIVIDING IN INTEGER
        //Debug.Log(totalPage);

        if (isUp)
        {
            if (_positionMenu + 1 >= totalPage)
            {
                _positionMenu = 0;
            }
            else
            {
                _positionMenu++;
            }
        }
        else
        {
            if (_positionMenu - 1 < 0)
            {
                _positionMenu = totalPage - 1;
            }
            else
            {
                _positionMenu--;
            }
        }
        Refresh_Buttons();
    }

    [FoldoutGroup("DEBUG")] [Button("Refresh UI")]
    public void DEBUG_RefreshButton()
    {
        Refresh_Buttons();
    }

    private void Refresh_Buttons()
    {
        foreach(var button in _allButtons)
        {
            Destroy(button.gameObject);
        }

        _allButtons.Clear();

        int curIndex = _positionMenu * limitButton;
        int upperLimit = (_positionMenu + 1) * limitButton;

        if ((_positionMenu + 1) * limitButton > allDrinkList.Count)
        {
            upperLimit = allDrinkList.Count;
        }

        for (int i = curIndex; i < upperLimit; i++)
        {
            var localIndex = i - (_positionMenu * limitButton);
            var currentItem = allDrinkList[i];
            var newButton = Instantiate(narakaButtonPrefab, parent_button);
            var subIcon = Hypatios.Assets.GetSubcategoryItemIcon(currentItem.item.subCategory);
            Vector3 pos = narakaButtonPrefab.transform.position;
            pos.y -= buttonDistance * localIndex;
            newButton.label_ItemName.text = currentItem.item.GetDisplayText();
            newButton.touchable.interactDescription = $"Buy {currentItem.item.GetDisplayText()}";
            newButton.iconMesh.material = subIcon.material;
            newButton.label_Soul.text = $"{currentItem.item.value} Souls";
            newButton.transform.position = pos;
            newButton.index = i;
            newButton.gameObject.SetActive(true);
            _allButtons.Add(newButton);
        }
    }



    public bool BuyDrink(Interact_NarakaBar_Button button)
    {
        var drinkList = allDrinkList[button.index];

        if (drinkList.item == null)
        {
            Debug.Log("No item!");
            return false;
        }

        if (Hypatios.Game.SoulPoint < drinkList.item.value)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Not enough souls! {drinkList.item.value} souls required.", 4f);
            return false;
        }

        Hypatios.Game.SoulPoint -= drinkList.item.value;

        if (drinkList.modelObject == null)
        {
            Hypatios.Player.Inventory.AddItem(drinkList.item, 1);
            MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(drinkList.item, 1);
            DeadDialogue.PromptNotifyMessage_Mod($"Purchased {drinkList.item.GetDisplayText()} for {drinkList.item.value} souls.", 4f);
        }
        else
        {
            animationScript.SetBool(true);
            timer_Cocktail = animationTime;
            selectedDrinkIndex = button.index;
            isMakingCocktail = true;
            DeadDialogue.PromptNotifyMessage_Mod($"Purchased {drinkList.item.GetDisplayText()} for {drinkList.item.value} souls. Wait for the cocktail to finish mixing.", 6f);

        }

        return true;
    }

}
