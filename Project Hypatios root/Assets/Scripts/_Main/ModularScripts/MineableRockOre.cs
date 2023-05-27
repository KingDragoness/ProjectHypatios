using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MineableRockOre : MonoBehaviour
{

    public bool shouldGenerateLoot = true;
    public LootTable lootTable;
    public Inventory inventory;

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

    public void LootAll()
    {
        GenerateLoot();

        for (int x = inventory.allItemDatas.Count - 1; x >= 0; x--)
        {
            int index = x;
            var itemDat = inventory.allItemDatas[index];
            var itemClass = Hypatios.Assets.GetItem(itemDat.ID);

            inventory.TransferTo(Hypatios.Player.Inventory, index);
            MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(itemDat);
            //DeadDialogue.PromptNotifyMessage_Mod($"Added {itemClass.GetDisplayText()} ({itemDat.count})", 3.5f);
        }

        Hypatios.ObjectPool.SummonParticle(CategoryParticleEffect.RockDestructible, true, transform.position, transform.rotation);
    }

}
