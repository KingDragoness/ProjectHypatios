using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_PerkOffering : MonoBehaviour
{

    public List<Interact_PerkOffer_Cauldron> allCauldrons = new List<Interact_PerkOffer_Cauldron>();

    private void Start()
    {
        GeneratePerks();
    }

    [Button("Refresh perks")]
    private void GeneratePerks()
    {
        foreach(var cauldron in allCauldrons)
        {
            cauldron.perkCustomEffect = CreateCustomPerkEffect();
            var perk1 = PlayerPerk.GetBasePerk(cauldron.perkCustomEffect.statusCategoryType);
            string s1 = perk1.GetDescriptionTempPerk(cauldron.perkCustomEffect.Value);
            int price = Mathf.FloorToInt(perk1.pricingPerValue * cauldron.perkCustomEffect.Value);

            cauldron.touch_TakePerk.interactDescription = $"{Mathf.FloorToInt(perk1.pricingPerValue * cauldron.perkCustomEffect.Value)} souls";
            cauldron.icon.sprite = perk1.PerkSprite;
            cauldron.dialogInspect.Dialogue_Content = $"{s1} Price: {price} souls";
        }
    }

    public void BuyPerk(Interact_PerkOffer_Cauldron _cauldron)
    {
        var perk1 = PlayerPerk.GetBasePerk(_cauldron.perkCustomEffect.statusCategoryType);
        int price = Mathf.FloorToInt(perk1.pricingPerValue * _cauldron.perkCustomEffect.Value);

        if (price > Hypatios.Game.SoulPoint)
        {
            DialogueSubtitleUI.instance.QueueDialogue($"Not enough souls!", "SYSTEM", 6f, shouldOverride: true);
            MainGameHUDScript.Instance.audio_Error.Play(); 
            return;
        }

        DialogueSubtitleUI.instance.QueueDialogue($"{perk1.TitlePerk} purchased for {price} souls.", "SYSTEM", 6f, shouldOverride: true);
        Hypatios.Player.PerkData.Temp_CustomPerk.Add(_cauldron.perkCustomEffect);
        Hypatios.Player.ReloadStatEffects();
        Hypatios.Game.SoulPoint -= price;

        foreach (var cauldron in allCauldrons)
        {
            cauldron.Deactivate();
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


    public bool IsDuplicatePerkSelect(StatusEffectCategory _category)
    {
        if (allCauldrons.Find(x => x.perkCustomEffect.statusCategoryType == _category) != null)
        {
            return true;
        }

        return false;
    }
}
