using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_PerkOffering : MonoBehaviour
{

    public List<Interact_PerkOffer_Cauldron> allCauldrons = new List<Interact_PerkOffer_Cauldron>();
    public bool isPermanentPerk = false;
    public bool multipleAvailablity = false;

    private void Start()
    {
        GeneratePerks();
    }

    [Button("Refresh perks")]
    private void GeneratePerks()
    {
        foreach (var cauldron in allCauldrons)
        {

            if (isPermanentPerk)
            {
                cauldron.statusType = PlayerPerk.RandomPickBasePerk().category;
                var perk1 = PlayerPerk.GetBasePerk(cauldron.statusType);
                string description = perk1.DescriptionPerk;

                if (cauldron.statusType == ModifierEffectCategory.ShortcutDiscount)
                    description = PlayerPerk.GetDescription_Shortcut(Hypatios.Player.PerkData.Perk_LV_ShortcutDiscount);
                if (cauldron.statusType == ModifierEffectCategory.SoulBonus)
                    description = PlayerPerk.GetDescription_LuckOfGod(Hypatios.Player.PerkData.Perk_LV_Soulbonus);

                cauldron.icon.sprite = perk1.PerkSprite;
                cauldron.label_Title.text = perk1.TitlePerk;
                cauldron.label_Description.text = description;
                cauldron.touch_TakePerk.interactDescription = $"Take perk";
                cauldron.dialogInspect.Dialogue_Content = $"{perk1.TitlePerk}: {description}";

            }
            else
            {
                cauldron.perkCustomEffect = CreateCustomPerkEffect();
                var perk1 = PlayerPerk.GetBasePerk(cauldron.perkCustomEffect.statusCategoryType);

                string s1 = perk1.GetDescriptionTempPerk(cauldron.perkCustomEffect.Value);
                int price = Mathf.FloorToInt(perk1.pricingPerValue * cauldron.perkCustomEffect.Value);

                cauldron.touch_TakePerk.interactDescription = $"{Mathf.FloorToInt(perk1.pricingPerValue * cauldron.perkCustomEffect.Value)} souls";
                cauldron.icon.sprite = perk1.PerkSprite;
                cauldron.label_Title.text = perk1.TitlePerk;
                cauldron.label_Description.text = perk1.GetDescriptionTempPerk(cauldron.perkCustomEffect.Value);
                cauldron.dialogInspect.Dialogue_Content = $"{s1} Price: {price} souls";
            }
        }
    }

    public void BuyPerk(Interact_PerkOffer_Cauldron _cauldron)
    {
        var perk1 = PlayerPerk.GetBasePerk(_cauldron.perkCustomEffect.statusCategoryType);
        int price = Mathf.FloorToInt(perk1.pricingPerValue * _cauldron.perkCustomEffect.Value);

        if (price > Hypatios.Game.SoulPoint)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Failed: Not enough souls.", 3f);
            MainGameHUDScript.Instance.audio_Error.Play(); 
            return;
        }

        DeadDialogue.PromptNotifyMessage_Mod($"{perk1.TitlePerk} purchased for {price} souls.", 6f);
        Hypatios.Player.PerkData.Temp_CustomPerk.Add(_cauldron.perkCustomEffect);
        Hypatios.Player.ReloadStatEffects();
        Hypatios.Game.SoulPoint -= price;

        if (multipleAvailablity == false)
        {
            foreach (var cauldron in allCauldrons)
            {
                cauldron.Deactivate();
            }
        }
        else
        {
            _cauldron.Deactivate();
        }
    }

    public void RewardPerk(Interact_PerkOffer_Cauldron _cauldron)
    {
        var perk1 = PlayerPerk.GetBasePerk(_cauldron.statusType);

        DeadDialogue.PromptNotifyMessage_Mod($"Aldrich gained {perk1.TitlePerk} perk.", 6f);
        Hypatios.Player.PerkData.AddPerkLevel(_cauldron.statusType);
        Hypatios.Player.ReloadStatEffects();

        if (multipleAvailablity == false)
        {
            foreach (var cauldron in allCauldrons)
            {
                cauldron.Deactivate();
            }
        }
        else
        {
            _cauldron.Deactivate();
        }
    }


    private PerkCustomEffect CreateCustomPerkEffect()
    {
        PerkCustomEffect customPerk = new PerkCustomEffect();
        float chance1 = Random.Range(0f, 1f);

        var statusTarget = PlayerPerk.RandomPickBaseTempPerk().category;
        customPerk.statusCategoryType = statusTarget;
        customPerk.Generate("PerkOffering");

        return customPerk;
    }


    public bool IsDuplicatePerkSelect(ModifierEffectCategory _category)
    {
        if (allCauldrons.Find(x => x.perkCustomEffect.statusCategoryType == _category) != null)
        {
            return true;
        }

        return false;
    }
}
