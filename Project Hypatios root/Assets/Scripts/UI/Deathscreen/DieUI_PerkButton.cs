using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DieUI_PerkButton : MonoBehaviour
{


    public DeadDialogue deadDialogue;
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;
    public StatusEffectCategory status;
    public PerkCustomEffect customEffect;

    [FoldoutGroup("Debug")] [Button("Refresh Perk")]
    public void RefreshPerk()
    {
        var perk = PlayerPerk.GetBasePerk(status);
        titleText.text = perk.TitlePerk;
        descriptionText.text = perk.DescriptionPerk;
        iconImage.sprite = perk.PerkSprite;

        //make sure there's no temporary perk of soul bonus 
        if (status == StatusEffectCategory.SoulBonus)
        {
            int levelLuck = FPSMainScript.savedata.AllPerkDatas.Perk_LV_Soulbonus;
            descriptionText.text = PlayerPerk.GetDescription_LuckOfGod(levelLuck);
        }

        if (customEffect.statusCategoryType != StatusEffectCategory.Nothing)
        {
            var perk1 = PlayerPerk.GetBasePerk(customEffect.statusCategoryType);
            descriptionText.text = perk1.GetDescriptionTempPerk(customEffect.Value);

        }
        else
        {
        }

    }

    public void Hover()
    {
        //temp perk
        if (customEffect.statusCategoryType != StatusEffectCategory.Nothing)
        {
            var perk = PlayerPerk.GetBasePerk(customEffect.statusCategoryType);

            if (customEffect.statusCategoryType == StatusEffectCategory.SoulBonus)
            {
                int levelLuck = FPSMainScript.savedata.AllPerkDatas.Perk_LV_Soulbonus;
                string s = $"Reaper: \"{PlayerPerk.GetDescription_LuckOfGod(levelLuck)}\"";
                DeadDialogue.PromptNotifyMessage(s, 999f);
            }
            else
                DeadDialogue.PromptNotifyMessage($"Reaper: {perk.GetDialogueTempPerk(customEffect.Value)}" , 999f);

        }
        else //default permanent perk
        {
            var perk = PlayerPerk.GetBasePerk(status);
            DeadDialogue.PromptNotifyMessage($"Reaper: {perk.ReaperDialogue}", 999f);
        }
    }




}
