using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(GuidComponent))]
public class Interact_Container : InteractableObject
{

    public GuidComponent Guid;
    public string ContainerName = "Container";
    public bool shouldGenerateLoot = true;
    public bool resetOnEnabled = false;
    public LootTable lootTable;
    public Inventory inventory;
    [FoldoutGroup("DEBUG")] public bool printRandomSeed = false;

    private int seed = 0;

    private void Start()
    {
        if (Guid == null) Guid = gameObject.AddComponent<GuidComponent>();
        if (shouldGenerateLoot)
        {
            float random = Random.Range(0f, 1f);

            if (random > lootTable.chanceNotSpawning)
            {
                GenerateLoot();
            }
        }
    }

    private void OnEnable()
    {
        if (Guid == null) return;
        if (resetOnEnabled == true)
        {
            ResetLoot();
        }
    }

    [Button("Reset loot")]
    public void ResetLoot()
    {
        inventory.allItemDatas.Clear();

        GenerateLoot();
    }

    private void GenerateLoot()
    {
        var guidStr = Guid.GetGuid().ToString().Substring(0,5);
        seed = Hypatios.GetSeed() + System.Convert.ToInt32(guidStr, 16) + Application.loadedLevel;
        var RandomSys = new System.Random(seed);
        int roll = RandomSys.Next(lootTable.minRoll, lootTable.maxRoll);


        for (int x = 0; x < roll; x++)
        {
            var item = lootTable.GetEntry(seed + x).item;

            //Hard-coded balance to prevent container spawning multiple 
            //weapon per item container
            if (IsContainWeapon() == false || item.category != ItemInventory.Category.Weapon)
            {
                inventory.AddItem(item, 1);
            }
        }
    }

    public bool IsContainWeapon()
    {
        if (inventory.allItemDatas.Find(x => x.category == ItemInventory.Category.Weapon) != null)
        {
            return true;
        }

        return false;
    }

    //Display custom HUD window for container
    public override string GetDescription()
    {
        //while run, modify quick loot menu screen
        return "Quick Loot";
    }


    //Quick loot
    public override void Interact()
    {

    }
}
