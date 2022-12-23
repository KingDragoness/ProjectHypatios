using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Chest", menuName = "Hypatios/Loot Table", order = 1)]
public class LootTable : ScriptableObject
{

    [System.Serializable]
    public class Entry
    {
        public ItemInventory item;
        public int weight = 10;
    }

    [ProgressBar(1, "maxRoll")]
    public int minRoll = 1;
    [ProgressBar("minRoll", 100)]
    public int maxRoll = 2;

    public List<Entry> entries = new List<Entry>();

    public int GetTotalWeight()
    {
        int total = 0;
        foreach(var entry1 in entries)
        {
            total += entry1.weight;
        }
        return total;
    }

    public Entry GetEntry()
    {
        int output = 0;

        //Getting a random weight value
        var totalWeight = GetTotalWeight();
        
        var rndWeightValue = Random.Range(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in entries)
        {
            processedWeight += entry.weight;
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }

        return entries[output];
    }

}
