using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public enum UpgradeWeaponType
{
    Damage,
    MagazineSize,
    Cooldown
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponItem : ScriptableObject
{
    
    [System.Serializable]
    public class WeaponFinalStat
    {
        public float damage = 0;
        public float cooldown = 0;
        public float movespeedMultiplier = 0;
        public float accuracy = 0;
        public float recoilMultiplier = 0;
        public bool isBurn = false;
        public bool isPoison = false;
        public int magazineSize = 0;
        public List<CustomVariable> customVariable = new List<CustomVariable>();

        public CustomVariable GetCustomVariable(string ID)
        {
            return customVariable.Find(x => x.variableName == ID);
        }
    }

    [System.Serializable]
    public class Recipe
    {
        [HorizontalGroup] [HideLabel] public ItemInventory inventory;
        [HorizontalGroup] [LabelWidth(40)] public int count = 1;
    }

    [System.Serializable]
    public class Attachment
    {
        public string ID = "StandardReceiver";
        public List<Recipe> RequirementCrafting = new List<Recipe>();
        [FoldoutGroup("Details")] public string Name = "Standard Receiver";
        [FoldoutGroup("Details")]
        [TextArea(3,5)]
        public string Description = "Deals additional damage and reduces cooldown. Worse accuracy.";
        [FoldoutGroup("Details")] public AttachementSlot slot;
        [FoldoutGroup("Details")] public int order = 10; //higher value means its prioritized first
        [FoldoutGroup("Modifiers")] public float modifier_damage = 10;
        [FoldoutGroup("Modifiers")] public float modifier_cooldown = 0.2f;
        [FoldoutGroup("Modifiers")] public float modifier_percent_movespeed = 0f;
        [FoldoutGroup("Modifiers")] public float modifier_percent_accuracy = 1;
        [FoldoutGroup("Modifiers")] public float modifier_percent_recoil = 1;
        [FoldoutGroup("Modifiers")] public List<CustomVariable> modifier_customVariable = new List<CustomVariable>();
        [FoldoutGroup("Modifiers")] public int override_magazineSize = 10;
        [FoldoutGroup("Modifiers")] public bool stat_burn = false;
        [FoldoutGroup("Modifiers")] public bool stat_poison = false;

        public CustomVariable GetCustomVariable(string ID)
        {
            return modifier_customVariable.Find(x => x.variableName == ID);
        }

        public string GetRequirementText()
        {
            string s_allRecipes = "";
            int i = 0;
            foreach (var recipe in RequirementCrafting)
            {
                s_allRecipes += $"({Hypatios.Player.Inventory.Count(recipe.inventory.GetID())}/{recipe.count}) {recipe.inventory.GetDisplayText()}";
                if (i < RequirementCrafting.Count - 1)
                {
                    s_allRecipes += ", ";
                }
                
                i++;
            }

            return s_allRecipes;
        }

    }

    public enum AttachementSlot
    {
        Frame,
        Receiver,
        Grip,
        Scope,
        Enchancement,
        Magazine,
        Tank
    }

    public enum Category
    {
        Melee = -1,
        Pistol = 0,
        Shotgun,
        SMG,
        Rifle,
        MiningBeam,
        IonBlaster,
        Katana = 1000,
        ThrowingKnife
    }


    public string nameWeapon = "Pistol";
    public GameObject prefab;
    public Template_AmmoAddedIcon UI_TemplateAmmoAdded;
    public Sprite weaponIcon;
    public Sprite ammoSpriteIcon;
    public Sprite overrideCrosshair_Sprite;
    public List<Attachment> attachments = new List<Attachment>();
    [ShowIf("isCraftable", false)] public List<Recipe> WeaponRequirementCrafting = new List<Recipe>();
    [FoldoutGroup("Ammo Crafts")] public List<Recipe> AmmoRequirementCrafting = new List<Recipe>();
    [FoldoutGroup("Ammo Crafts")] public int craft_AmmoAmount = 20;
    [FoldoutGroup("Charge Station")] public int buy_AmmoAmount = 20;
    [FoldoutGroup("Charge Station")] public int buy_AmmoSoulPrice = 2;
    [FoldoutGroup("Charge Station")] public int buy_SoulPrice = 2;
    public bool isCraftable = false;
    public int defaultDamage;
    public int defaultMagazineSize;
    public float defaultCooldown;
    public AnimationCurve rewardRate;
    public float moveSpeed = 1f; //1/100%, normal speed. 0.5/50% for heavy weapons
    public List<CustomVariable> defaultVariables = new List<CustomVariable>();


    public WeaponFinalStat GetFinalStat(List<string> allAttachments)
    {
        WeaponFinalStat stat = new WeaponFinalStat();
        stat.damage = defaultDamage;
        stat.magazineSize = defaultMagazineSize;
        stat.cooldown = defaultCooldown;
        stat.recoilMultiplier = 1f;
        var newList = attachments.OrderBy(x => x.order).ToList();

        //Initialize default variables
        foreach (var var1 in defaultVariables)
        {
            var statVariable = stat.GetCustomVariable(var1.variableName);

            if (statVariable == null)
            {
                statVariable = new CustomVariable(var1.variableName);
                statVariable.value = var1.value;
                stat.customVariable.Add(statVariable);
            }
        }

        foreach (var attach in newList)
        {
            if (allAttachments.Contains(attach.ID) == false) continue;
            stat.damage += attach.modifier_damage;
            stat.cooldown += attach.modifier_cooldown;
            stat.movespeedMultiplier += attach.modifier_percent_movespeed;
            stat.accuracy += attach.modifier_percent_accuracy;
            stat.recoilMultiplier += attach.modifier_percent_recoil;
            if (attach.stat_burn) stat.isBurn = attach.stat_burn;
            if (attach.stat_poison) stat.isPoison = attach.stat_poison;
            if (attach.override_magazineSize > 0) stat.magazineSize = attach.override_magazineSize;


            foreach (var var1 in defaultVariables)
            {
                var customVariable = attach.GetCustomVariable(var1.variableName);
                var statVariable = stat.GetCustomVariable(var1.variableName);

                if (customVariable == null) continue;

                if (customVariable.type == CustomVariable.Type.FlatAdd)
                    statVariable.value += customVariable.value;

                if (customVariable.type == CustomVariable.Type.Override)
                    statVariable.value = customVariable.value;
            }
        }

        if (stat.magazineSize <= 0) stat.magazineSize = defaultMagazineSize;

        return stat;
    }

    public string GetRequirementText()
    {
        string s_allRecipes = "";
        int i = 0;
        foreach (var recipe in WeaponRequirementCrafting)
        {
            s_allRecipes += $"({Hypatios.Player.Inventory.Count(recipe.inventory.GetID())}/{recipe.count}) {recipe.inventory.GetDisplayText()}";
            if (i < WeaponRequirementCrafting.Count - 1)
            {
                s_allRecipes += ", ";
            }

            i++;
        }

        return s_allRecipes;
    }

    public string GetRequirementAmmosText()
    {
        string s_allRecipes = "";
        int i = 0;
        foreach (var recipe in AmmoRequirementCrafting)
        {
            s_allRecipes += $"({Hypatios.Player.Inventory.Count(recipe.inventory.GetID())}/{recipe.count}) {recipe.inventory.GetDisplayText()}";
            if (i < AmmoRequirementCrafting.Count - 1)
            {
                s_allRecipes += ", ";
            }

            i++;
        }

        return s_allRecipes;
    }

    public bool IsAttachmentSlotOccupied(string attachID, List<string> allAttachments)
    {
        var attachment = GetAttachmentWeaponMod(attachID);

        foreach(var attach1 in allAttachments)
        {
            var weaponMod = GetAttachmentWeaponMod(attach1);
            if (weaponMod.slot == attachment.slot)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAttachmentExists(string attachID, List<string> allAttachments)
    {
        var attachment = GetAttachmentWeaponMod(attachID);

        foreach (var attach1 in allAttachments)
        {
            if (attach1 == attachment.ID)
            {
                return true;
            }
        }

        return false;
    }


    public bool IsCraftAttachmentRequirementMet(string attachID)
    {
        var attachment = GetAttachmentWeaponMod(attachID);

        foreach(var recipe in attachment.RequirementCrafting)
        {
            if (Hypatios.Player.Inventory.Count(recipe.inventory.GetID()) < recipe.count)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsRequirementMet()
    {
        foreach (var recipe in WeaponRequirementCrafting)
        {
            if (Hypatios.Player.Inventory.Count(recipe.inventory.GetID()) < recipe.count)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsAmmoRequirementMet()
    {

        foreach (var recipe in AmmoRequirementCrafting)
        {
            if (Hypatios.Player.Inventory.Count(recipe.inventory.GetID()) < recipe.count)
            {
                return false;
            }
        }

        return true;
    }


    public string GetAttachmentName(string ID)
    {
        var attach = attachments.Find(x => x.ID == ID);

        if (attach != null)
            return attach.Name;
        else
            return "NULL";
    }

    public Attachment GetAttachmentWeaponMod(string ID)
    {
        var attach = attachments.Find(x => x.ID == ID);

        if (attach != null)
            return attach;
        else
            return null;
    }

}
