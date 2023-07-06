using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class kThanidUI_SerumCreator : MonoBehaviour
{


    public kThanidLabUI kthanidUI;
    public Text label_Description;
    public Text label_ResultWarning;
    public Text label_ButtonSerumCreate;
    public Button buttonSerumCreate;
    public InputField input_SerumName;
    public Color color_errorText;
    public Color color_warningText;

    [Space]
    public HypatiosSave.ItemDataSave resultSerum = new HypatiosSave.ItemDataSave();

    private string SerumCustomName = "Pathetic Serum";
    private List<PerkCustomEffect> SerumCustomEffects = new List<PerkCustomEffect>();
    private List<string> SerumAilments = new List<string>();
    private float SerumTime = 0f;
    private float SerumAlcoholAmount = 0f;
    private float SerumPotency = 1f;
    private int EssenceCount = 0;
    private bool isSerumValid = false;
    private bool isDuplicateEssences = false;

    private int positiveEssence = 0;
    private int negativeEssence = 0;

    public void Refresh()
    {
        CalculateSerum();
        UpdateWarningText();
        UpdateDescription();

        if (isSerumValid)
        {
            buttonSerumCreate.interactable = true;
        }
        else buttonSerumCreate.interactable = false;

        label_ButtonSerumCreate.text = $"Create {SerumCustomName}";
    }

    private void CalculateSerum()
    {


        //filling the form
        {
            SerumCustomName = input_SerumName.text;
            SerumCustomEffects.Clear();
            SerumAilments.Clear();
            SerumTime = 0;
            SerumAlcoholAmount = 0f;
            SerumPotency = 1f;
            EssenceCount = 0;
            positiveEssence = 0;
            negativeEssence = 0;
            isDuplicateEssences = false;

            int i2 = 0;

            foreach (var itemDat in kthanidUI.FabricatorInventory.allItemDatas)
            {
                if (itemDat.isGenericItem == false && itemDat.GENERIC_ESSENCE_POTION == false)
                {
                    var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

                    if (itemClass.consume_AlcoholAmount > 0 && itemClass.category == ItemInventory.Category.Consumables)
                    {
                        SerumTime += itemClass.consume_HealTime * itemDat.count;
                        SerumAlcoholAmount += itemClass.consume_AlcoholAmount * itemDat.count;
                    }

                    if (itemClass.IS_REACTANT)
                    {
                        SerumAlcoholAmount *= (100f - itemClass.Reactant_ReduceAlcohol * itemDat.count) * 0.01f;
                        SerumPotency *= (100f - itemClass.Reactant_ReduceEfficiency * itemDat.count) * 0.01f ;
                    }
                }

                if (itemDat.isGenericItem == true && itemDat.GENERIC_ESSENCE_POTION)
                {
                    EssenceCount++;

                    if (itemDat.ESSENCE_TYPE == HypatiosSave.EssenceType.Modifier)
                    {
                        bool antiPotion = false;
                        if (kthanidUI.Index_AntiPotions.Contains(i2)) antiPotion = true;
                        if (antiPotion == false) positiveEssence++; else negativeEssence++;
                        
                        if (kthanidUI.FabricatorInventory.allItemDatas.Find(x => x.ESSENCE_CATEGORY == itemDat.ESSENCE_CATEGORY && x != itemDat) != null)
                        {
                            isDuplicateEssences = true;
                        }
                    }
                    else
                    {
                        var ailmentClass = Hypatios.Assets.GetStatusEffect(itemDat.ESSENCE_STATUSEFFECT_GROUP);
                        if (ailmentClass.isNegativeAilment == false) positiveEssence++; else negativeEssence++;

                        if (kthanidUI.FabricatorInventory.allItemDatas.Find(x => x.ESSENCE_STATUSEFFECT_GROUP == itemDat.ESSENCE_STATUSEFFECT_GROUP && x != itemDat) != null)
                        {
                            isDuplicateEssences = true;
                        }
                    }
                }

                i2++;

            }

            int i1 = 0;

            foreach (var itemDat in kthanidUI.FabricatorInventory.allItemDatas)
            {
                if (itemDat.isGenericItem == true && itemDat.GENERIC_ESSENCE_POTION)
                {
                    bool antiPotion = false;

                    if (kthanidUI.Index_AntiPotions.Contains(i1)) antiPotion = true;

                    if (itemDat.ESSENCE_TYPE == HypatiosSave.EssenceType.Modifier)
                    {
                        PerkCustomEffect modifier = new PerkCustomEffect();
                        var modifierClass = Hypatios.Assets.GetStatusEffect(itemDat.ESSENCE_CATEGORY);
                        modifier.statusCategoryType = itemDat.ESSENCE_CATEGORY;

                        if (antiPotion == false)
                        {
                            modifier.Value = modifierClass.baseValue * SerumAlcoholAmount;
                            if (negativeEssence > 0) modifier.Value *= (negativeEssence + 1f);
                            if (positiveEssence > 0) modifier.Value *= 1f / (positiveEssence);
                        }
                        else
                        {
                            modifier.Value = modifierClass.baseValue * SerumAlcoholAmount / EssenceCount;
                        }

                        if (antiPotion) modifier.Value *= -1;
                        SerumCustomEffects.Add(modifier);
                    }
                    else
                    {
                        SerumAilments.Add(itemDat.ESSENCE_STATUSEFFECT_GROUP);
                    }
                }

                i1++;
            }
        }

        resultSerum.ID = "Serum_kThanid";
        resultSerum.count = 1;
        resultSerum.category = ItemInventory.Category.Consumables;
        resultSerum.isGenericItem = true;
        resultSerum.GENERIC_KTHANID_SERUM = true;
        resultSerum.SERUM_CUSTOM_NAME = SerumCustomName;
        resultSerum.SERUM_CUSTOM_EFFECTS = new List<PerkCustomEffect>();
        resultSerum.SERUM_AILMENTS = new List<string>();
        resultSerum.SERUM_ALCOHOL = SerumAlcoholAmount;

        foreach (var effect in SerumCustomEffects)
        {
            var dupeEffect = effect.Clone();
            dupeEffect.origin = resultSerum.SERUM_CUSTOM_NAME;
            dupeEffect.isPermanent = false;
            dupeEffect.timer = SerumTime;
            resultSerum.SERUM_CUSTOM_EFFECTS.Add(dupeEffect);
        }
        foreach (var ailment in SerumAilments)
        {
            resultSerum.SERUM_AILMENTS.Add(ailment);
        }

        resultSerum.SERUM_TIME = SerumTime;
    }

    private void UpdateDescription()
    {
        if (isSerumValid == false)
        {
            label_Description.text = "Cannot craft serum. You need an essence and an alcohol.";
            return;
        }

        label_Description.text = $"{Hypatios.RPG.GetSerumCustomDescription(resultSerum)}";
    }

    private void UpdateWarningText()
    {
        bool isAlcohol = false;
        bool isNamed = false;
        bool isEssence = false;
        bool alcoholOver50 = false;
        bool multipleEssence = false;
        isSerumValid = false;
        string s1 = "";

        foreach(var itemDat in kthanidUI.FabricatorInventory.allItemDatas)
        {
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            if (itemClass.subCategory == ItemInventory.SubiconCategory.Alcohol)
            {
                isAlcohol = true;
            }
            if (itemClass.subCategory == ItemInventory.SubiconCategory.Essence && itemClass.GENERIC_ESSENCE_POTION)
            {
                isEssence = true;
            }
        }

        if (SerumAlcoholAmount >= 50) alcoholOver50 = true;

        if (input_SerumName.text != "" | string.IsNullOrEmpty(input_SerumName.text) == false)
        {
            isNamed = true;
        }

        if (isAlcohol == false)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_errorText)}>[ ! ] NO ALCOHOL</color>\n";
        }
        if (isNamed == false)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_errorText)}>[ ! ] NAME NOT VALID</color>\n";
        }
        if (isEssence == false)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_errorText)}>[ ! ] NO ESSENCE</color>\n";
        }
        if (alcoholOver50)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_warningText)}>( ! ) You can use reactant to reduce the serum's alcohol amount.</color>\n";
        }
        if (positiveEssence == 0 && negativeEssence > 0)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_warningText)}>( ! ) Your current serum can be crafted but there's no positive effect at all.</color>\n";
        }
        if (isDuplicateEssences)
        {
            s1 += $"<color=#{ColorUtility.ToHtmlStringRGB(color_warningText)}>( ! ) Your current serum can be crafted but same modifiers are not stackable upon use.</color>\n";
        }

        if (isAlcohol && isNamed && isEssence)
        {
            isSerumValid = true;
        }

        label_ResultWarning.text = $"{s1}";
    }


    public void CreateSerum()
    {
        Hypatios.Player.Inventory.allItemDatas.Add(resultSerum.Clone());
        kthanidUI.FabricatorInventory.allItemDatas.Clear();
        DeadDialogue.PromptNotifyMessage_Mod($"Crafted {resultSerum.SERUM_CUSTOM_NAME}.", 4f);
        kthanidUI.RefreshUI();

    }
}
