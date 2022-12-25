using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_Container : InteractableObject
{

    public string ContainerName = "Container";
    public bool shouldGenerateLoot = true;
    public LootTable lootTable;
    public InventoryData inventory;

    private void Start()
    {
        if (shouldGenerateLoot) GenerateLoot();
    }

    [Button("Reset loot")]
    public void ResetLoot()
    {
        inventory.allItemDatas.Clear();

        GenerateLoot();
    }

    private void GenerateLoot()
    {
        int roll = Random.Range(lootTable.minRoll, lootTable.maxRoll);

        for (int x = 0; x < roll; x++)
        {
            inventory.AddItem(lootTable.GetEntry().item, 1);
        }
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
