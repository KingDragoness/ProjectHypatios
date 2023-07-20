using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_OreTrader : MonoBehaviour
{
    
    [System.Serializable]
    public class OrePriceList
    {
        public ItemInventory itemClass;
        public int sellAmount = 10;
        public int sellSoulPrice = 10;
    }

    public List<OrePriceList> all_PriceList = new List<OrePriceList>();
    public TextMesh label_MetalName;
    public TextMesh label_Count;
    public int index = 0;

    private float _timerRefresh = 1f;

    private void Start()
    {
        RefreshMonitor();
    }

    private void Update()
    {
        if (_timerRefresh > 0f)
        {
            _timerRefresh -= Time.deltaTime;
        }
        else
        {
            RefreshMonitor();
            _timerRefresh = 1f;
        }
    }
    [FoldoutGroup("Debug")]
    [Button("Sell Ores")]
    public void Sell()
    {
        Sell_Item(index);
    }

    [Button("Sell Item")]
    public void SellByCustomIndex(int _s)
    {
        Sell_Item(_s);
    }


    private void Sell_Item(int _index)
    {
        OrePriceList priceList = all_PriceList[_index];
        int count = Hypatios.Player.Inventory.Count(priceList.itemClass.GetID());

        if (count < priceList.sellAmount)
        {
            Hypatios.Dialogue.QueueDialogue($"You need at least x{priceList.sellAmount} to be able to sell {priceList.itemClass.GetDisplayText()}. ({count}/{priceList.sellAmount})"
                , "SYSTEM", 5f);
            RefreshMonitor();

            return;
        }

        int baseReward = priceList.sellSoulPrice;
        int bonusReward = 0;
        {
            int a = (int)Mathf.Clamp((baseReward / 4f), 1f, 10f);
            int countSoul = Random.Range(1, a + 1);

            for (int i = 0; i < countSoul; i++)
            {
                bonusReward += PlayerPerk.GetBonusSouls();
            }
        }
        Hypatios.Player.GetNetSoulBonusPerk();

        Hypatios.Game.SoulPoint += (bonusReward + baseReward);

        if (bonusReward > 0)
            Hypatios.Dialogue.QueueDialogue($"Sold {priceList.itemClass.GetDisplayText()} ({priceList.sellAmount}) for {baseReward} souls (+{bonusReward} extra souls).", "SYSTEM", 3f, shouldOverride: true);
        else
            Hypatios.Dialogue.QueueDialogue($"Sold {priceList.itemClass.GetDisplayText()} ({priceList.sellAmount}) for {baseReward} souls.", "SYSTEM", 3f, shouldOverride: true);

        Hypatios.Player.Inventory.RemoveItem(priceList.itemClass.GetID(), priceList.sellAmount);
        soundManagerScript.instance.Play("reward");



        RefreshMonitor();
    }

    public void RefreshMonitor()
    {

        var priceList = all_PriceList[index];
        int count = Hypatios.Player.Inventory.Count(priceList.itemClass.GetID());
        label_MetalName.text = $"{priceList.itemClass.GetDisplayText().Capitalize()} ({count})";
        label_Count.text = $"x{priceList.sellAmount} for {priceList.sellSoulPrice} souls";
    }

    public void CycleIndex(int netIndex = -1)
    {
        index += netIndex;

        if (index < 0)
            index = all_PriceList.Count - 1;
        if (index > all_PriceList.Count - 1)
            index = 0;

        RefreshMonitor();
    }

}
