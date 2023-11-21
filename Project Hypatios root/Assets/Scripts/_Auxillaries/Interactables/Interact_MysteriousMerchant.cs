using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_MysteriousMerchant : MonoBehaviour
{

    public List<Interact_MysteriousMerchant_Cardbox> allItemToSell = new List<Interact_MysteriousMerchant_Cardbox>();
    public LootTable lootTable;
    [Space]
    public bool IsGenerateOnStart = false;
    public bool GenerateOnEnable = false;

    private bool hasStarted = false;
    private List<ItemInventory> alreadyExistItems = new List<ItemInventory>();

    private void Start()
    {
        if (IsGenerateOnStart)
        {
            GenerateLoot();
        }

        hasStarted = true;
    }


    private void OnEnable()
    {
        if (hasStarted == false)
        {
            return;
        }

        if (GenerateOnEnable)
            GenerateLoot();
    }

    public void Buy(Interact_MysteriousMerchant_Cardbox cardbox)
    {

        if (Hypatios.Game.SoulPoint < cardbox.item.value)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Not enough souls! {cardbox.item.value} souls required.", 4f);
            return;
        }

        Hypatios.Game.SoulPoint -= cardbox.item.value;

        {
            cardbox.count--;
            Hypatios.Player.Inventory.AddItem(cardbox.item, 1);
            MainGameHUDScript.Instance.lootItemUI.NotifyItemLoot(cardbox.item, 1);
            DeadDialogue.PromptNotifyMessage_Mod($"Purchased {cardbox.item.GetDisplayText()} for {cardbox.item.value} souls.", 4f);

            if (cardbox.count <= 0)
            {
                DeadDialogue.PromptNotifyMessage_Mod($"{cardbox.item.GetDisplayText()} sold out!", 5f);
            }
        }

        cardbox.Refresh();
    }


    public void GenerateLoot()
    {
        alreadyExistItems.Clear();
        int attempt = 0;

        foreach(var cardbox in allItemToSell)
        {
            int roll = Random.Range(lootTable.minRoll, lootTable.maxRoll);
            var item = lootTable.GetEntry(attempt);
            int attemptX = 0;

            while (IsSimilarItem(item.item) && attemptX < 100)
            {
                item = lootTable.GetEntry(attemptX);
                attemptX++;
            }

            cardbox.gameObject.SetActive(true);
            cardbox.item = item.item;
            cardbox.count = Count(item, roll);
            cardbox.Refresh();
            alreadyExistItems.Add(item.item);
            attempt++;
        }
    }

    internal int Count(LootTable.Entry entry, int roll)
    {
        int count = 1;

        for (int x = 0; x < roll; x++)
        {
            float random = Random.Range(0f, 1f); //0.7, 0.4
            float chanceThreshold = (float)entry.weight / (float)lootTable.GetTotalWeight(); //0.33

            if (random > chanceThreshold)
            {
                count++;
            } 
        }

        return count;
    }

    public bool IsSimilarItem(ItemInventory itemTarget)
    {
        bool isSimilarExisted = false;

        foreach(var cardBox in allItemToSell)
        {
            if (alreadyExistItems.Find(x => x == itemTarget) == true)
            {
                return true;
            }
        }

        return isSimilarExisted;
    }

}
