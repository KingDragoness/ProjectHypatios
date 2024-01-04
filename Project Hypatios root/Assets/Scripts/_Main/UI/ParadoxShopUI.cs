using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParadoxShopUI : MonoBehaviour
{

    [System.Serializable]
    public class PerkSprite
    {
        public Sprite sprite;
        public PlayerPerks perk;
    }

    public Text soulPoint_Text;
    public Text tooltip_Text;
    [TextArea(3,5)]
    public string helperString;
    [Space]
    [Header("Preview Unit")]
    public Text descriptionPrev_Text;
    public Text titlePrev_Text;

    [Space]
    public Transform content;
    public ParadoxSectionButtonUI buttonParadoxTemplate;
    public ParadoxSectionButtonUI currentParadoxButton;
    public List<ParadoxSectionButtonUI> allParadoxUIs = new List<ParadoxSectionButtonUI>();

    [Space]
    [Header("Upgrade Center")]
    public Transform contentPerk;
    public ParadoxUpgradeButtonUI buttonParadoxUpgradeTemplate;
    public List<PerkSprite> perkSprites;
    public List<ParadoxUpgradeButtonUI> allParadoxUpgradeUIs = new List<ParadoxUpgradeButtonUI>();


    private ParadoxShopOwner shopOwner;

    private void Start()
    {
        buttonParadoxTemplate.gameObject.SetActive(false);
        buttonParadoxUpgradeTemplate.gameObject.SetActive(false);
    }

    private void OnEnable()
    {

        RefreshUI();
        Unpreview();
        titlePrev_Text.text = "Paradox Shopkeeper";

        if (Hypatios.Game.everUsed_Paradox == false)
        {
            Hypatios.Game.RuntimeTutorialHelp(Hypatios.CodexList.ParadoxShop);
            //MainGameHUDScript.Instance.ShowPromptUI("PARADOX SHOP", helperString, false);
            Hypatios.Game.everUsed_Paradox = true;
        }

        descriptionPrev_Text.text = "Welcome to the Paradox Shop. You can buy shortcut, secret passage and level modification to shorten the level course.";

        ShowTooltip("Welcome to the Paradox Shop. You can buy shortcut, secret passage and level modification to shorten the level course.");

    }

    public void ShowTooltip(string s)
    {
        tooltip_Text.text = s;
    }

    public void RefreshUI()
    {

        ShowTooltip(" ");
        shopOwner = ParadoxShopOwner.Instance;
        soulPoint_Text.text = Hypatios.Game.SoulPoint.ToString();
        RefreshButtons();
        RefreshPerkButtons();
    }

    private void Update()
    {
        if (buttonParadoxTemplate == null)
        {
        }

        foreach (var STUPIDBUTTON in allParadoxUIs)
        {
            if (currentParadoxButton == STUPIDBUTTON) continue;

            if (STUPIDBUTTON.attachedParadox.isPreviewing == true)
            {
                STUPIDBUTTON.buttonAnimator.SetBool("Highlighted", true);
            }
            else
            {
                STUPIDBUTTON.buttonAnimator.SetBool("Highlighted", false);
            }
        }
    }


    //Removed feature, moved to grim reaper's 
    #region Upgrade

    const float PER_UPGRADE_MAX_HP = 9;
    const float PER_UPGRADE_REGEN_HP = 0.1f;

    public void UpgradePerk(ParadoxUpgradeButtonUI button)
    {
        int price = 0;
        var heal = FindObjectOfType<CharacterScript>().Health;
        int lv_Luck = 1;// Hypatios.Game.Perk_LV_Soulbonus;

        if (button.perkType == PlayerPerks.HealthMax)
        {
            //price = PlayerPerk.GetPrice_MaxHP(heal.maxHealth, heal.maxHealth + PER_UPGRADE_MAX_HP);
        }
        else if (button.perkType == PlayerPerks.HealthRegen)
        {
            //price = PlayerPerk.GetPrice_RegenHP(heal.healthRegen, heal.healthRegen + PER_UPGRADE_REGEN_HP);
        }
        else if (button.perkType == PlayerPerks.LuckGod)
        {
            price = PlayerPerk.GetPrice_LuckOfGod(lv_Luck);

            if (lv_Luck >= 5)
            {
                ShowTooltip("Level maxed out!");
                Debug.Log("Level maxed out!");
                MainGameHUDScript.Instance.audio_Error.Play();
                return;
            }
        }

        if (Hypatios.Game.SoulPoint < price)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;

        }

        Hypatios.Game.SoulPoint -= price;
        MainGameHUDScript.Instance.audio_PurchaseReward.Play();

        if (button.perkType == PlayerPerks.HealthMax)
        {
            //heal.maxHealth = heal.maxHealth + PER_UPGRADE_MAX_HP;
        }
        else if (button.perkType == PlayerPerks.HealthRegen)
        {
            //heal.healthRegen = heal.healthRegen + PER_UPGRADE_REGEN_HP;
        }
        else if (button.perkType == PlayerPerks.LuckGod)
        {
            //Hypatios.Game.LuckOfGod_Level++;
        }

        RefreshUI();
        TooltipHover_Perk(button);
    }

    public void TooltipHover_Perk(ParadoxUpgradeButtonUI button)
    {
        string s = "";
        int price = 0;
        var heal = FindObjectOfType<CharacterScript>().Health;

        if (button.perkType == PlayerPerks.HealthMax)
        {
            float maxHP = 0; // heal.maxHealth;
            float result = maxHP + PER_UPGRADE_MAX_HP;

            price = PlayerPerk.GetPrice_MaxHP(maxHP, result);
            s = $"Upgrade component: Max HP [{maxHP} -> {result}]" +
            $" [Soul: {price}]";

            titlePrev_Text.text = "Upgrade Maximum HP";

        }
        else if (button.perkType == PlayerPerks.HealthRegen)
        {
            float regen = 0; // heal.healthRegen;
            float result = regen + PER_UPGRADE_REGEN_HP;

            price = PlayerPerk.GetPrice_RegenHP(regen, result);
            s = $"Upgrade component: Hitpoint regen [{regen} -> {Mathf.Round(result*10)/10} per second]" +
            $" [Soul: {price}]";

            titlePrev_Text.text = "Upgrade HP regen";
        }
        else if (button.perkType == PlayerPerks.LuckGod)
        {
            int lv_Luck = 1; // Hypatios.Game.Perk_LV_Soulbonus;
            price = PlayerPerk.GetPrice_LuckOfGod(lv_Luck);
            s = $"[LV {lv_Luck + 1}/6]" +
            $" [Soul: {price}]";

            titlePrev_Text.text = "Luck of God";
        }

        descriptionPrev_Text.text = $"{s}";
        Unpreview();
        ShowTooltip($"Buy for {price} souls");

        if (button.perkType == PlayerPerks.LuckGod)
        {
            //descriptionPrev_Text.text = $"{PlayerPerk.GetDescription_LuckOfGod(Hypatios.Game.Perk_LV_Soulbonus)} {s} ";
        }


    }

    #endregion


    #region Refresh Buttons

    private void RefreshPerkButtons()
    {
        foreach (var buttonUi in allParadoxUpgradeUIs)
        {
            Destroy(buttonUi.gameObject);
        }

        allParadoxUpgradeUIs.Clear();
        var values = System.Enum.GetValues(typeof(PlayerPerks)).Cast<PlayerPerks>();

        foreach (var perk in values)
        {
            var newButton = Instantiate(buttonParadoxUpgradeTemplate, contentPerk);
            var sprite1 = perkSprites.Find(x => x.perk == perk);

            newButton.perkType = perk;
            newButton.gameObject.SetActive(true);
            if (sprite1 != null)
            {
                newButton.icon.sprite = sprite1.sprite;
            }

            allParadoxUpgradeUIs.Add(newButton);
        }
    }

    private void RefreshButtons()
    {
        foreach (var buttonUi in allParadoxUIs)
        {
            Destroy(buttonUi.gameObject);
        }

        allParadoxUIs.Clear();

        if (shopOwner != null)
        {
            shopOwner.RefreshAllParadoxes();

            var filteredParadoxes = shopOwner.paradoxLevelScripts;

            if (Hypatios.Game.DEBUG_ShowAllParadox == false)
                filteredParadoxes.RemoveAll(x => x.isRequireTrivia == true && x.IsTriviaFulfilled() == false);

            foreach (var levelScript in filteredParadoxes)
            {
                var newButton = Instantiate(buttonParadoxTemplate, content);
                newButton.gameObject.SetActive(true);
                var rt = newButton.GetComponent<RectTransform>();
                rt.localScale = Vector3.one;
                rt.localPosition = Vector3.zero;

                newButton.paradoxName_Text.text = levelScript.paradoxName;
                newButton.paradoxPrice_Text.text = $"{levelScript.soulPrice} Souls";
                newButton.attachedParadox = levelScript;

                if (newButton.attachedParadox.paradoxEntity.value == newButton.attachedParadox.buyTargetValue)
                {
                    newButton.buttonBuy.interactable = false;
                }
                else
                {
                    if (Hypatios.Game.SoulPoint < newButton.attachedParadox.soulPrice)
                    {
                        newButton.buttonBuy.interactable = false;
                    }
                    else
                    {
                        newButton.buttonBuy.interactable = true;
                    }
                }

                allParadoxUIs.Add(newButton);
            }
        }
    }

    public void HoverThis(ParadoxSectionButtonUI sectionUI)
    {
        Unpreview();
        currentParadoxButton = sectionUI;
        var paradox = currentParadoxButton.attachedParadox;

        paradox.virtualCamera.SetActive(true);
        descriptionPrev_Text.text = paradox.description;
        titlePrev_Text.text = paradox.paradoxName;

    }

    private void OnDisable()
    {
        Unpreview();

        foreach (var button in allParadoxUIs)
        {
            button.attachedParadox.isPreviewing = false;
        }
    }



    public void Unpreview()
    {
        if (currentParadoxButton == null) return;
        var paradox = currentParadoxButton.attachedParadox;

        if (paradox == null) return;
        paradox.virtualCamera.SetActive(false);
        paradox.isPreviewing = false;
        currentParadoxButton = null;

        descriptionPrev_Text.text = "";
        titlePrev_Text.text = "";
    }


    public void TogglePreview()
    {
        if (currentParadoxButton == null)
        {
            return;
        }

        var paradox = currentParadoxButton.attachedParadox;
        paradox.isPreviewing = !paradox.isPreviewing;
    }


    #endregion

    public void AttemptBuy(ParadoxSectionButtonUI button)
    {
        if (Hypatios.Game.SoulPoint < button.attachedParadox.soulPrice)
        {
            ShowTooltip("Not enough souls!");
            Debug.Log("Insufficient souls!");
            MainGameHUDScript.Instance.audio_Error.Play();
            return;
        }

        button.attachedParadox.paradoxEntity.value = button.attachedParadox.buyTargetValue;
        Hypatios.Game.SoulPoint -= button.attachedParadox.soulPrice;

        RefreshUI();

    }

}
