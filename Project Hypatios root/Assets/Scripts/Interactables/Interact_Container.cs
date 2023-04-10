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
    public LootTable lootTable;
    public InventoryData inventory;
    [FoldoutGroup("DEBUG")] public bool printRandomSeed = false;

    private int seed = 0;

    private void Start()
    {
        if (Guid == null) Guid = gameObject.AddComponent<GuidComponent>();
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
        var guidStr = Guid.GetGuid().ToString().Substring(0,5);
        seed = Hypatios.GetSeed() + System.Convert.ToInt32(guidStr, 16);
        var RandomSys = new System.Random(seed);
        int roll = RandomSys.Next(lootTable.minRoll, lootTable.maxRoll);

        for (int x = 0; x < roll; x++)
        {
            inventory.AddItem(lootTable.GetEntry(seed + x).item, 1);
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
