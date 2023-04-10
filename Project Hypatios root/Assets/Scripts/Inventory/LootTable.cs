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

    public Entry GetEntry(int customSeed = 0)
    {
        int output = 0;
        var seed = Hypatios.GetSeed() + customSeed;
        var RandomSys = new System.Random(seed);

        //Getting a random weight value
        var totalWeight = GetTotalWeight();
        int rndWeightValue = RandomSys.Next(1, totalWeight + 1);
        //Debug.Log($"SEED: {seed} | Pointer: {rndWeightValue} | Totalweight: {totalWeight}");


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
