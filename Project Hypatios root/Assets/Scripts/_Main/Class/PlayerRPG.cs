using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPG : MonoBehaviour
{
   
    public void UseItem(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData)
    {
        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            if (Hypatios.Player.Weapon.GetGunScript(itemClass.attachedWeapon.nameWeapon) == null
                && Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count <= 3)
            {
                Hypatios.Game.currentWeaponStat.Add(itemData.weaponData);
                Hypatios.Player.Weapon.TransferAllInventoryAmmoToOneItemData(ref itemData);
                Hypatios.Player.Weapon.RefreshWeaponLoadout(itemData.ID);
                Hypatios.Player.Inventory.allItemDatas.Remove(itemData);

                itemData.weaponData.currentAmmo = 0;
            }
            else
            {
                Debug.LogError("Cannot use same weapon/too many weapons equipped");
                return;
            }
        }
        else if (itemClass.category == ItemInventory.Category.Consumables)
        {
            if (itemClass.isGenericItem == false)
            {
                float healSpeed = itemClass.consume_HealAmount / itemClass.consume_HealTime;

                soundManagerScript.instance.PlayOneShot("consume");
                Hypatios.Player.Health.Heal((int)itemClass.consume_HealAmount, healSpeed);
                Hypatios.Player.Health.alcoholMeter += itemClass.consume_AlcoholAmount;

                if (itemClass.isInstantDashRefill)
                {
                    Hypatios.Player.timeSinceLastDash = 10f;
                }
                if (itemClass.statusEffect != null)
                {
                    itemClass.statusEffect.AddStatusEffectPlayer(itemClass.statusEffectTime);
                }
                if (itemClass.statusEffectToRemove.Count > 0)
                {
                    foreach (var sg in itemClass.statusEffectToRemove)
                    {
                        if (sg == null) continue;
                        Hypatios.Player.RemoveStatusEffectGroup(sg);
                    }
                }
                if (itemClass.isTriggerFlag && itemClass.flag != null)
                {
                    itemClass.flag.TriggerFlag(itemClass.runLastFlag);
                }
            }
            else if (itemClass.isGenericItem && itemClass.GENERIC_KTHANID_SERUM)
            {
                Hypatios.Player.Health.alcoholMeter += itemData.SERUM_ALCOHOL;

                foreach (var effect in itemData.SERUM_CUSTOM_EFFECTS)
                {
                    PerkCustomEffect perkOfType = new PerkCustomEffect();
                    perkOfType.statusCategoryType = effect.statusCategoryType;
                    perkOfType.origin = effect.origin + Hypatios.TimeTick;
                    perkOfType.Value = effect.Value;
                    perkOfType.isPermanent = false;
                    perkOfType.timer = effect.timer;
                    Hypatios.Player.PerkData.Temp_CustomPerk.Add(perkOfType);
                }
                foreach (var ailment in itemData.SERUM_AILMENTS)
                {
                    var ailmentClass = Hypatios.Assets.GetStatusEffect(ailment);
                    ailmentClass.AddStatusEffectPlayer(itemData.SERUM_TIME);
                }

                Hypatios.Player.ReloadStatEffects();
            }


            Hypatios.Player.Inventory.RemoveItem(itemData);
        }
        else if (itemClass.category == ItemInventory.Category.Normal && itemClass.IsReadable())
        {
            Hypatios.UI.readableBookUI.OpenText(itemClass.readableBook);
        }
    }

    public string GetItemName(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData)
    {
        if (itemClass.isGenericItem == true)
        {
            if (itemClass.GENERIC_KTHANID_SERUM == true)
            {
                return itemData.SERUM_CUSTOM_NAME;
            }
            else if (itemClass.GENERIC_ESSENCE_POTION == true)
            {
                if (itemData.ESSENCE_TYPE == HypatiosSave.EssenceType.Modifier)
                {
                    var modifier = Hypatios.Assets.GetStatusEffect(itemData.ESSENCE_CATEGORY);
                    string plus = "";

                    for (int z = 0; z < itemData.ESSENCE_MULTIPLIER; z++)
                    {
                        if (z == 0) continue;
                        plus += "+";
                    }

                    return $"{plus}{modifier.GetTitlePerk()} Essence";
                }
                else
                {
                    var ailment = Hypatios.Assets.GetStatusEffect(itemData.ESSENCE_STATUSEFFECT_GROUP);
                    return $"{ailment.GetDisplayText()} Essence";
                }
            }
        }
        return itemClass.GetDisplayText();
    }

    public string GetEssenceName(ModifierEffectCategory _modiferCategory)
    {
        var modifierClass = Hypatios.Assets.GetStatusEffect(_modiferCategory);

        return $"{modifierClass.GetTitlePerk()} Essence";
    }

    public string GetEssenceName(string _statusGroup)
    {
        var ailment = Hypatios.Assets.GetStatusEffect(_statusGroup);

        if (ailment == null)
            return "NULL";

        return $"{ailment.GetDisplayText()} Essence";
    }


    public string GetSerumCustomDescription(HypatiosSave.ItemDataSave itemDataList)
    {
        if (itemDataList.SERUM_CUSTOM_DESCRIPTION != "")
            return itemDataList.SERUM_CUSTOM_DESCRIPTION;

        string s1 = "Gain ";
        List<string> allEntries = new List<string>();
        int effectCount = 0;

        for(int x = 0; x < itemDataList.SERUM_CUSTOM_EFFECTS.Count; x++)
        {
            var effect = itemDataList.SERUM_CUSTOM_EFFECTS[x];
            var modifier = Hypatios.Assets.GetStatusEffect(effect.statusCategoryType);
            if (modifier == null) continue;

            allEntries.Add($"{GetDescription(modifier.category, effect.Value, true)} {modifier.GetTitlePerk()}");
            effectCount++;

            //if (x < itemDataList.SERUM_CUSTOM_EFFECTS.Count - 2) s1 += ", ";
            //else if (x <= itemDataList.SERUM_CUSTOM_EFFECTS.Count - 2) s1 += " and ";
        }

        foreach (var ailment in itemDataList.SERUM_AILMENTS)
        {
            var ailmentClass = Hypatios.Assets.GetStatusEffect(ailment);
            allEntries.Add($"{ailmentClass.GetDisplayText()}");
            effectCount++;
        }

        for (int x = 0; x < allEntries.Count; x++)
        {
            string s2 = allEntries[x];

            if (x < allEntries.Count - 2) s2 += ", ";
            else if (x <= allEntries.Count - 2) s2 += " and ";

            s1 += s2;
        }

        s1 += $" for {itemDataList.SERUM_TIME}s but gain {itemDataList.SERUM_ALCOHOL}% alcohol.";

        return s1;
    }

    public string GetDescription(ModifierEffectCategory statusEffect, float value, bool isAdditive = false)
    {
        string s = "";

        if (value > 0 && isAdditive == true)
        {
            s += "+";
        }

        if (statusEffect == ModifierEffectCategory.MovementBonus)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.BonusDamageMelee)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.BonusDamageGun)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.RegenHPBonus)
        {
            float value_ = value;
            s += $"{value_} HP/s";
        }
        else if (statusEffect == ModifierEffectCategory.RegenHPPercentage)
        {
            s += $"{Mathf.FloorToInt(value * 1000f)/10f}%";
        }
        else if (statusEffect == ModifierEffectCategory.MaxHitpointBonus)
        {
            float value_ = value;
            s += $"{value_} HP";
        }
        else if (statusEffect == ModifierEffectCategory.MaxHPPercentage)
        {
            s += $"{Mathf.FloorToInt(value * 1000f) / 10f}%";
        }
        else if (statusEffect == ModifierEffectCategory.Recoil)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.KnockbackResistance)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.DashCooldown)
        {
            s += $"{value}s";
        }
        else if (statusEffect == ModifierEffectCategory.Alcoholism)
        {
            s += $"{value}/100%";
        }
        else if (statusEffect == ModifierEffectCategory.Digestion)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else if (statusEffect == ModifierEffectCategory.ArmorRating)
        {
            s += $"{Mathf.RoundToInt(value * 100)}%";
        }
        else
        {
            s = $"{value}";
        }

        return s;
    }

    public string GetModiferValueString_PermaPerk(ModifierEffectCategory category)
    {
        float value = 0;
        string s = "";
        HypatiosSave.PerkDataSave PerkData = Hypatios.Player.PerkData;

        if (category == ModifierEffectCategory.MaxHitpointBonus)
        {
            value = PlayerPerk.GetValue_MaxHPUpgrade(PerkData.Perk_LV_MaxHitpointUpgrade);
            s = $"{value} HP";
        }
        else if (category == ModifierEffectCategory.RegenHPBonus)
        {
            value = PlayerPerk.GetValue_RegenHPUpgrade(PerkData.Perk_LV_RegenHitpointUpgrade);
            if (value == 0) s = $"{value} HP/s";
            else if (value > 0) s = $"{value} HP/s";
            else s = $"-{value} HP/s";
        }
        else if (category == ModifierEffectCategory.KnockbackResistance)
        {
            value = PlayerPerk.GetValue_KnockbackResistUpgrade(PerkData.Perk_LV_KnockbackRecoil);
            s = $"-{value}";
        }
        else if (category == ModifierEffectCategory.Recoil)
        {
            value = PlayerPerk.GetValue_RecoilUpgrade(PerkData.Perk_LV_WeaponRecoil);
            s = $"-{value}";
        }
        else if (category == ModifierEffectCategory.BonusDamageMelee)
        {
            value = PlayerPerk.GetValue_BonusMeleeDamage(PerkData.Perk_LV_IncreaseMeleeDamage);
            s = $"{value * 100}%";
        }
        else if (category == ModifierEffectCategory.BonusDamageGun)
        {
            value = PlayerPerk.GetValue_BonusGunDamage(PerkData.Perk_LV_IncreaseGunDamage);
            s = $"{value * 100}%";
        }
        else if (category == ModifierEffectCategory.DashCooldown)
        {
            value = PlayerPerk.GetValue_Dashcooldown(PerkData.Perk_LV_DashCooldown);
            s = $"{value}s";
        }
        else if (category == ModifierEffectCategory.SoulBonus)
        {
            value = Hypatios.Player.GetNetSoulBonusPerk();
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.ShortcutDiscount)
        {
            value = Hypatios.Player.GetNetShortcutPerk();
            s = $"Lv {value}/5";
        }
        else if (category == ModifierEffectCategory.Alcoholism)
        {
            value = Mathf.RoundToInt(Hypatios.Player.Health.alcoholMeter);
            s = $"{value}/100%";
        }

        return s;
    }

    public string GetPreviewFav_Description(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData)
    {
        string s1 = "";

        if (itemClass.GENERIC_KTHANID_SERUM)
        {
            s1 += $"{GetSerumCustomDescription(itemData)}";
        }
        else
        {
            s1 += $"\n{itemClass.Description}\n";
        }

        return s1;
    }

    public string GetPreviewFav_Title(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData)
    {
        return GetItemName(itemClass, itemData);
    }

    public string GetPreviewItemLeftSide(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData, bool ignoreInteractTooltip = false)
    {
        string sLeft = "";
        string s_interaction = "";

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            var weaponClass = itemClass.attachedWeapon;
            var weaponSave = itemData.weaponData;
            var weaponStat = weaponClass.GetFinalStat(weaponSave.allAttachments);
            int totalAmmoOfType = Hypatios.Player.Weapon.GetTotalAmmoOfWeapon(weaponClass.nameWeapon);
            string s_allAttachments = "";
            string s_Description = $"{itemClass.Description}";

            bool isSimilarWeaponEquipped = false;
            bool isTooManyEqupped = false;

            if (Hypatios.Player.Weapon.GetGunScript(itemClass.attachedWeapon.nameWeapon) != null) isSimilarWeaponEquipped = true;
            if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count >= 4) isTooManyEqupped = true;

            foreach (var attachment in weaponSave.allAttachments)
            {
                s_allAttachments += $"{weaponClass.GetAttachmentName(attachment)}, ";
            }

            if (!isSimilarWeaponEquipped && !isTooManyEqupped)
            {
                s_interaction = "<'LMB' to equip weapon>\n<Hold 'X' to destroy item>\n<'F' to favorite>";
            }
            else
            {
                s_interaction = "<Hold 'X' to destroy item>\n<'F' to favorite>";
            }


            sLeft += $"Damage\n";
            sLeft += $"Fire rate\n";
            sLeft += $"Mag size\n";
            sLeft += $"Ammo Left\n";
            sLeft += $"\n<size=13>{s_Description}</size>\n";
            sLeft += $"\n<size=13>{s_interaction}</size>\n";

        }
        else
        {
            if (itemClass.category == ItemInventory.Category.Normal && itemClass.IsReadable())
            {
                s_interaction = "<'LMB' to read>\n<Hold 'X' to destroy item>\n<'F' to favorite>";
            }
            else if (itemClass.category != ItemInventory.Category.Consumables)
            {
                s_interaction = "<Hold 'X' to destroy item>\n<'F' to favorite>";
            }
            else
            {
                s_interaction = "<Hold 'LMB' to consume>\n<Hold 'X' to destroy item>\n<'F' to favorite>";
            }

            if (itemData.isGenericItem == false)
            {
                sLeft += $"{itemClass.GetDisplayText()}\n";
                sLeft += $"\n{itemClass.Description}\n";
            }
            else
            {
                if (itemData.GENERIC_KTHANID_SERUM == true)
                {
                    sLeft += $"{itemData.SERUM_CUSTOM_NAME}\n";
                    sLeft += $"\n{GetSerumCustomDescription(itemData)}\n";
                }
                else if (itemData.GENERIC_ESSENCE_POTION == true)
                {
                    sLeft += $"{GetItemName(itemClass, itemData)}\n";
                    sLeft += $"\n{itemClass.Description}\n";
                    //sLeft += $"\n{GetSerumCustomDescription(itemData)}\n";
                }
            }


            if (ignoreInteractTooltip == false)
            {
                sLeft += $"\n<size=13>{s_interaction}</size>\n";
            }
        }

        return sLeft;

    }


    public string GetPreviewItemRightSide(ItemInventory itemClass, HypatiosSave.ItemDataSave itemData)
    {
        string sRight = "";

        if (itemClass.category == ItemInventory.Category.Weapon)
        {
            var weaponClass = itemClass.attachedWeapon;
            var weaponSave = itemData.weaponData;
            var weaponStat = weaponClass.GetFinalStat(weaponSave.allAttachments);
            int totalAmmoOfType = Hypatios.Player.Weapon.GetTotalAmmoOfWeapon(weaponClass.nameWeapon);
            string s_allAttachments = "";
            string s_Description = $"{itemClass.Description}";

            bool isSimilarWeaponEquipped = false;
            bool isTooManyEqupped = false;

            if (Hypatios.Player.Weapon.GetGunScript(itemClass.attachedWeapon.nameWeapon) != null) isSimilarWeaponEquipped = true;
            if (Hypatios.Player.Weapon.CurrentlyHeldWeapons.Count >= 4) isTooManyEqupped = true;


            sRight += $"{weaponStat.damage}\n";
            sRight += $"{weaponStat.cooldown} per sec\n";
            if (weaponClass.isMachineOfMadness) { sRight += $"∞"; } else { sRight += $"{weaponStat.magazineSize}\n"; }
            if (weaponClass.isMachineOfMadness) { sRight += $"∞"; } else { sRight += $"{totalAmmoOfType}\n"; }

        }
        else
        {
            if (itemClass.category != ItemInventory.Category.Consumables)
            {
            }
            else
            {
            }
      
            sRight += $"({itemData.count})\n";
        }

        return sRight;

    }

    public Sprite GetSprite_StatusEffect(HypatiosSave.ItemDataSave itemDat)
    {
        var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

        if (itemDat.GENERIC_ESSENCE_POTION == true)
        {
            if (itemDat.ESSENCE_CATEGORY != ModifierEffectCategory.Nothing)
            {
                var statusEffect = Hypatios.Assets.GetStatusEffect(itemDat.ESSENCE_CATEGORY);
                return statusEffect.PerkSprite;
            }
            else
            {
                var statusEffectGroup = Hypatios.Assets.GetStatusEffect(itemDat.ESSENCE_STATUSEFFECT_GROUP);
                return statusEffectGroup.PerkSprite;
            }
        }
        else
        {
            return itemClass.GetSprite();
        }
    }    

}
